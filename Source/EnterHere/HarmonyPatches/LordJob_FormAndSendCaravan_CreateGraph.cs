using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch(typeof(LordJob_FormAndSendCaravan), nameof(LordJob_FormAndSendCaravan.CreateGraph))]
public static class LordJob_FormAndSendCaravan_CreateGraph
{
    public static void Prefix(ref IntVec3 ___exitSpot, LordJob_FormAndSendCaravan __instance)
    {
        if (!EnterHereMod.Instance.EnterHereSettings.Colonists)
        {
            return;
        }

        var searcher = __instance.lord.ownedPawns.RandomElement();
        var exitLocation = Main.FindBestExitSpot(searcher, TraverseMode.ByPawn, true);

        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        ___exitSpot = exitLocation;
    }
}