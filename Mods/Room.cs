using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Breeze.Mods
{
    internal class Room
    {
        public static string room;
        public static void SaveRoom()
        {
            if (!PhotonNetwork.InRoom) return;
            room = PhotonNetwork.CurrentRoom.Name;
        }
        public static void JoinSavedRoom()
        {
            if (PhotonNetwork.InRoom) return;
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(room, JoinType.Solo);
        }
        public static void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public static Player GetPlayerFromRig(VRRig rig)
        {
            return PhotonNetwork.CurrentRoom.GetPlayer(rig.Creator.ActorNumber);
        }

        public static string PlayerPlatform(Player p)
        {
            p.CustomProperties.TryGetValue("platform", out object platform);
            if (platform == null) platform = "Quest";
            return platform.ToString();
        }

        public static void CreateNameTag(VRRig targetRig)
        {
            if (targetRig == null || targetRig.isOfflineVRRig) return;
            if (!NetworkSystem.Instance.InRoom || GorillaTagger.Instance.offlineVRRig == null) return;

            GameObject tag = new GameObject("tagObj", typeof(Canvas));
            tag.transform.position = targetRig.transform.position + new Vector3(0f, 0.67f, 0f);

            tag.transform.LookAt(Camera.main.transform);
            tag.transform.Rotate(0f, 180f, 0f);

            var tagText = tag.AddComponent<TextMeshPro>();
            tagText.fontSize = 1.5f;
            tagText.alignment = TextAlignmentOptions.Center;
            tagText.color = targetRig.playerColor;
            tagText.text = $"FPS: {targetRig.fps} | Platform: {PlayerPlatform(GetPlayerFromRig(targetRig))}\nName: {targetRig.Creator.NickName}";

            GameObject.Destroy(tag, Time.deltaTime);
        }

        public static void NameTags()
        {
            foreach (VRRig targetRigs in VRRigCache.ActiveRigs)
            {
                CreateNameTag(targetRigs);
            }
        }
    }
}
