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
    internal static readonly List<Type> Targets;

    public static readonly Texture2D EnterOnlyTexture2D = ContentFinder<Texture2D>.Get("UI/EnterOnly");

    public static readonly Texture2D ExitOnlyTexture2D = ContentFinder<Texture2D>.Get("UI/ExitOnly");

    public static readonly Texture2D EnterAndExitTexture2D = ContentFinder<Texture2D>.Get("UI/EnterAndExit");

    private static readonly Dictionary<Pawn, Tuple<int, IntVec3>> ExitSpotCache = new();

    static Main()
    {
        Targets = [typeof(IncidentWorker_VisitorGroup)];

        if (ModLister.GetActiveModWithIdentifier("Orion.Hospitality", true) != null)
        {
            var foundType = AccessTools.TypeByName("Hospitality.IncidentWorker_VisitorGroup");
            if (foundType != null)
            {
                Targets.Add(foundType);

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
                IsEntrance: true
            }).ToArray();

        if (list == null || !list.Any())
        {
            return startIntVec3;
        }

        foreach (var spot in list.InRandomOrder())
        {
            var currentSpotPosition = spot.Position;

            if (CellFinder.TryFindRandomEdgeCellNearWith(currentSpotPosition, 10f, map, cellValidator,
                    out var resultIntVec3))
            {
                return resultIntVec3;
            }

            continue;

            bool cellValidator(IntVec3 edgeCell)
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
        }

        Log.Message(
            "[EnterHere]: Could not find a suitable edge-cell near an enter spot, defaulting to vanilla enter behaviour");
        return startIntVec3;
    }


    public static IntVec3 FindBestExitSpot(Map map)
    {
        var list = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("EnterHereSpot"))
            ?.Where(building => building is EnterSpot { IsExit: true }).ToArray();

        if (list == null || !list.Any())
        {
            return IntVec3.Invalid;
        }

        return list.InRandomOrder().Select(spot =>
        {
            var currentSpotPosition = spot.Position;

            return CellFinder.TryFindRandomEdgeCellNearWith(currentSpotPosition, 10f, map, cellValidator,
                out var resultIntVec3)
                ? resultIntVec3
                : IntVec3.Invalid;

            bool cellValidator(IntVec3 edgeCell)
            {
                if (!edgeCell.Standable(map))
                {
                    return false;
                }

                if (edgeCell.Fogged(map))
                {
                    return false;
                }

                return !map.roofGrid.Roofed(edgeCell) && edgeCell.GetDistrict(map).TouchesMapEdge;
            }
        }).FirstOrDefault(cell => cell != IntVec3.Invalid);
    }

    public static IntVec3 FindBestExitSpot(Pawn pawn, TraverseMode mode, bool guests = false)
    {
        var cachedSpot = checkExitCache(pawn);

        if (cachedSpot != IntVec3.Invalid)
        {
            return cachedSpot;
        }

        var map = pawn.Map;
        var list = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("EnterHereSpot"))
            ?.Where(building => building is EnterSpot { IsExit: true }).ToArray();

        if (list == null || !list.Any())
        {
            return IntVec3.Invalid;
        }

        Pawn alternatePawn = null;
        if (guests)
        {
            alternatePawn = map.mapPawns.FreeColonists.OrderBy(colonist => colonist.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
            if (alternatePawn == null)
            {
                return IntVec3.Invalid;
            }

            Log.Message(
                $"[EnterHere]: Hospitality guests, will use nearest colonist for pathfinding. ({alternatePawn})");
        }

        foreach (var spot in list.InRandomOrder())
        {
            var currentSpotPosition = spot.Position;

            if (!CellFinder.TryFindRandomEdgeCellNearWith(currentSpotPosition, 10f, map, cellValidator,
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

        bool cellValidator(IntVec3 edgeCell)
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

            if (!edgeCell.GetDistrict(map).TouchesMapEdge)
            {
                return false;
            }

            return alternatePawn?.CanReach(edgeCell, PathEndMode.OnCell, Danger.Deadly, mode: mode) ??
                   pawn.CanReach(edgeCell, PathEndMode.OnCell, Danger.Deadly, mode: mode);
        }
    }

    private static IntVec3 checkExitCache(Pawn pawn)
    {
        if (!ExitSpotCache.TryGetValue(pawn, out var cachedSpot))
        {
            return IntVec3.Invalid;
        }

        if (GenTicks.TickRareInterval * 2 < GenTicks.TicksGame - cachedSpot.Item1)
        {
            ExitSpotCache.Remove(pawn);
            return IntVec3.Invalid;
        }

        ExitSpotCache[pawn] = new Tuple<int, IntVec3>(GenTicks.TicksGame, cachedSpot.Item2);
        return ExitSpotCache[pawn].Item2;
    }
}