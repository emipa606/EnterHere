using HarmonyLib;
using RimWorld;
using Verse;

namespace EnterHere;

[HarmonyPatch(typeof(LordToil_PrepareCaravan_Leave), nameof(LordToil_PrepareCaravan_Leave.UpdateAllDuties))]
public static class LordToil_PrepareCaravan_Leave_UpdateAllDuties
{
    public static void Prefix(ref IntVec3 ___exitSpot, LordToil_PrepareCaravan_Leave __instance)
    {
        if (!EnterHereMod.instance.EnterHereSettings.Colonists)
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