using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker", typeof(IncidentParms))]
public static class IncidentWorker_TraderCaravanArrival_TryExecuteWorker
{
    [HarmonyPriority(Priority.Low)]
    public static void Prefix(ref IncidentParms parms)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Traders)
        {
            return;
        }

        parms.spawnCenter = Main.FindBestEnterSpot((Map)parms.target, parms.spawnCenter);
    }
}