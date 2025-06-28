using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch]
public static class IncidentWorker_VisitorGroup_TryExecuteWorker
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (var visitorTypes in Main.Targets)
        {
            yield return AccessTools.Method(visitorTypes, "TryExecuteWorker");
        }
    }

    [HarmonyPriority(Priority.Low)]
    public static void Prefix(ref IncidentParms parms)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Visitors)
        {
            return;
        }

        parms.spawnCenter = Main.FindBestEnterSpot((Map)parms.target, parms.spawnCenter);
    }
}