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

    // We only need map and spawnParams; spawnParams is a struct (value type) so keep it as ref object to mutate it via reflection.
    public static void Prefix(Map map, ref object spawnParams)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Colonists)
        {
            return;
        }

        if (spawnParams == null)
        {
            return;
        }

        var exitLocation = Main.FindBestExitSpot(map);
        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        // Access the nested struct's field extraCellValidator (Predicate<IntVec3>) and set it.
        var spType = spawnParams.GetType();
        var extraCellValidatorField = AccessTools.Field(spType, "extraCellValidator");
        if (extraCellValidatorField == null)
        {
            return; // Field name might differ in future versions.
        }

        extraCellValidatorField.SetValue(spawnParams, (Predicate<IntVec3>)validator);
        return;

        // Assign new validator
        bool validator(IntVec3 cell)
        {
            return cell.DistanceTo(exitLocation) < 10;
        }
    }
}