using System.Reflection;
using HarmonyLib;
using Verse;

namespace EnterHere_HospitalityPatch;

[StaticConstructorOnStartup]
public class EnterHere_HospitalityPatch
{
    static EnterHere_HospitalityPatch()
    {
        new Harmony("Mlie.EnterHere_HospitalityPatch").PatchAll(Assembly.GetExecutingAssembly());
    }
}