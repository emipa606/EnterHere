using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class IncidentWorker_EntitySwarm_GenerateEntities
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        if (!ModsConfig.AnomalyActive)
        {
            yield return AccessTools.Method(typeof(IncidentWorker_EntitySwarm_GenerateEntities), "Dummy");
            yield break;
        }

        yield return AccessTools.Method(typeof(IncidentWorker_EntitySwarm), "GenerateEntities");
        foreach (var subclass in typeof(IncidentWorker_EntitySwarm).AllSubclasses())
        {
            var subMethod = AccessTools.Method(subclass, "GenerateEntities");
            if (subMethod == null)
            {
                continue;
            }

            yield return subMethod;
        }
    }

    [HarmonyPriority(Priority.Low)]
    public static void Postfix(ref IncidentParms parms)
    {
        if (!EnterHereMod.instance.EnterHereSettings.EnemyRaids)
        {
            return;
        }

        parms.spawnCenter = Main.FindBestEnterSpot((Map)parms.target, parms.spawnCenter);
    }

    private static void Dummy(IncidentParms parms)
    {
        // Dummy method to force Harmony to create a patch class
    }
}