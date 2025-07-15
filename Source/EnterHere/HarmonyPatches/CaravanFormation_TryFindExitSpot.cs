using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class CaravanFormation_TryFindExitSpot
{
    private static List<object> caravanVehicles = [];

    public static bool Prepare()
    {
        return ModLister.GetActiveModWithIdentifier("SmashPhil.VehicleFramework", true) != null;
    }

    public static IEnumerable<MethodBase> TargetMethods()
    {
        var caravanFormationType = AccessTools.TypeByName("Vehicles.World.CaravanFormation");
        var methods = AccessTools.GetDeclaredMethods(caravanFormationType);
        foreach (var method in methods)
        {
            if (method.Name != "TryFindExitSpot")
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 3)
            {
                continue;
            }

            if (!parameters[2].ParameterType.IsByRef || parameters[2].ParameterType.GetElementType() != typeof(IntVec3))
            {
                continue;
            }

            yield return method;
        }
    }

    public static void Postfix(ref IntVec3 spot, ref bool __result)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Colonists)
        {
            return;
        }

        var validVehicleExitSpotMethod =
            AccessTools.Method("Vehicles.World.Dialog_FormVehicleCaravan:ValidVehicleExitSpot");
        var vehiclePawnType = AccessTools.TypeByName("Vehicles.VehiclePawn");
        var formationField = AccessTools.Field("Vehicles.World.CaravanFormation:formation");
        var mapField = AccessTools.Field("Vehicles.World.FormationInfo:map");
        var vehiclesField = AccessTools.Field("Vehicles.World.FormationInfo:vehicles");
        if (validVehicleExitSpotMethod == null || vehiclePawnType == null)
        {
            return;
        }

        var formation = formationField.GetValue(null);
        caravanVehicles = ((IEnumerable)vehiclesField.GetValue(formation))
            .Cast<object>()
            .ToList();
        var map = (Map)mapField.GetValue(formation);

        var exitLocation = Main.FindBestExitSpot(map);

        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        if (caravanVehicles.Count == 0)
        {
            return;
        }

        if (validator(exitLocation))
        {
            spot = exitLocation;
            __result = true;
            return;
        }

        if (!CellFinder.TryFindRandomCellNear(exitLocation, map, 10, validator, out exitLocation))
        {
            Log.Message(
                "[EnterHere]: Could not find a valid exit spot for the caravan vehicles. Using the original exit location.");
            return;
        }

        spot = exitLocation;
        __result = true;

        return;

        bool validator(IntVec3 intVec3)
        {
            foreach (var caravanVehicle in caravanVehicles)
            {
                if ((bool)validVehicleExitSpotMethod.Invoke(null, [intVec3, caravanVehicle, map]))
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}