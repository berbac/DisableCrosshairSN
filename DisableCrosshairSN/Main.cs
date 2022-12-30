using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;

namespace DisableCrosshairSN
{
    [BepInPlugin(myGUID, modName, versionString)]
    //[BepInProcess("Subnautica.exe")]

    public class DisableCrosshairSN : BaseUnityPlugin
    {
        private const string myGUID = "com.berbac.subnautica.disablecrosshair.mod";
        private const string modName = "DisableCrosshairSN";
        private const string versionString = "1.4.0";

        //public static ConfigEntry<bool> ConfigNoCrosshairInSeaMoth;
        //public static ConfigEntry<bool> ConfigNoCrosshairInPrawnSuit;
        //public static ConfigEntry<bool> ConfigNoCrosshairOnFoot;

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        public void Awake()
        {
            //ConfigNoCrosshairInSeaMoth = Config.Bind("General",
            //    "NoCrosshairInSeaMoth",
            //    false,
            //    "Disable Crosshair in Seamoth");
            //ConfigNoCrosshairInPrawnSuit = Config.Bind("General",
            //    "NoCrosshairInPrawnSuit",
            //    false,
            //    "Disable Crosshair in Prawn Suit");
            //ConfigNoCrosshairOnFoot = Config.Bind("General",
            //    "NoCrosshairOnFoot",
            //    false,
            //    "Disable Crosshair while swimming/on foot");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo(modName + " " + versionString + " " + "loaded.");
            logger = Logger;
            //Harmony.CreateAndPatchAll(typeof(CrosshairPatcher), "com.berbac.subnautica.disablecrosshair.mod");
            CrosshairMenu.Patch();
            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "DisableCrosshairSN");

        }
    }
}
