using HarmonyLib;
namespace DisableCrosshairSN
{
    [HarmonyPatch(typeof(uGUI), "Update")]
    public static class CrosshairPatcher
    {
        private static bool _crosshairOff;// = false;

        public static bool Prefix()
        {
            //File.AppendAllText("CrosshairMod.txt", HandReticle.main.interactPrimaryText.text.ToString() );

            if ((HandReticle.main.interactPrimaryText.text.ToString().Length > 0) )
            { // Checks for tooltip text around the crosshair. If found enables crosshair
                if (_crosshairOff)
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    _crosshairOff = false;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            else if (_crosshairOff && CrosshairMenu.Config.DisableCrosshair)
            {
                return true;
            }

            else if (!_crosshairOff && CrosshairMenu.Config.DisableCrosshair)
            {
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
                return false;
            }

            else if (!_crosshairOff &&
                ((CrosshairMenu.Config.NoCrosshairInSeaMoth && Player.main.inSeamoth) ||
                (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit)))
            {
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
                return false;
            }

            else if (_crosshairOff &&
                ((!Player.main.inExosuit && !Player.main.inSeamoth) ||
                (Player.main.inExosuit && !CrosshairMenu.Config.NoCrosshairInPrawnSuit) ||
                (Player.main.inSeamoth && !CrosshairMenu.Config.NoCrosshairInSeaMoth)))
            {
                HandReticle.main.UnrequestCrosshairHide();
                _crosshairOff = false;
                return false;
            }
            return true;
        }
    }

}

