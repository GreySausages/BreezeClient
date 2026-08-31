using HarmonyLib;
using JetBrains.Annotations;
using Photon.Pun;
using PlayFab;
using PlayFab.EventsModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BreezeClient.Backend
{
    [HarmonyPatch(typeof(MonkeAgent), "SendReport")]
    internal class AntiCheat : MonoBehaviour
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
            if (susReason.ToLower() == "empty rig")
                return false;

            if (susId == PhotonNetwork.LocalPlayer.UserId)
            {
                PhotonNetwork.Disconnect();
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(MonkeAgent), "LogErrorCount")]
    public class NoLogErrorCount : MonoBehaviour
    {
        private static bool Prefix(string logString, string stackTrace, LogType type) => false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "CloseInvalidRoom")]
    public class NoCloseInvalidRoom : MonoBehaviour
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "CheckReports")]
    public class NoCheckReports : MonoBehaviour
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "QuitDelay", MethodType.Enumerator)]
    public class NoQuitDelay : MonoBehaviour
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "IncrementRPCCallLocal")]
    public class NoIncrementRPCCallLocal : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction) => false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker : MonoBehaviour
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(MonkeAgent), "IncrementRPCCall", new Type[] { typeof(PhotonMessageInfo), typeof(string) })]
    public class NoIncrementRPCCall : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfo info, string callingMethod = "") => false;
    }

    [HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[] { typeof(PhotonMessageInfoWrapped), typeof(string) })]
    public class NoIncrementRPC : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall) => false;
    }

    [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.EnqueueTelemetryEvent))]
    public class EnqueueTelemetryEvent
    {
        private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null) => false;
    }

    [HarmonyPatch(typeof(PlayFabEventsAPI), nameof(PlayFabEventsAPI.WriteTelemetryEvents))]
    public class WriteTelemetryEvents
    {
        private static bool Prefix(WriteEventsRequest request, System.Action<WriteEventsResponse> resultCallback, System.Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null) => false;
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), nameof(PlayFabClientInstanceAPI.ReportDeviceInfo))]
    public class PlayfabUtil02
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), nameof(PlayFabClientAPI.ReportDeviceInfo))]
    public class PlayfabUtil03
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), nameof(PlayFabClientAPI.AttributeInstall))]
    public class PlayfabUtil05
    {
        private static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(PlayFabHttp), nameof(PlayFabHttp.InitializeScreenTimeTracker))]
    public class PlayfabUtil06
    {
        private static bool Prefix() => false;
    }
}