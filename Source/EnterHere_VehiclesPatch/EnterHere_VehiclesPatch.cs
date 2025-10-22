using System.Reflection;
using HarmonyLib;
using Verse;

namespace EnterHere_VehiclesPatch;

[StaticConstructorOnStartup]
public class EnterHere_VehiclesPatch
{
    static EnterHere_VehiclesPatch()
    {
        new Harmony("Mlie.EnterHere_VehiclesPatch").PatchAll(Assembly.GetExecutingAssembly());
        Log.Message("[EnterHere]: Added compatibility with Vehicle Framework");
    }
}