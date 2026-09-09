using HarmonyLib;
using UnityEngine;

namespace Breeze.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class GhostPatch : MonoBehaviour
    {
        public static bool Prefix(VRRig __instance)
        {
            return !(__instance == GorillaTagger.Instance.offlineVRRig);
        }
    }

    [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
    public static class GhostPatch2
    {
        public static bool Prefix(VRRigJobManager __instance, VRRig rig)
        {
            return !(__instance == GorillaTagger.Instance.offlineVRRig);
        }
    }

    [HarmonyPatch(typeof(VRRig), "PostTick")]
    public class RigPatch3
    {
        public static bool Prefix(VRRig __instance) =>
            !__instance.isLocal || __instance.enabled;
    }
}
