using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fancy.Resources;
using FishNet;
using FishNet.Managing;
using FishNet.Object;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Fancy;

public static class CosmeticLoader
{
    public static bool HasLoadedAllCosmetics { get; private set; }

    private static CosmeticsManager m_manager;
    private static readonly LayerMask m_dontCollideWithPlayerLayer = LayerMask.NameToLayer("DontCollideWithPlayer");
    private static GameObject m_cosmeticInstancePrefab;
    private static List<CosmeticReferenceBase> m_references = [];
    private static int m_suitCount;
    
    public static void RegisterCosmetic(CosmeticReferenceBase reference) {
        if (HasLoadedAllCosmetics) {
            Plugin.Logger.LogError($"CosmeticLoader: tried to register \"{reference.cosmeticName}\" after cosmetics have been loaded; it will be ignored.");
            return;
        }
        
        // insert sorted to ensure order of cosmetic and networkobject indices
        // we can sort by source guid as cosmetic references in a manifest are guaranteed to be loaded in a consistent order
        var index = m_references.TakeWhile(r => string.CompareOrdinal(r.sourceGuid, reference.sourceGuid) < 0).Count();
        m_references.Insert(index, reference);
        
        if (reference is SuitCosmeticReference) ++m_suitCount;
    }

    public static IEnumerator LoadAllWhenReady() {
        // wait for CosmeticManager to init
        while (!m_manager) yield return null;
        Plugin.Logger.LogInfo("Loading all registered cosmetics...");
        m_cosmeticInstancePrefab = Assets.Load<GameObject>("CosmeticInstanceTemplate");
        
        // assumes that the manager will have the same amount of arm mats as suit mats. which i think should always be true..?
        var oldSuitsCount = m_manager.mats.Length;
        var matsToAdd = new Material[m_suitCount];
        var armMatsToAdd = new Material[m_suitCount];
        int suitIdx = 0;

        var networkManager = InstanceFinder.NetworkManager;
        
        foreach (var reference in m_references) {
            bool isHat = false;
            bool isCig = false;
            if (reference is PrefabCosmeticReference pcr) {
                if (pcr.type == PrefabCosmeticType.Hat) {
                    isHat = true;
                    
                    // add networkobject component to. network the object
                    var nob = pcr.prefab.GetComponent<NetworkObject>() ?? pcr.prefab?.AddComponent<NetworkObject>();
                    if (nob) networkManager.SpawnablePrefabs.AddObject(nob);
                    
                    // add hat tag to all children with colliders to ensure proper gun hitreg on hats
                    // also add objects with colliders to the DontCollideWithPlayer layer
                    foreach (var collider in pcr.prefab!.GetComponentsInChildren<Collider>()) {
                        collider.gameObject.tag = "Hat";
                        collider.gameObject.layer = m_dontCollideWithPlayerLayer;
                    }
                }
                else
                    isCig = true;
            }
            
            Transform parentToUse;
            if (isHat)
                parentToUse = m_manager.hatsParent;
            else if (isCig)
                parentToUse = m_manager.cigsParent;
            else
                parentToUse = m_manager.suitsParent;
            
            var cosmeticObject = Object.Instantiate(m_cosmeticInstancePrefab, parentToUse, false);
            
            var instance = cosmeticObject.GetComponent<CosmeticInstance>();
            instance.isHat = isHat;
            instance.isCig = isCig;
            // cosmeticinstance sets the sprite on Start() anyway so to get the default image to work this has to be done
            instance.sprite = reference.cosmeticIcon ?? instance.transform.Find("Image").GetComponent<Image>().sprite;
            instance.hat = (reference as PrefabCosmeticReference)?.prefab;
            instance.cosmeticName = reference.cosmeticName;
            if (reference.dlcOnly) {
                instance.acquired = false;
                instance.unlockWithDlc = true;
            }
            
            if (reference is SuitCosmeticReference sr) {
                matsToAdd[suitIdx] = sr.suitMaterial;
                armMatsToAdd[suitIdx] = sr.armsMaterial;
                instance.index = oldSuitsCount + suitIdx;
                ++suitIdx;
            }
        }
        
        // register new suits
        var newSuitsCount = oldSuitsCount + m_suitCount;
        if (newSuitsCount > oldSuitsCount) {
            // create arrays of materials with new mats in it then set cosmeticmanager's mat fields to them
            var newMats = new Material[newSuitsCount];
            var newArmMats = new Material[newSuitsCount];
            m_manager.mats.CopyTo(newMats, 0);
            m_manager.fparmsMats.CopyTo(newArmMats, 0);
            matsToAdd.CopyTo(newMats, oldSuitsCount);
            armMatsToAdd.CopyTo(newArmMats, oldSuitsCount);
            m_manager.mats = newMats;
            m_manager.fparmsMats = newArmMats;
        }
        
        // call manager start again manually (ew) & also register cigs properly why doesnt the game do that normally 
        m_manager.Start();
        m_manager.cigs = new GameObject[m_manager.cigsChildren.Length];
        
        for (int i = 0; i < m_manager.cigs.Length; ++i) {
            m_manager.cigs[i] = m_manager.cigsChildren[i].hat;
            m_manager.cigsChildren[i].index = i;
        }

        Plugin.Logger.LogInfo("All custom cosmetics loaded!");
        Plugin.Logger.LogWarning($"Your cosmetics code is {BundleLocator.InstalledCode}. MAKE SURE THIS IS *IDENTICAL* TO THE ONE THAT PEOPLE YOU WANT TO PLAY WITH HAVE!"); 
        Plugin.Logger.LogWarning("If your codes do not match, you do not have the same cosmetic bundles installed and strange behaviour may occur. You have been warned!");
        
        HasLoadedAllCosmetics = true;
        m_manager.LoadDress();
    }
    
    #region Patches

    [HarmonyPatch(typeof(CosmeticInstance))]
    public static class CosmeticInsancePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void SetHoverText(CosmeticInstance __instance, string ___cosmeticName) {
            __instance.GetComponent<ButtonSizeTween>().customText = ___cosmeticName.SplitCamelCase();
        }
    }
    
    [HarmonyPatch(typeof(CosmeticsManager))]
    internal static class CosmeticsManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void CaptureManager(CosmeticsManager __instance) {
            m_manager = __instance;
        }
    }

    [HarmonyPatch(typeof(NetworkManager))]
    internal static class NetworkManagerPatch
    {
        // there is *absolutely* a better way to do this~ but every
        // more "official" way i tried failed horribly so here we are
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void RefreshPrefabs(NetworkManager __instance) {
            foreach (var reference in m_references) {
                if (reference is not PrefabCosmeticReference pcr) continue;
                if (pcr.prefab.GetComponent<NetworkObject>() is { } nob)
                    __instance.SpawnablePrefabs.AddObject(nob);
            }
        }
    }
    
    #endregion
}