using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class EnterMapUtilityVehicles_FindNearEdgeCell
{
    public static bool Prepare()
    {
        return ModLister.GetActiveModWithIdentifier("SmashPhil.VehicleFramework", true) != null;
    }

    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method("Vehicles.World.EnterMapUtilityVehicles:FindNearEdgeCell");
    }

    public static void Prefix(ref Predicate<IntVec3> extraCellValidator, Map map)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Colonists)
        {
            return;
        }

        var exitLocation = Main.FindBestExitSpot(map);

        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        extraCellValidator = cell => cell.DistanceTo(exitLocation) < 10;
    }
}