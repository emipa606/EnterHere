using Verse;

namespace EnterHere;

public class PlaceWorker_NearEdge : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        if (loc.DistanceToEdge(map) > 10)
        {
            return new AcceptanceReport("EH.MustPlaceNearEdge".Translate());
        }

        return true;
    }
}