using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnterHere;
using HarmonyLib;
using Vehicles;
using Vehicles.World;
using Verse;

namespace EnterHere_VehiclesPatch;

[HarmonyPatch(typeof(CaravanFormation), "TryFindExitSpot", [typeof(List<Pawn>), typeof(bool), typeof(IntVec3)],
    [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out])]
public static class CaravanFormation_TryFindExitSpot
{
    private static List<VehiclePawn> caravanVehicles = [];
    private static Map caravanMap;
    private static readonly Func<IntVec3, VehiclePawn, Map, bool> ValidVehicleExitSpot;

    static CaravanFormation_TryFindExitSpot()
    {
        // Try direct fully-qualified name first
        ValidVehicleExitSpot = createDelegate();
        if (ValidVehicleExitSpot == null)
        {
            Log.Warning("[EnterHere]: Could not bind ValidVehicleExitSpot local function in CaravanFormation.");
        }
    }

    private static Func<IntVec3, VehiclePawn, Map, bool> createDelegate()
    {
        var cfType = typeof(CaravanFormation);
        // Compiler-generated local function method name pattern: <TryFindExitSpot>g__ValidVehicleExitSpot|<il offset>_<sequence>
        var method = AccessTools.Method(cfType, "<TryFindExitSpot>g__ValidVehicleExitSpot|11_2");
        if (method == null)
        {
            method = cfType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(m => m.Name.Contains("ValidVehicleExitSpot") && m.GetParameters().Length == 3);
        }

        if (method == null)
        {
            return null;
        }

        try
        {
            return (Func<IntVec3, VehiclePawn, Map, bool>)Delegate.CreateDelegate(
                typeof(Func<IntVec3, VehiclePawn, Map, bool>), method);
        }
        catch (Exception e)
        {
            Log.Warning("[EnterHere]: Failed creating delegate for ValidVehicleExitSpot: " + e.Message);
            return null;
        }
    }

    public static void Postfix(ref IntVec3 spot, ref bool __result)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Colonists)
        {
            return;
        }

        if (ValidVehicleExitSpot == null)
        {
            return; // Cannot validate without delegate
        }

        var formation = CaravanFormation.formation;
        if (formation == null)
        {
            return;
        }

        caravanVehicles = formation.vehicles;
        caravanMap = formation.Map;
        if (caravanMap == null)
        {
            return;
        }

        if (caravanVehicles.Count == 0)
        {
            return;
        }

        var exitLocations = Main.FindAllExitSpots(caravanMap);
        if (exitLocations == null)
        {
            return;
        }

        foreach (var location in exitLocations)
        {
            if (!CellFinder.TryFindRandomCellNear(location.Position, caravanMap, 10, combinedValidator,
                    out var exitLocation))
            {
                continue;
            }

            spot = exitLocation;
            __result = true;
            return;
        }

        return;

        static bool combinedValidator(IntVec3 c)
        {
            foreach (var caravanVehicle in caravanVehicles)
            {
                if (c.PadForHitbox(caravanMap, caravanVehicle.VehicleDef) != c)
                {
                    return false;
                }

                if (!ValidVehicleExitSpot(c, caravanVehicle, caravanMap))
                {
                    return false;
                }
            }

            return true;
        }
    }
}