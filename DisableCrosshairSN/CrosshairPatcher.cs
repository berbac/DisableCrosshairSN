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
        private static bool crosshairIsOff;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality(Clone)", "SeaTruckSleeperModule(Clone)", "Jukebox(Clone)" }; // special cases to show crosshair
        internal static string techType;
        internal static bool textHand;

        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.UpdateText))]
        [HarmonyPrefix]
        public static void SetTextRaw_Prefix(string ___textHand, string ___textHandSubscript)
        {
            textHand = !string.IsNullOrEmpty(___textHand + ___textHandSubscript);
        }

        [HarmonyPatch(nameof(GUIHand.OnUpdate))]
        [HarmonyPostfix]
        public static void OnUpdate_Postfix(GUIHand __instance)
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
            bool targetNeedsCrosshair = (__instance.GetActiveTarget() && playerMode == Player.Mode.Normal) || textHand ||
                (Player.main.IsInsideWalkable() && Array.Exists(showCrosshairWhilePointingAt, element => element == techType));

            //File.AppendAllText("DisableCrosshair_Log.txt",
            //    "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
            //    "\n textHand: " + textHand +
            //    "\n playerMode: " + playerMode +
            //    "\n techtype: " + techType + 
            //    //"\n grabMode: " + ___grabMode +
            //    "\n____________________________________");

            if (crosshairIsOff)
            {
                if (((!CrosshairOptions.NoCrosshairOnFoot || targetNeedsCrosshair) && isNormalOrSitting) ||
                   ((!CrosshairOptions.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairOptions.NoCrosshairInSeaMoth && Player.main.inSeamoth))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    crosshairIsOff = false;
                    File.AppendAllText("DisableCrosshair_Log.txt", " Case: Enable CH");
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
                    HandReticle.main.RequestCrosshairHide();
                    crosshairIsOff = true;
                    File.AppendAllText("DisableCrosshair_Log.txt", " Case: Disable CH");
                    return;
                }
                else return;
            }
        }
    }
}

