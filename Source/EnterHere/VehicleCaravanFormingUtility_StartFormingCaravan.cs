using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class VehicleCaravanFormingUtility_StartFormingCaravan
{
    private static bool Prepare()
    {
        return ModLister.GetActiveModWithIdentifier("SmashPhil.VehicleFramework", true) != null;
    }

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method("Vehicles.VehicleCaravanFormingUtility:StartFormingCaravan");
    }

    public static void Prefix(ref IntVec3 exitSpot, List<Pawn> pawns)
    {
        if (!EnterHereMod.instance.EnterHereSettings.Colonists)
        {
            return;
        }

        var pawn = pawns.RandomElement();
        var map = pawn.Map;
        var validVehicleExitSpotMethod = AccessTools.Method("Vehicles.Dialog_FormVehicleCaravan:ValidVehicleExitSpot");
        var vehiclePawnType = AccessTools.TypeByName("Vehicles.VehiclePawn");
        if (validVehicleExitSpotMethod == null || vehiclePawnType == null)
        {
            return;
        }

        var vehiclePawn = Convert.ChangeType(pawns.First(pawnObject => pawnObject.GetType() == vehiclePawnType),
            vehiclePawnType);

        var exitLocation = Main.FindBestExitSpot(map);

        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        if (Validator(exitLocation))
        {
            exitSpot = exitLocation;
            return;
        }

        if (CellFinder.TryFindRandomCellNear(exitLocation, map, 10, Validator, out exitLocation))
        {
            exitSpot = exitLocation;
        }

        return;

        bool Validator(IntVec3 intVec3)
        {
            return (bool)validVehicleExitSpotMethod.Invoke(null, [intVec3, vehiclePawn, map]);
        }
    }
}