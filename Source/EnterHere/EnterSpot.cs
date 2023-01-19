using System.Collections.Generic;
using Verse;

namespace EnterHere;

public class EnterSpot : Building
{
    private int spotType;

    public bool IsExit => spotType != 0;
    public bool IsEnterance => spotType != 1;

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        var changeTypeAction = new Command_Action
        {
            action = delegate
            {
                spotType++;
                if (spotType == 3)
                {
                    spotType = 0;
                }
            }
        };
        switch (spotType)
        {
            case 0:
                changeTypeAction.icon = Main.EnterOnlyTexture2D;
                changeTypeAction.defaultLabel = "EH.EnterOnly".Translate();
                changeTypeAction.defaultDesc = "EH.EnterOnlyTT".Translate();
                break;
            case 1:
                changeTypeAction.icon = Main.ExitOnlyTexture2D;
                changeTypeAction.defaultLabel = "EH.ExitOnly".Translate();
                changeTypeAction.defaultDesc = "EH.ExitOnlyTT".Translate();
                break;
            case 2:
                changeTypeAction.icon = Main.EnterAndExitTexture2D;
                changeTypeAction.defaultLabel = "EH.EnterAndExit".Translate();
                changeTypeAction.defaultDesc = "EH.EnterAndExitTT".Translate();
                break;
        }

        yield return changeTypeAction;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref spotType, "spotType");
    }
}