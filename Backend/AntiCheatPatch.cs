using HarmonyLib;
using Photon.Pun;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BreezeClient.Backend
{
    [HarmonyPatch(typeof(MonkeAgent), "SendReport")]
    internal class anticheatnotif : MonoBehaviour
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
            if (susId == PhotonNetwork.LocalPlayer.UserId)
            {

            }
            return false;
        }
    }
}
