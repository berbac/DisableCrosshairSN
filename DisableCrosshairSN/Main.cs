using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;

namespace DisableCrosshairSN
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "DisableCrosshairSN");
            CrosshairMenu.Patch();
        }

    }
}
