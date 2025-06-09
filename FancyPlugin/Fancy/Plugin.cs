using BepInEx;
using BepInEx.Logging;
using ComputerysModdingUtilities;
using Fancy.Resources;
using HarmonyLib;
using UnityEngine;

[assembly: StraftatMod(isVanillaCompatible: false)]

namespace Fancy;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; }
    internal static new ManualLogSource Logger;

    public static readonly string loadBearingColonThree = ":3";
    private void Awake() {
        if (loadBearingColonThree != ":3") Application.Quit();
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        Instance = this;
        Logger = base.Logger;
        Assets.LoadBundle();
        StartCoroutine(BundleLocator.DiscoverAndLoadBundles());
    
        new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
        Logger.LogInfo("Hiiiiiiiiiiii :3");
    }
}