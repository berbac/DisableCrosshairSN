using HarmonyLib;

namespace DisableCrosshairSN
{
    [HarmonyPatch(typeof(uGUI), "Update")]
    public static class CrosshairPatcher
    {
        private static bool _crosshairOff = false;

        public static bool Prefix()
        {
            if (!_crosshairOff && CrosshairMenu.Config.DisableCrosshair)
            {
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
                return false;
            }

            else if (_crosshairOff && CrosshairMenu.Config.DisableCrosshair)
            {
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

