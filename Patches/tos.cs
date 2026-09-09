using GorillaTagScripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Patches
{
    internal class tos
    {
        public class TOSPatches
        {
            [HarmonyPatch(typeof(LegalAgreements), "Update")]
            public class Update
            {
                private static bool Prefix(LegalAgreements __instance)
                {
                    FieldInfo scroll = __instance.GetType().GetField("scrollSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo speed = __instance.GetType().GetField("_maxScrollSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
                    ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                    scroll.SetValue(__instance, 10f);
                    speed.SetValue(__instance, 10f);
                    return false;
                }
            }

            [HarmonyPatch(typeof(ModIOTermsOfUse_v1), "PostUpdate")]
            public class PostUpdateModIO
            {
                private static bool Prefix(ModIOTermsOfUse_v1 __instance)
                {
                    __instance.TurnPage(999);
                    ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                    FieldInfo time = __instance.GetType().GetField("holdTime", BindingFlags.NonPublic | BindingFlags.Instance);
                    time.SetValue(__instance, 0.1f);
                    return false;
                }
            }

            [HarmonyPatch(typeof(AgeSlider), "PostUpdate")]
            public class PostUpdateAgeSlider
            {
                private static bool Prefix(AgeSlider __instance)
                {
                    FieldInfo age = __instance.GetType().GetField("currentAge", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo holdTime = __instance.GetType().GetField("holdTime", BindingFlags.NonPublic | BindingFlags.Instance);
                    age.SetValue(__instance, 21);
                    holdTime.SetValue(__instance, 0.1f);
                    return false;
                }
            }

            [HarmonyPatch(typeof(PrivateUIRoom), "StartOverlay")]
            public class StartOverlay
            {
                private static bool Prefix() => false;
            }

            [HarmonyPatch(typeof(KIDManager), nameof(KIDManager.UseKID))]
            public class UseKID
            {
                private static bool Prefix(ref Task<bool> __result)
                {
                    __result = Task.FromResult(false);
                    return false;
                }
            }
        }
    }
}
