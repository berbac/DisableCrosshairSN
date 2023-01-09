using HarmonyLib;
//using SMLHelper.V2.Json;

namespace DisableCrosshairSN
{
    //public class CrosshairOptions : ConfigFile
    //{ 
    //    // these are the default values
    //    public bool NoCrosshairInSeaMoth = true; 
    //    public bool NoCrosshairInPrawnSuit = true;
    //    public bool NoCrosshairOnFoot = true;
    //}
    [HarmonyPatch(typeof(uGUI_OptionsPanel))]
    public static class CrosshairMenu
    {
        //public static CrosshairOptions Config { get; } = new CrosshairOptions();
        private static Harmony _harmony;

        [HarmonyPatch(nameof(uGUI_OptionsPanel.Update))]
        public static void Patch()
        {
            //Config.Load(); // load crosshair config from config.json
            _harmony = new Harmony("com.berbac.subnautica.disablecrosshair.mod.menu");
            _harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddGeneralTab", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("AddGeneralTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(GameSettings), "SerializeSettings", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("SerializeSettings_Postfix")), null);
        }

        public static void AddGeneralTab_Postfix(uGUI_OptionsPanel __instance)
        {
            __instance.AddHeading(0, "Hide Crosshair");
            __instance.AddToggleOption(0, "In Seamoth", CrosshairOptions.NoCrosshairInSeaMoth, (bool v) => CrosshairOptions.NoCrosshairInSeaMoth = v);
            __instance.AddToggleOption(0, "In Prawn Suit", CrosshairOptions.NoCrosshairInPrawnSuit, (bool v) => CrosshairOptions.NoCrosshairInPrawnSuit = v);
            __instance.AddToggleOption(0, "While Walking/Swimming", CrosshairOptions.NoCrosshairOnFoot, (bool v) => CrosshairOptions.NoCrosshairOnFoot = v);
            __instance.AddToggleOption(0, "While piloting the Cyclops", CrosshairOptions.NoCrosshairPilotingCylops, (bool v) => CrosshairOptions.NoCrosshairPilotingCylops = v);
            __instance.AddToggleOption(0, "Disable control hints (e.g. Press B to exit)", CrosshairOptions.NoControlHints, (bool v) => CrosshairOptions.NoControlHints = v);
        }

        public static void SerializeSettings_Postfix(GameSettings.ISerializer serializer)
        {
            CrosshairOptions.NoCrosshairInSeaMoth = serializer.Serialize("NoCrosshairinSeaMoth", CrosshairOptions.NoCrosshairInSeaMoth);
            CrosshairOptions.NoCrosshairInPrawnSuit = serializer.Serialize("NoCrosshairInPrawnSuit", CrosshairOptions.NoCrosshairInPrawnSuit);
            CrosshairOptions.NoCrosshairOnFoot = serializer.Serialize("NoCrosshairOnFoot", CrosshairOptions.NoCrosshairOnFoot);
            CrosshairOptions.NoCrosshairPilotingCylops = serializer.Serialize("NoCrosshairPilotingCylops", CrosshairOptions.NoCrosshairPilotingCylops);
            CrosshairOptions.NoControlHints = serializer.Serialize("NoControlHints", CrosshairOptions.NoControlHints);
            //Config.Save(); // save crosshair config to config.json

        }
    }
}
