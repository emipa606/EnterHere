using Verse;

namespace EnterHere;

/// <summary>
///     Definition of the enterHereSettings for the mod
/// </summary>
internal class EnterHereSettings : ModSettings
{
    public bool Colonists = true;
    public bool Traders = true;
    public bool Travelers = true;
    public bool Visitors = true;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Visitors, "Visitors", true);
        Scribe_Values.Look(ref Traders, "Traders", true);
        Scribe_Values.Look(ref Travelers, "Travelers", true);
        Scribe_Values.Look(ref Colonists, "Colonists", true);
    }
}