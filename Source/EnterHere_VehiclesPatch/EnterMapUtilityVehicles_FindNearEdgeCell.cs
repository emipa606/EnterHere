using EnterHere;
using HarmonyLib;
using Vehicles;
using Vehicles.World;
using Verse;

namespace EnterHere_VehiclesPatch;

[HarmonyPatch(typeof(EnterMapUtilityVehicles), "FindNearEdgeCell")]
public static class EnterMapUtilityVehicles_FindNearEdgeCell
{
    private static VehicleDef currentVehicleDef;
    private static Map currentMap;

    public static void Postfix(ref IntVec3 __result, Map map, VehicleDef vehicleDef)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Colonists)
        {
            return;
        }

        if (vehicleDef.type != VehicleType.Land)
        {
            return;
        }

        currentVehicleDef = vehicleDef;
        currentMap = map;

        var enterLocations = Main.FindAllEnterSpots(map);
        if (enterLocations == null)
        {
            return;
        }

        foreach (var location in enterLocations)
        {
            if (!CellFinder.TryFindRandomCellNear(location.Position, map, 10, extraValidator,
                    out var exitLocation))
            {
                continue;
            }

            __result = exitLocation;
            return;
        }

        return;

        static bool extraValidator(IntVec3 position)
        {
            return Main.CellValidator(position, currentMap, position) &&
                   currentVehicleDef?.CellRectStandable(currentMap, position, Rot4.Random) == true;
        }
    }
}