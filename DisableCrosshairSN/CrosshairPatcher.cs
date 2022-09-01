using HarmonyLib;
using UnityEngine;
using System;
using System.IO;
//TODO : NullReferenceError for a short time after loading
namespace DisableCrosshairSN
{
    [HarmonyPatch]
    public static class CrosshairPatcher
    {
        private static bool crosshairIsOff;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality(Clone)", "SeaTruckSleeperModule(Clone)", "Jukebox(Clone)" }; // special cases to show crosshair
        internal static string techType;
        internal static bool textHand;
        //internal static string activeTarget;

        [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
        [HarmonyPostfix]
        public static void Postfix()//GUIHand __instance
        {
            //if (Player.main == null) // skip this if no Player.main instance exists
            //   return;

            // getting techType for map room screen, jukebox etc.
            // check if CH needs to be enabled for interaction while on foot/swimming
            // try-catch needed -> throws error if no target in range

            try
            {
                Targeting.GetTarget(Player.main.gameObject, 10, out GameObject getTarget, out float _);
                CraftData.GetTechType(getTarget, out var _techType);
                techType = _techType.name;
            }
            catch (NullReferenceException) { techType = null; }

            try { textHand = HandReticle.main.interactPrimaryText.text.Length > 0; }
            catch (NullReferenceException) { textHand = false; }

            //var activeTarget = __instance.GetActiveTarget();

            Player.Mode playerMode = Player.main.GetMode();
            bool isNormalOrSitting = playerMode == Player.Mode.Normal || playerMode == Player.Mode.Sitting;
            bool targetNeedsCrosshair =  textHand || //(activeTarget.Length != 0 && playerMode == Player.Mode.Normal) ||
                (Player.main.IsInsideWalkable() && Array.Exists(showCrosshairWhilePointingAt, element => element == techType)) ||
                (Player.main.inExosuit && techType != "Exosuit(Clone)" && techType.Length > 0);

/*            File.AppendAllText("DisableCrosshair_Log.txt",
            "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
            "\n textHand: " + textHand +
            "\n playerMode: " + playerMode +
            "\n techType: " + techType +
            //"\n activeTarget:" + __instance.GetActiveTarget() +
            "\n____________________________________");*/

            if (crosshairIsOff)
            {
                if (((!CrosshairMenu.Config.NoCrosshairOnFoot || targetNeedsCrosshair) && isNormalOrSitting) ||
                   ((!CrosshairMenu.Config.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairMenu.Config.NoCrosshairInSeaMoth && Player.main.inSeamoth))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    crosshairIsOff = false;
                    return;
                }
                else return;
            }

            else // Crosshair is currently on
            {
                if ((CrosshairMenu.Config.NoCrosshairOnFoot && isNormalOrSitting && !targetNeedsCrosshair) ||
                   (CrosshairMenu.Config.NoCrosshairInSeaMoth && Player.main.inSeamoth) ||
                   (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit && !targetNeedsCrosshair ))
                {
                    HandReticle.main.RequestCrosshairHide();
                    crosshairIsOff = true;
                    return;
                }
                else return;
            }
        }
    }
}

