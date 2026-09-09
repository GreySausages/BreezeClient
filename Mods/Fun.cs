using System;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Breeze.Mods
{
    internal class Fun
    {
        public static void SoundSpam(int id = 18)
        {
            if ((!NetworkSystem.Instance.InRoom || !ControllerInputPoller.instance.rightControllerTriggerButton) && Time.time > rpccooldown + 0.1f) { rpccooldown = Time.time; return; } // I felt like doin the cooldown like this instead of the way i do it in the water splash hand bc idrk

            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
            {
                id,
                false,
                999f
            });
            Move.RPCProt();
        }

        public static float rpccooldown = 0f;
        public static void WaterSplashHands()
        {
            if (Time.time > rpccooldown + 0.1f)
            {
                if (ControllerInputPoller.instance.rightControllerTriggerButton)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.rightHandTransform.position,
                        4f, 100f, false ,true
                    });
                }
                if (ControllerInputPoller.instance.leftControllerTriggerButton)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.leftHandTransform.position,
                        4f, 100f, false ,true
                    });
                }
                rpccooldown = Time.time;
            }
            Move.RPCProt();
        }
    }
}
