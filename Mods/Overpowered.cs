using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Breeze.Mods
{
    internal class Overpowered
    {
        public static void Partykicker()
        {
            if (!PhotonNetwork.InRoom) return;
            if (!FriendshipGroupDetection.Instance.IsInParty) return;
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("TUNGTUNGGOD", JoinType.ForceJoinWithParty);
        }
        public static void Elevatorkicker()
        {
            if (!PhotonNetwork.InRoom) return;
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("TUNGTUNGGOD", JoinType.JoinWithElevator);
        }
    }
}
