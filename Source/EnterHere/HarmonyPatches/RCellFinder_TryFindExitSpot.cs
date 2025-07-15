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
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return typeof(RCellFinder).GetMethod(nameof(RCellFinder.TryFindBestExitSpot));
        yield return typeof(RCellFinder).GetMethod(nameof(RCellFinder.TryFindRandomExitSpot));
    }

    public static bool Prefix(Pawn pawn, ref IntVec3 spot, TraverseMode mode, ref bool __result)
    {
        if (pawn.MentalStateDef == MentalStateDefOf.PanicFlee)
        {
            return true;
        }

        if (pawn.Faction.HostileTo(Faction.OfPlayerSilentFail) && !EnterHereMod.Instance.EnterHereSettings.EnemyRaids)
        {
            return true;
        }

        switch (pawn.IsColonist)
        {
            case true when !EnterHereMod.Instance.EnterHereSettings.Colonists:
                return true;
            case false:
            {
                var pawnLord = pawn.GetLord();
                switch (pawnLord?.LordJob)
                {
                    case LordJob_TravelAndExit:
                    case LordJob_VisitColony when !EnterHereMod.Instance.EnterHereSettings.Visitors:
                    case LordJob_TradeWithColony when !EnterHereMod.Instance.EnterHereSettings.Traders:
                    case LordJob_AssistColony when !EnterHereMod.Instance.EnterHereSettings.FriendlyRaids:
                        return true;
                }

                break;
            }
        }

        var exitLocation = Main.FindBestExitSpot(pawn, mode);

        if (exitLocation == IntVec3.Invalid)
        {
            return true;
        }

        spot = exitLocation;
        __result = true;
        return false;
    }
}