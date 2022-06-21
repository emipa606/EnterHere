using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace EnterHere;

[StaticConstructorOnStartup]
public static class Main
{
    internal static readonly List<Type> targets;

    static Main()
    {
        targets = new List<Type> { typeof(IncidentWorker_VisitorGroup) };

        if (ModLister.GetActiveModWithIdentifier("Orion.Hospitality") != null)
        {
            var foundType = AccessTools.TypeByName("Hospitality.IncidentWorker_VisitorGroup");
            if (foundType != null)
            {
                targets.Add(foundType);

                Log.Message("[EnterHere]: Hospitality loaded, patching its IncidentWorker_VisitorGroup.");
            }
        }

        new Harmony("Mlie.EnterHere").PatchAll(Assembly.GetExecutingAssembly());
    }

    public static IntVec3 FindBestEnterSpot(Map map, IntVec3 startIntVec3)
    {
        var list = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("EnterHereSpot"));

        if (!list.Any())
        {
            return startIntVec3;
        }

        var currentSpotPosition = list.RandomElement().Position;

        bool CellValidator(IntVec3 edgeCell)
        {
            if (!edgeCell.Standable(map))
            {
                return false;
            }

            if (map.roofGrid.Roofed(edgeCell))
            {
                return false;
            }

            if (!map.reachability.CanReachColony(edgeCell))
            {
                return false;
            }

            if (!map.reachability.CanReach(edgeCell, currentSpotPosition, PathEndMode.OnCell,
                    TraverseParms.For(TraverseMode.PassDoors)))
            {
                return false;
            }

            if (!edgeCell.GetDistrict(map).TouchesMapEdge)
            {
                return false;
            }

            if (edgeCell.Fogged(map))
            {
                return false;
            }

            return true;
        }

        if (CellFinder.TryFindRandomEdgeCellNearWith(currentSpotPosition, 10f, map, CellValidator,
                out var resultIntVec3))
        {
            return resultIntVec3;
        }

        Log.Message(
            $"[EnterHere]: Could not find a suitable edge-cell near Enter-spot at {currentSpotPosition}, defaulting to vanilla enter behaviour");
        return startIntVec3;
    }
}