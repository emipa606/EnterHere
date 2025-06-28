using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch(typeof(IncidentWorker_TravelerGroup), "TryExecuteWorker", typeof(IncidentParms))]
public static class IncidentWorker_TravelerGroup_TryExecuteWorker
{
    [HarmonyPriority(Priority.Low)]
    public static void Prefix(ref IncidentParms parms)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Travelers)
        {
            return;
        }

        parms.spawnCenter = Main.FindBestEnterSpot((Map)parms.target, parms.spawnCenter);
    }
}