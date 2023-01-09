using HarmonyLib;
using System.IO;

namespace DisableCrosshairSN
{
    //[HarmonyPatch(typeof(PilotingChair), nameof(PilotingChair.Update))]
    //public static class DisablePressToExitCyclops
    //{
    //    [HarmonyPrefix]
    //    public static bool Patch()
    //    {
    //        File.AppendAllText("_Log_.txt", "Skipped" + "\n");
    //        return false;
    //    }
    //}
    [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.SetTextRaw))]
    public static class DisablePressToExitMessage
    {
        [HarmonyPrefix]
        public static bool Patch(HandReticle.TextType type, string text)
        {
            if (CrosshairOptions.NoControlHints && type == HandReticle.TextType.Use)
            {
                return false;
            }
          return true;
        }
    }
}
