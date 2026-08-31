using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Photon.Realtime;
using GorillaLocomotion.Gameplay;
using HarmonyLib;

namespace diddy.hello 
{
    internal class RigShit : MonoBehaviour
    {
        public static VRRig GetRigFromPlayer(Photon.Realtime.Player p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }

        public static PhotonView rig2view(VRRig p)
        {
            return (PhotonView)Traverse.Create(p).Field("photonView").GetValue();
        }

        public static PhotonView GetViewFromPlayer(Photon.Realtime.Player p)
        {
            return rig2view(GorillaGameManager.instance.FindPlayerVRRig(p));
        }

        public static VRRig GetOwnVRRig()
        {
            return GorillaTagger.Instance.offlineVRRig;
        }

        public static PhotonView GetViewFromRig(VRRig rig)
        {
            return rig2view(rig);
        }

        public static Photon.Realtime.Player GetPlayerFromRig(VRRig rig)
        {
            return PhotonNetwork.CurrentRoom.GetPlayer(rig.Creator.ActorNumber);
        }

        public static GorillaRopeSwing GetPlayersRope(VRRig rig)
        {
            return (GorillaRopeSwing)Traverse.Create(rig).Field("currentRopeSwing").GetValue();
        }

        public static bool battleIsOnCooldown(VRRig rig)
        {
            return rig.mainSkin.material.name.Contains("hit");
        }

        public static Photon.Realtime.Player GetRandomPlayer(bool includeSelf)
        {
            if (includeSelf)
            {
                Player p = PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, 11)];
                if (p != null)
                {
                    return p;
                }
                return GetRandomPlayer(includeSelf);
            }
            Player p2 = PhotonNetwork.PlayerListOthers[UnityEngine.Random.Range(0, 10)];
            if (p2 != null)
            {
                return p2;
            }
            return GetRandomPlayer(includeSelf);
        }
    }
}
