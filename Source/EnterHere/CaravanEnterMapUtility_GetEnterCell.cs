using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace EnterHere;

[HarmonyPatch(typeof(CaravanEnterMapUtility), "GetEnterCell")]
public static class CaravanEnterMapUtility_GetEnterCell
{
    public static bool Prefix(Caravan caravan, Map map, ref IntVec3 __result)
    {
        if (!EnterHereMod.instance.EnterHereSettings.Colonists)
        {
            return true;
        }

        if (!caravan.IsPlayerControlled || map.ParentFaction != Faction.OfPlayer)
        {
            return true;
        }

        var spawnLocation = Main.FindBestEnterSpot(map, IntVec3.Invalid);

        if (spawnLocation == IntVec3.Invalid)
        {
            return true;
        }

        __result = spawnLocation;
        return false;
    }
}