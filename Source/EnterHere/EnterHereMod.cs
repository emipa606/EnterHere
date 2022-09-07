using UnityEngine;
using Verse;

namespace EnterHere;

[StaticConstructorOnStartup]
internal class EnterHereMod : Mod
{
    /// <summary>
    ///     The instance of the enterHereSettings to be read by the mod
    /// </summary>
    public static EnterHereMod instance;

    /// <summary>
    ///     The private enterHereSettings
    /// </summary>
    private EnterHereSettings enterHereSettings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public EnterHereMod(ModContentPack content) : base(content)
    {
        instance = this;
    }

    /// <summary>
    ///     The instance-enterHereSettings for the mod
    /// </summary>
    internal EnterHereSettings EnterHereSettings
    {
        get
        {
            if (enterHereSettings == null)
            {
                enterHereSettings = GetSettings<EnterHereSettings>();
            }

            return enterHereSettings;
        }
        set => enterHereSettings = value;
    }

    /// <summary>
    ///     The title for the mod-enterHereSettings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Enter Here";
    }

    /// <summary>
    ///     The enterHereSettings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        listing_Standard.Label("EH.Title".Translate());
        listing_Standard.CheckboxLabeled("EH.Traders".Translate(), ref EnterHereSettings.Traders,
            "EH.Traders.Tooltip".Translate());
        listing_Standard.CheckboxLabeled("EH.Visitors".Translate(), ref EnterHereSettings.Visitors,
            "EH.Visitors.Tooltip".Translate());
        listing_Standard.CheckboxLabeled("EH.Travelers".Translate(), ref EnterHereSettings.Travelers,
            "EH.Travelers.Tooltip".Translate());
        listing_Standard.CheckboxLabeled("EH.Colonists".Translate(), ref EnterHereSettings.Colonists,
            "EH.Colonists.Tooltip".Translate());
        listing_Standard.CheckboxLabeled("EH.FriendlyRaids".Translate(), ref EnterHereSettings.FriendlyRaids,
            "EH.FriendlyRaids.Tooltip".Translate());
        listing_Standard.CheckboxLabeled("EH.EnemyRaids".Translate(), ref EnterHereSettings.EnemyRaids,
            "EH.EnemyRaids.Tooltip".Translate());

        listing_Standard.End();
    }
}