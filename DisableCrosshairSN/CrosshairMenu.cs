using HarmonyLib;
using SMLHelper.V2.Json;

namespace DisableCrosshairSN
{
    public class CrosshairOptions : ConfigFile
    { // these are the default values
        public bool NoCrosshairInSeaMoth = false; 
        public bool NoCrosshairInPrawnSuit = false;
        public bool DisableCrosshair = false;
    }
    public static class CrosshairMenu
    {
        public static CrosshairOptions Config { get; } = new CrosshairOptions();
        private static Harmony _harmony;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), "Update")]
        public static void Patch()
        {
            Config.Load(); // load crosshair config in config.json
            _harmony = new Harmony("com.berbac.subnautica.disablecrosshair.mod");
            _harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddGeneralTab", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("AddGerneralTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(GameSettings), "SerializeSettings", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("SerializeSettings_Postfix")), null);
        }

        public static void AddGerneralTab_Postfix(uGUI_OptionsPanel __instance)
        {
            __instance.AddHeading(0, "Crosshair");
            __instance.AddToggleOption(0, "Disable crosshair in sea moth", Config.NoCrosshairInSeaMoth, (bool v) => Config.NoCrosshairInSeaMoth = v);
            __instance.AddToggleOption(0, "Disable crosshair in prawn suit", Config.NoCrosshairInPrawnSuit, (bool v) => Config.NoCrosshairInPrawnSuit = v);
            __instance.AddToggleOption(0, "Disable crosshair completely", Config.DisableCrosshair, (bool v) => Config.DisableCrosshair = v);
        }

        public static void SerializeSettings_Postfix(GameSettings.ISerializer serializer)
        {
            Config.Save(); // save crosshair config in config.json
        }

    }

}
