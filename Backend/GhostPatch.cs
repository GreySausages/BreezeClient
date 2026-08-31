using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Threading;
using System.Net;
using Photon.Pun;
using System.Linq;
using GorillaLocomotion.Gameplay;

namespace BreezeClient.Backend
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class GhostPatch : MonoBehaviour
    {
        public static bool Prefix(VRRig __instance)
        {
            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                return false;
            }
            return true;
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

