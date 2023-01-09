using HarmonyLib;
using UnityEngine;
using System;
using System.Timers;
using System.IO;

namespace DisableCrosshairSN
{
    [HarmonyPatch(typeof(GUIHand))]
    public static class CrosshairPatcher
    {
        private static bool ShouldHideCrosshair;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality(Clone)", "SeaTruckSleeperModule(Clone)", "Jukebox(Clone)" }; // special cases to show crosshair
        internal static string techType;
        internal static bool textHand;
        //internal static int logTime = DateTime.Now.Second;
        internal static int hideCount;
        internal static bool isPilotingCyclops;
        internal static DateTime cyclopsMotorButtonTimer;
        internal static double cyclopsMotorButtonShowCrosshairSeconds = 2d;

        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.UpdateText))]
        [HarmonyPrefix]
        public static void SetTextRaw_Prefix(string ___textHand, string ___textHandSubscript, ref int ___hideCount)
        {
            textHand = !string.IsNullOrEmpty(___textHand + ___textHandSubscript);
            hideCount = ___hideCount;
        }

        [HarmonyPatch(nameof(GUIHand.OnUpdate))]
        [HarmonyPostfix]
        public static void OnUpdate_Postfix(GUIHand __instance, GUIHand.GrabMode ___grabMode)
        {
            //TODO: neccessary?
            if (Player.main == null || cyclopsMotorButtonTimer > DateTime.Now) // skip block if no Player.main instance exists or if cyclopsmotorbutton was mousovered
            {
                return;
            }

            // getting techType for map room screen, jukebox etc.
            // check if CH needs to be enabled for interaction while on foot/swimming
            // try-catch needed -> throws error if no target in range

            try
            {
                Targeting.GetTarget(Player.main.gameObject, 10, out GameObject getTarget, out float _);
                CraftData.GetTechType(getTarget, out var _techType);
                techType = _techType.name;
            }
            catch (NullReferenceException)
            {
                techType = null;
            }

            Player.Mode playerMode = Player.main.GetMode();
            bool isNormalOrSitting = playerMode == Player.Mode.Normal || playerMode == Player.Mode.Sitting;
            bool targetNeedsCrosshair = (__instance.GetActiveTarget() && playerMode == Player.Mode.Normal && !Player.main.cinematicModeActive) || 
                textHand ||
                (Player.main.IsInsideWalkable() && Array.Exists(showCrosshairWhilePointingAt, element => element == techType));
            //grabmode = __instance.grabMode.ToString();

            //if (DateTime.Now.Second != logTime)
            //{
            //    File.AppendAllText("DisableCrosshair_Log.txt",
            //        "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
            //        "\n textHand: " + textHand +
            //        "\n playerMode: " + playerMode +
            //        "\n techtype: " + techType +
            //        "\n grabMode: " + ___grabMode +
            //        "\n ActiveTarget: " + __instance.GetActiveTarget() +
            //        "\n hideCount: " + hideCount +
            //        "\n ShouldHideCrosshair: " + ShouldHideCrosshair +
            //        "\n _cinematicModeActive: " + Player.main.cinematicModeActive +
            //        "\n MousOverMotorButton: " + isMouseOverCyclopsMotorModeButton +
            //        "\n" + DateTime.Now +
            //        "\n____________________________________");
            //    logTime = DateTime.Now.Second;
            //}
            //isMouseOverCyclopsMotorModeButton = false;
            if (hideCount > 0) // crosshair is currently off
            {
                if (((!CrosshairOptions.NoCrosshairOnFoot || targetNeedsCrosshair) && isNormalOrSitting) ||
                   ((!CrosshairOptions.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairOptions.NoCrosshairInSeaMoth && Player.main.inSeamoth) ||
                   ((!CrosshairOptions.NoCrosshairPilotingCylops || targetNeedsCrosshair) && isPilotingCyclops))
                {
                    ShouldHideCrosshair = false;
                    return;
                }
                else return;
            }

            else // Crosshair is currently on
            {
                if ((CrosshairOptions.NoCrosshairOnFoot && isNormalOrSitting && !targetNeedsCrosshair) ||
                   (CrosshairOptions.NoCrosshairInSeaMoth && Player.main.inSeamoth) ||
                   (CrosshairOptions.NoCrosshairInPrawnSuit && Player.main.inExosuit && !targetNeedsCrosshair) ||
                   (CrosshairOptions.NoCrosshairPilotingCylops && isPilotingCyclops && !targetNeedsCrosshair))
                {
                    ShouldHideCrosshair = true;
                    return;
                }
                else return;
            }
        }

        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.UpdateText))]
        [HarmonyPostfix]
        public static void SetHideCount(ref int ___hideCount)
        {
            // this enables and disables the crosshair
            if (ShouldHideCrosshair)
            {
                ___hideCount = 1; // hide
            }
            else
            {
                ___hideCount = 0; //show
            }
        }

        [HarmonyPatch(typeof(CyclopsHelmHUDManager),nameof(CyclopsHelmHUDManager.Update))]
        [HarmonyPostfix]
        public static void GetPlayerPilotingCyclops(ref bool ___hudActive)
        {
            // this checks for the cyclops hud meaning if true the player is piloting it
            isPilotingCyclops = ___hudActive;
        }

        [HarmonyPatch(typeof(CyclopsMotorModeButton), nameof(CyclopsMotorModeButton.OnMouseOver))]
        [HarmonyPrefix]
        public static bool GetCyclopsMotorModeButtonMouseOver()
        {
            // this enables the crosshair for mouseover on motorbutton in cyclops for a given timespan
            if (isPilotingCyclops)
            {
                ShouldHideCrosshair = false;
                cyclopsMotorButtonTimer = DateTime.Now.AddSeconds(cyclopsMotorButtonShowCrosshairSeconds);
            }
            return true;
        }
    }
}

