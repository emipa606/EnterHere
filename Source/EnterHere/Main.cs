using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace EnterHere;

[StaticConstructorOnStartup]
public static class Main
{
    internal static readonly List<Type> targets;

    public static readonly Texture2D EnterOnlyTexture2D = ContentFinder<Texture2D>.Get("UI/EnterOnly");

    public static readonly Texture2D ExitOnlyTexture2D = ContentFinder<Texture2D>.Get("UI/ExitOnly");

    public static readonly Texture2D EnterAndExitTexture2D = ContentFinder<Texture2D>.Get("UI/EnterAndExit");

    private static readonly Dictionary<Pawn, Tuple<int, IntVec3>> ExitSpotCache =
        new Dictionary<Pawn, Tuple<int, IntVec3>>();

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
        var list = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("EnterHereSpot"))?.Where(building =>
            building is EnterSpot
            {
                IsEnterance: true
            });

        if (list == null || !list.Any())
        {
            return startIntVec3;
        }

        foreach (var spot in list.InRandomOrder())
        {
            var currentSpotPosition = spot.Position;

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

                return !edgeCell.Fogged(map);
            }

            if (CellFinder.TryFindRandomEdgeCellNearWith(currentSpotPosition, 10f, map, CellValidator,
                    out var resultIntVec3))
            {
                return resultIntVec3;
            }
        }

        Log.Message(
            "[EnterHere]: Could not find a suitable edge-cell near an enter spot, defaulting to vanilla enter behaviour");
        return startIntVec3;
    }


    public static IntVec3 FindBestExitSpot(Pawn pawn, IntVec3 startingPoint, TraverseMode mode)
    {
        var cachedSpot = checkExitCache(pawn);

        if (cachedSpot != IntVec3.Invalid)
        {
            return cachedSpot;
        }

        var map = pawn.Map;
        var list = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("EnterHereSpot"))
            ?.Where(building => building is EnterSpot { IsExit: true });

        if (list == null || !list.Any())
        {
            return IntVec3.Invalid;
        }

        foreach (var spot in list.InRandomOrder())
        {
            var currentSpotPosition = spot.Position;

            bool CellValidator(IntVec3 edgeCell)
            {
                if (!edgeCell.Standable(map))
                {
                    return false;
                }

                if (edgeCell.Fogged(map))
                {
                    return false;
                }

                if (map.roofGrid.Roofed(edgeCell))
                {
                    return false;
                }

                return edgeCell.GetDistrict(map).TouchesMapEdge &&
                       pawn.CanReach(edgeCell, PathEndMode.OnCell, Danger.Deadly);
            }

            if (!CellFinder.TryFindRandomEdgeCellNearWith(currentSpotPosition, 10f, map, CellValidator,
                    out var resultIntVec3))
            {
                continue;
            }

            ExitSpotCache[pawn] = new Tuple<int, IntVec3>(GenTicks.TicksGame, resultIntVec3);
            return resultIntVec3;
        }

        Log.Message(
            "[EnterHere]: Could not find a suitable edge-cell near an exit spot, defaulting to vanilla exit behaviour");
        return IntVec3.Invalid;
    }

    private static IntVec3 checkExitCache(Pawn pawn)
    {
        if (!ExitSpotCache.ContainsKey(pawn))
        {
            return IntVec3.Invalid;
        }

        var cachedSpot = ExitSpotCache[pawn];
        if (GenTicks.TickRareInterval * 2 < GenTicks.TicksGame - cachedSpot.Item1)
        {
            ExitSpotCache.Remove(pawn);
            return IntVec3.Invalid;
        }

        ExitSpotCache[pawn] = new Tuple<int, IntVec3>(GenTicks.TicksGame, cachedSpot.Item2);
        return ExitSpotCache[pawn].Item2;
    }
}