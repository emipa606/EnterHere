using EnterHere;
using HarmonyLib;
using Hospitality;
using Verse;
using Verse.AI.Group;

namespace EnterHere_HospitalityPatch;

[HarmonyPatch(typeof(TransitionAction_EnsureHaveNearbyExitDestination), "DoAction")]
public static class TransitionAction_EnsureHaveNearbyExitDestination_DoAction
{
    public static void Postfix(ref Transition trans)
    {
        if (!EnterHereMod.instance.EnterHereSettings.Visitors)
        {
            return;
        }

        var target = (LordToil_Travel)trans.target;
        var searcher = target.lord.ownedPawns.RandomElement();

        var exitLocation = Main.FindBestExitSpot(searcher, TraverseMode.ByPawn, true);

        if (exitLocation == IntVec3.Invalid)
        {
            return;
        }

        target.SetDestination(exitLocation);
    }
}