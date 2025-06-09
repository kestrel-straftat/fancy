using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace Fancy.Patches;

[HarmonyPatch(typeof(CosmeticsManager))]
public static class CosmeticsManagerFixes
{
    // the following patches are purely to fix the shoddy clamping CosmeticsManager does on cosmetic indices. lol
        
    [HarmonyPatch("LoadDress")]
    [HarmonyPrefix]
    public static void ProperBoundsCheck(ref int ___suitIndex, ref int ___hatIndex, ref int ___cigIndex, GameObject[] ___suitsChildren, GameObject[] ___hatsChildren, GameObject[] ___cigsChildren) {
        ___suitIndex = Mathf.Clamp(___suitIndex, 0, ___suitsChildren.Length - 1);
        ___hatIndex = Mathf.Clamp(___hatIndex, 0, ___hatsChildren.Length - 1);
        ___cigIndex = Mathf.Clamp(___cigIndex, 0, ___suitsChildren.Length - 1);
    }
        
    // the original method incorrectly clamps cigIndex using the length of the suitsChildren array for some reason...
    [HarmonyPatch("ChangeDress")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchChangeDressIndexException(IEnumerable<CodeInstruction> instructions) {
        CodeMatch[] loadSuitsLengthMatch = [
            new(
                OpCodes.Ldfld,
                AccessTools.Field(typeof(CosmeticsManager), "suitsChildren")
            ),
            new(
                OpCodes.Ldlen
            )
        ];
        
        return new CodeMatcher(instructions)
            .End()
            .MatchBack(false, loadSuitsLengthMatch)
            .Set(OpCodes.Ldfld, AccessTools.Field(typeof(CosmeticsManager), "cigsChildren"))
            .MatchBack(false, loadSuitsLengthMatch)
            .Set(OpCodes.Ldfld, AccessTools.Field(typeof(CosmeticsManager), "cigsChildren"))
            .InstructionEnumeration();
    }
}