using HarmonyLib;

namespace DisableCrosshairSN
{
    class CrosshairDisabler
    {
        [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
        public static class CrosshairPatcher
        {
            private static bool _crosshairOff;

            public static void Postfix(GUIHand __instance)
            {
/*              Targeting.GetTarget(Player.main.gameObject, 50, out var tar, out var num);
                File.AppendAllText("DisableCrosshairSNLog.txt", "\n __instance.GetActiveTarget() : " + __instance.GetActiveTarget() +
                   "\ntar : "+ tar );*/


                if (Player.main == null) // skip this if no Player.main instance exists
                    return;

                var activeTarget = __instance.GetActiveTarget();
                var crosshairHasText = HandReticle.main.interactPrimaryText.text.ToString().Length > 0;
               
                if (_crosshairOff)
                {
                    if ((activeTarget && !Player.main.inSeamoth && !Player.main.inExosuit) || crosshairHasText)
                    {
                        HandReticle.main.UnrequestCrosshairHide();
                        _crosshairOff = false;
                        return;
                    }

                    else if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
                        return;

                    else if ((!Player.main.inExosuit && !Player.main.inSeamoth) ||
                        (!CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit) ||
                        (!CrosshairMenu.Config.NoCrosshairInSeaMoth && Player.main.inSeamoth))
                    {
                        HandReticle.main.UnrequestCrosshairHide();
                        _crosshairOff = false;
                        return;
                    }
                    else return;
                }

                else //(!_crosshairOff)
                {
                    if ((activeTarget && !(Player.main.inSeamoth || Player.main.inExosuit)) || crosshairHasText)
                        return;

                    else if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
                    {
                        HandReticle.main.RequestCrosshairHide();
                        _crosshairOff = true;
                        return;
                    }

                    else if ((CrosshairMenu.Config.NoCrosshairInSeaMoth && Player.main.inSeamoth) ||
                            (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit))
                    {
                        HandReticle.main.RequestCrosshairHide();
                        _crosshairOff = true;
                    }
                }
            }
        }
    }
}

