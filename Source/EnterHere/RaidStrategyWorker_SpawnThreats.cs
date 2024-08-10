using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class RaidStrategyWorker_SpawnThreats
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(RaidStrategyWorker), nameof(RaidStrategyWorker.SpawnThreats));

        foreach (var subclass in typeof(RaidStrategyWorker).AllSubclasses())
        {
            var subMethod = AccessTools.DeclaredMethod(subclass, "SpawnThreats");
            if (subMethod == null)
            {
                continue;
            }

            yield return subMethod;
        }
    }

    [HarmonyPriority(Priority.Low)]
    public static void Prefix(ref IncidentParms parms)
    {
        if (parms.raidArrivalMode != PawnsArrivalModeDefOf.EdgeWalkIn)
        {
            return;
        }

        if (EnterHereMod.instance.EnterHereSettings.FriendlyRaids &&
            parms.faction?.AllyOrNeutralTo(Faction.OfPlayerSilentFail) == true ||
            EnterHereMod.instance.EnterHereSettings.EnemyRaids &&
            parms.faction?.HostileTo(Faction.OfPlayerSilentFail) == true)
        {
            parms.spawnCenter = Main.FindBestEnterSpot((Map)parms.target, parms.spawnCenter);
        }
    }
}