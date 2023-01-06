using HarmonyLib;
using UnityEngine;
using System;
using System.IO;

namespace DisableCrosshairSN
{
    //[HarmonyPatch]
    [HarmonyPatch(typeof(GUIHand))]
    public static class CrosshairPatcher
    {
        private static bool ShouldHideCrosshair;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality(Clone)", "SeaTruckSleeperModule(Clone)", "Jukebox(Clone)" }; // special cases to show crosshair
        internal static string techType;
        internal static bool textHand;
        //internal static bool shieldButtonHover;
        //internal static bool cyclopseEngineChangeStateMouseHover;
        //internal static int logTime = DateTime.Now.Second;
        internal static int hideCount;

        //[HarmonyPatch(typeof(CyclopsEngineChangeState), nameof(CyclopsEngineChangeState.Update))]
        //[HarmonyPrefix]
        //public static void CycopseGetEngineChangeStateHover(bool ___mouseHover)
        //{
        //    cyclopseEngineChangeStateMouseHover = ___mouseHover;
        //}

        //[HarmonyPatch(typeof(CyclopsShieldButton), nameof(CyclopsShieldButton.Update))]
        //[HarmonyPrefix]
        //public static void CyclpseGetShieldButton(bool ___mouseHover)
        //{
        //    shieldButtonHover = ___mouseHover;
        //}

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
            if (Player.main == null) // skip block if no Player.main instance exists
                return;

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
            //        //"\n CyclopseShieldButtonMouseHover: " + shieldButtonHover +
            //        //"\n cyclopseEngineChangeStateMouseHover: " + cyclopseEngineChangeStateMouseHover +
            //        "\n" + DateTime.Now +
            //        "\n____________________________________");
            //    logTime = DateTime.Now.Second;
            //}

            if (hideCount > 0) // crosshair is currently off
            {
                if (((!CrosshairOptions.NoCrosshairOnFoot || targetNeedsCrosshair) && isNormalOrSitting) ||
                   ((!CrosshairOptions.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairOptions.NoCrosshairInSeaMoth && Player.main.inSeamoth))
                {
                    //HandReticle.main.UnrequestCrosshairHide();
                    ShouldHideCrosshair = false;
                    return;
                }
                else return;
            }

            else // Crosshair is currently on
            {
                if ((CrosshairOptions.NoCrosshairOnFoot && isNormalOrSitting && !targetNeedsCrosshair) ||
                   (CrosshairOptions.NoCrosshairInSeaMoth && Player.main.inSeamoth) ||
                   (CrosshairOptions.NoCrosshairInPrawnSuit && Player.main.inExosuit && !targetNeedsCrosshair))
                {
                    //HandReticle.main.RequestCrosshairHide();
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
            if (ShouldHideCrosshair)
            {
                ___hideCount = 1;
            }
            else
            {
                ___hideCount = 0;
            }
        }
    }
}

