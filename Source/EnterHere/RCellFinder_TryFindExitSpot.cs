using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace EnterHere;

[HarmonyPatch]
public static class RCellFinder_TryFindExitSpot
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return typeof(RCellFinder).GetMethod("TryFindBestExitSpot");
        yield return typeof(RCellFinder).GetMethod("TryFindRandomExitSpot");
    }

    public static bool Prefix(Pawn pawn, ref IntVec3 spot, TraverseMode mode, ref bool __result)
    {
        if (pawn.MentalStateDef == MentalStateDefOf.PanicFlee)
        {
            return true;
        }

        if (pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) && !EnterHereMod.instance.EnterHereSettings.EnemyRaids)
        {
            return true;
        }

        if (pawn.IsColonist && !EnterHereMod.instance.EnterHereSettings.Colonists)
        {
            return true;
        }

        if (!pawn.IsColonist)
        {
            var pawnLord = pawn.GetLord();
            switch (pawnLord?.LordJob)
            {
                case LordJob_TravelAndExit:
                case LordJob_VisitColony when !EnterHereMod.instance.EnterHereSettings.Visitors:
                case LordJob_TradeWithColony when !EnterHereMod.instance.EnterHereSettings.Traders:
                case LordJob_AssistColony when !EnterHereMod.instance.EnterHereSettings.FriendlyRaids:
                    return true;
            }
        }

        var exitLocation = Main.FindBestExitSpot(pawn, pawn.Position, mode);

        if (exitLocation == IntVec3.Invalid)
        {
            return true;
        }

        spot = exitLocation;
        __result = true;
        return false;
    }
}