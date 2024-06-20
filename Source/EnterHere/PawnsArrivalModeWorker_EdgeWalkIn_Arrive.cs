using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkIn), nameof(PawnsArrivalModeWorker_EdgeWalkIn.Arrive))]
public static class PawnsArrivalModeWorker_EdgeWalkIn_Arrive
{
    public static void Prefix(ref IncidentParms parms)
    {
        if (!EnterHereMod.instance.EnterHereSettings.EnemyRaids || !ModsConfig.AnomalyActive ||
            parms.faction != Faction.OfEntities)
        {
            return;
        }

        parms.spawnCenter = Main.FindBestEnterSpot((Map)parms.target, parms.spawnCenter);
    }
}