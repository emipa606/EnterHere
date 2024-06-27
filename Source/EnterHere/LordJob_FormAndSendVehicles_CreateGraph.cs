using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class LordJob_FormAndSendVehicles_CreateGraph
{
    private static IntVec3 exitPoint;

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(LordJob_FormAndSendVehicles_CreateGraph),
            nameof(Dummy));
        if (ModLister.GetActiveModWithIdentifier("SmashPhil.VehicleFramework", true) == null)
        {
            yield break;
        }

        yield return AccessTools.Method("Vehicles.LordJob_FormAndSendVehicles:CreateGraph");
    }

    public static void Prefix(ref IntVec3 ___exitPoint, LordJob_FormAndSendCaravan __instance)
    {
        if (!EnterHereMod.instance.EnterHereSettings.Colonists)
        {
            return;
        }

        var searcher = __instance.lord.ownedPawns.RandomElement();
        var exitLocation = Main.FindBestExitSpot(searcher, TraverseMode.ByPawn, true);

        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        ___exitPoint = exitLocation;
    }

    public static void Dummy()
    {
        // Dummy method to force Harmony to create a patch class
    }
}