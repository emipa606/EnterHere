using Mlie;
using UnityEngine;
using Verse;

namespace EnterHere;

[StaticConstructorOnStartup]
public class EnterHereMod : Mod
{
    /// <summary>
    ///     The instance of the enterHereSettings to be read by the mod
    /// </summary>
    public static EnterHereMod Instance;

    private static string currentVersion;

    /// <summary>
    ///     The private enterHereSettings
    /// </summary>
    public readonly EnterHereSettings EnterHereSettings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public EnterHereMod(ModContentPack content) : base(content)
    {
        Instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        EnterHereSettings = GetSettings<EnterHereSettings>();
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
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);
        listingStandard.Gap();
        listingStandard.Label("EH.Title".Translate());
        listingStandard.CheckboxLabeled("EH.Traders".Translate(), ref EnterHereSettings.Traders,
            "EH.Traders.Tooltip".Translate());
        listingStandard.CheckboxLabeled("EH.Visitors".Translate(), ref EnterHereSettings.Visitors,
            "EH.Visitors.Tooltip".Translate());
        listingStandard.CheckboxLabeled("EH.Travelers".Translate(), ref EnterHereSettings.Travelers,
            "EH.Travelers.Tooltip".Translate());
        listingStandard.CheckboxLabeled("EH.Colonists".Translate(), ref EnterHereSettings.Colonists,
            "EH.Colonists.Tooltip".Translate());
        listingStandard.CheckboxLabeled("EH.FriendlyRaids".Translate(), ref EnterHereSettings.FriendlyRaids,
            "EH.FriendlyRaids.Tooltip".Translate());
        listingStandard.CheckboxLabeled("EH.EnemyRaids".Translate(), ref EnterHereSettings.EnemyRaids,
            "EH.EnemyRaids.Tooltip".Translate());
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("EH.CurrentVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }
}