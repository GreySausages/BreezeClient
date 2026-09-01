using BepInEx;
using BreezeClient.UI;
using BreezeClient.Utilities;
using diddy.hello;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using HarmonyLib;
using OVRSimpleJSON;
using Photon.Pun;   
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static BreezeClient.UI.WristMenu;
using static UnityEngine.UI.GridLayoutGroup;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BreezeClient.Backend
{
    internal class Mods : MonoBehaviour
    {
        public static bool oiwefkwenfjk;

        public static void Disconnect()
        {
            WristMenu.GetButton("Disconnect").enabled = false;
            WristMenu.DestroyMenu();
            WristMenu.instance.Draw();
            PhotonNetwork.Disconnect();
        }
        public static void IronMoneyMonke()
        {
            if (gripDownR)
            {
                GorillaTagger.Instance.rigidbody.AddForce(GorillaTagger.Instance.rightHandTransform.right * 12f * Time.deltaTime, ForceMode.VelocityChange);
            }
            if (gripDownL)
            {
                GorillaTagger.Instance.rigidbody.AddForce(-GorillaTagger.Instance.leftHandTransform.right * 12f * Time.deltaTime, ForceMode.VelocityChange);
            }
        }

        public static void BugGun()
        {
            GameObject Bug = GameObject.Find("Floating Bug Holdable");

            Gunlib(()  =>
            {
                Bug.transform.position = pointer.transform.position + new Vector3(0f, 0.2f, 0f);
            });
        }

        public static void TagSelf()
        {
            if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                return;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig.mainSkin.material.name.Contains("fected"))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = rig.rightHandTransform.position;
                    GorillaGameModes.GameMode.ReportTag(PhotonNetwork.LocalPlayer);
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    break;
                }
            }
            GorillaTagger.Instance.offlineVRRig.enabled = true;
            return;
        }

        public static void JoinDiscord()
        {
            Application.OpenURL("https://discord.gg/FU2642QrRd");
            WristMenu.DisableButton("Join Discord");
        }

        public static void StutterGun(float delay, int howmany)
        {
            Gunlib(() =>
            {
                if (Time.time > Shittymethod)
                {
                    for (int i = 0; i < howmany; i++)
                    {
                        if (LockedPlayer != null)
                        {
                            SendOPRaiseEvent202(LockedPlayer);
                        }
                    }

                    Shittymethod = Time.time + delay;
                }
            });
        }

        public static float Yaw = -1f;
        public static float Pitch = -1f;
        public static float anchorX;
        public static float anchorY;
        public static void WASDFly()
        {
            Rigidbody rb = GorillaTagger.Instance.rigidbody;
            Transform cam = GorillaLocomotion.GTPlayer.Instance.GetControllerTransform(false).parent;
            rb.linearVelocity = Vector3.zero;

            if (Mouse.current.rightButton.isPressed)
            {
                Vector3 euler = cam.rotation.eulerAngles;

                if (Yaw < 0)
                {
                    Yaw = euler.y;
                    anchorX = Mouse.current.position.value.x / Screen.width;
                }
                if (Pitch < 0)
                {
                    Pitch = euler.x;
                    anchorY = Mouse.current.position.value.y / Screen.height;
                }

                float pitch = Pitch - (Mouse.current.position.value.y / Screen.height - anchorY) * 360f * 1.33f;
                float yaw = Yaw + (Mouse.current.position.value.x / Screen.width - anchorX) * 360f * 1.33f;

                pitch = pitch > 180f ? pitch - 360f : pitch;
                pitch = Mathf.Clamp(pitch, -90f, 90f);

                cam.rotation = Quaternion.Euler(pitch, yaw, euler.z);
            }
            else
            {
                Yaw = -1f;
                Pitch = -1f;
            }

            const float speed = 9f;
            float dt = Time.deltaTime * speed;

            KeyCode[] keys = {
                KeyCode.W, KeyCode.S, KeyCode.A,
                KeyCode.D, KeyCode.Space, KeyCode.LeftControl
            };

            foreach (KeyCode key in keys)
            {
                if (!UnityInput.Current.GetKey(key))
                    continue;

                switch (key)
                {
                    case KeyCode.W:
                        rb.transform.position += cam.forward * dt;
                        break;
                    case KeyCode.S:
                        rb.transform.position -= cam.forward * dt;
                        break;
                    case KeyCode.A:
                        rb.transform.position -= cam.right * dt;
                        break;
                    case KeyCode.D:
                        rb.transform.position += cam.right * dt;
                        break;
                    case KeyCode.Space:
                        rb.transform.position += Vector3.up * dt;
                        break;
                    case KeyCode.LeftControl:
                        rb.transform.position += Vector3.down * dt;
                        break;
                }
            }
        }

        public static void EmptyGunlib()
        {
            Gunlib(() => { });
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
            tagText.text = $"FPS: {targetRig.fps} | Platform: {PlayerPlatform(RigShit.GetPlayerFromRig(targetRig))}\nName: {targetRig.Creator.NickName}";

            GameObject.Destroy(tag, Time.deltaTime);
        }

        public static void NameTags()
        {
            foreach (VRRig targetRigs in VRRigCache.ActiveRigs)
            {
                CreateNameTag(targetRigs);
            }
        }
        public static void NameTagGun()
        {
            Gunlib(() =>
            {
                CreateNameTag(LockedPlayer);
            });
        }

        public class LoadGenesis : MonoBehaviour
        {
            public static LoadGenesis Instance;

            public static IEnumerator DownloadAndLoadMenu(string DownloadLink, string name = "Genesis Reborn")
            {
                string directory = Path.GetDirectoryName(Path.Combine(Paths.GameRootPath, $"{name}.dll"));

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                using (UnityWebRequest a = UnityWebRequest.Get(DownloadLink))
                {
                    yield return a.SendWebRequest();

                    if (a.result != UnityWebRequest.Result.Success) yield break;

                    byte[] data = a.downloadHandler.data;

                    if (data == null || data.Length == 0) yield break;

                    try
                    {
                        File.WriteAllBytes(Path.Combine(Paths.GameRootPath, $"{name}.dll"), data);
                    }
                    catch (Exception ex)
                    {
                        yield break;
                    }

                    try
                    {
                        Assembly assembly = Assembly.Load(data);
                        Type pluginType = assembly.GetTypes().FirstOrDefault(type => typeof(BaseUnityPlugin).IsAssignableFrom(type) && !type.IsAbstract && type != typeof(BaseUnityPlugin)); ;
                        GameObject obj = new GameObject(name);
                        DontDestroyOnLoad(obj);
                        obj.AddComponent(pluginType);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            public static void LoadGenesisReborn()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadGenesis");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/incharilla1/ShibaGT-Genesis-Reborn/releases/download/1.0.5/ShibaGTGenesisReborn.dll"));

                DisableButton("Genesis Reborn");
            }
            public static void LoadUndefined()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadUndefined");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/ImudTrust-Projects/Undefined/releases/download/V1.0.3/Undefined.dll", "Undefined"));

                DisableButton("Undefined");
            }
            public static void LoadGenesisReal()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadGenesisReal");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/GreySausages/BreezeClient/raw/refs/heads/main/ShibaGT%20Genesis.dll", "Actual Genesis"));

                DisableButton("Real Genesis (D)");
            }
            public static void LoadGenesisPlon()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadGenesisPlon");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/GreySausages/BreezeClient/raw/refs/heads/main/ShibaGTGenesisReborn.dll", "ShibaGTGenesisReborn"));

                DisableButton("Genesis Reborn (Plon)");
            }
            public static void LoadCubeClient()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadCubeClient");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/GreySausages/BreezeClient/raw/refs/heads/main/Cube.Client.V2.2.dll", "CubeClient"));

                DisableButton("Cube Client");
            }
            public static void LoadUntitled()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadUntitled");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/GreySausages/BreezeClient/raw/refs/heads/main/UntitledMenu.dll", "Untitled"));

                DisableButton("Untitled");
            }
            public static void LoadParrotClient()
            {
                if (Instance == null)
                {
                    GameObject obj = new GameObject("LoadParrotClient");
                    UnityEngine.Object.DontDestroyOnLoad(obj);
                    Instance = obj.AddComponent<LoadGenesis>();
                }

                Instance.StartCoroutine(DownloadAndLoadMenu("https://github.com/Scrypto34/parrot.client/releases/download/1.0.0/parrot.client.dll", "Parrot Client"));

                DisableButton("Parrot Client");
            }
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, System.UIntPtr dwExtraInfo);

        static void Press(byte key)
        {
            keybd_event(key, 0, 0, System.UIntPtr.Zero);
            keybd_event(key, 0, 2, System.UIntPtr.Zero);
        }

        public static void PlayPause()
        {
            Press(179);
            DisableButton("Play | Pause");
        }
        public static void Next()
        {
            Press(176);
            DisableButton("Next");
        }
        public static void Previous()
        {
            Press(177);
            DisableButton("Previous");
        }

        public static void RestartGame()
        {
            Process.Start("steam://rungameid/1533390");
            Application.Quit();
        }

        public static bool inSettings = false;
        public static bool inOp = false;
        public static bool inMovement = false;
        public static bool inRig = false;
        public static bool inTag = false;
        public static bool inSafety = false;
        public static bool inVis = false;
        public static bool inFav = false;
        public static bool inOtherMenus = false;

        public static bool IspressingButton;
        public static bool GhostToggle;
        public static float Shittymethod;
        public static void ProcessTeleportGun()
        {
            Gunlib(() =>
            {
                if (!teleportGunAntiRepeat)
                {
                    Vector3 pos = pointer.transform.position;
                    GorillaLocomotion.GTPlayer.Instance.TeleportTo(pos - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position, GorillaTagger.Instance.transform.rotation, false);
                    VRRig.LocalRig.transform.position = pos;
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                    teleportGunAntiRepeat = true;
                }
            }, () =>
            {
                teleportGunAntiRepeat = false;
            });
        }

        public static int NoBarrier()
        {
            return ~((IEnumerable<string>)new string[] { "TransparentFX", "Ignore Raycast", "Zone", "Gorilla Trigger", "Gorilla Boundary", "GorillaCosmetics", "GorillaParticle" }).Select((Func<string, int>)LayerMask.NameToLayer).Aggregate(0, (int num, int l) => num | (1 << l));
        }
        public static void Gunlib(Action mod, Action mod2 = null)
        {
            if (gripDownR)
            {
                Physics.Raycast(GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position, -GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.up, out var hitInfo, 100f, NoBarrier());
                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitInfo, 100f, NoBarrier());
                }
                if (LockOn)
                {
                    if (LockedPlayer == null && triggerDownR)
                    {
                        LockedPlayer = hitInfo.collider?.GetComponentInParent<VRRig>();
                    }
                    else if (LockedPlayer != null && triggerDownR)
                    {
                        hitInfo.point = LockedPlayer.transform.position;
                    }
                    else if (LockedPlayer != null && !triggerDownR)
                    {
                        LockedPlayer = null;
                    }
                }
                else
                {
                    if (LockedPlayer == null && triggerDownR)
                    {
                        LockedPlayer = hitInfo.collider?.GetComponentInParent<VRRig>();
                    }
                    else if (LockedPlayer != null && hitInfo.collider?.GetComponentInParent<VRRig>() == null)
                    {
                        LockedPlayer = null;
                    }
                }
                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                    GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.GetComponent<Renderer>().material.color = Color.red;
                    pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                pointer.transform.position = hitInfo.point;
                if (triggerDownR)
                {
                    pointer.GetComponent<Renderer>().material.color = Color.green;
                    GameObject g = new GameObject("Line");
                    LineRenderer l = g.AddComponent<LineRenderer>();
                    l.startWidth = 0.01f;
                    l.endWidth = 0.01f;
                    l.positionCount = 2;
                    l.useWorldSpace = true;
                    l.SetPosition(0, GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position);
                    l.SetPosition(1, hitInfo.point);
                    l.material.shader = Shader.Find("Sprites/Default");
                    l.startColor = Color.green;
                    l.endColor = Color.green;
                    GameObject.Destroy(l, Time.deltaTime);
                    mod.Invoke();
                }
                else if (!triggerDownR)
                {
                    pointer.GetComponent<Renderer>().material.color = Color.red;
                    GameObject g = new GameObject("Line");
                    LineRenderer l = g.AddComponent<LineRenderer>();
                    l.startWidth = 0.01f;
                    l.endWidth = 0.01f;
                    l.positionCount = 2;
                    l.useWorldSpace = true;
                    l.SetPosition(0, GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position);
                    l.SetPosition(1, hitInfo.point);
                    l.material.shader = Shader.Find("Sprites/Default");
                    l.startColor = Color.red;
                    l.endColor = Color.red;
                    GameObject.Destroy(l, Time.deltaTime);
                    mod2?.Invoke();
                }
            }
            else
            {
                mod2?.Invoke();
                GameObject.Destroy(pointer);
                pointer = null;
            }
        }

        public static void Gunlib(Action mod, bool LockOn)
        {
            if (gripDownR)
            {
                Physics.Raycast(GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position, -GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.up, out var hitInfo, 100f, NoBarrier());
                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitInfo, 100f, NoBarrier());
                }
                if (LockOn)
                {
                    if (LockedPlayer == null && triggerDownR)
                    {
                        LockedPlayer = hitInfo.collider?.GetComponentInParent<VRRig>();
                    }
                    else if (LockedPlayer != null && triggerDownR)
                    {
                        hitInfo.point = LockedPlayer.transform.position;
                    }
                    else if (LockedPlayer != null && !triggerDownR)
                    {
                        LockedPlayer = null;
                    }
                }
                else
                {
                    if (LockedPlayer == null && triggerDownR)
                    {
                        LockedPlayer = hitInfo.collider?.GetComponentInParent<VRRig>();
                    }
                    else if (LockedPlayer != null && hitInfo.collider?.GetComponentInParent<VRRig>() == null)
                    {
                        LockedPlayer = null;
                    }
                }
                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                    GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                    pointer.GetComponent<Renderer>().material.color = Color.red;
                    pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                pointer.transform.position = hitInfo.point;
                if (triggerDownR)
                {
                    pointer.GetComponent<Renderer>().material.color = Color.green;
                    GameObject g = new GameObject("Line");
                    LineRenderer l = g.AddComponent<LineRenderer>();
                    l.startWidth = 0.01f;
                    l.endWidth = 0.01f;
                    l.positionCount = 2;
                    l.useWorldSpace = true;
                    l.SetPosition(0, GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position);
                    l.SetPosition(1, hitInfo.point);
                    l.material.shader = Shader.Find("Sprites/Default");
                    l.startColor = Color.green;
                    l.endColor = Color.green;
                    GameObject.Destroy(l, Time.deltaTime);
                    mod.Invoke();
                }
                else if (!triggerDownR)
                {
                    pointer.GetComponent<Renderer>().material.color = Color.red;
                    GameObject g = new GameObject("Line");
                    LineRenderer l = g.AddComponent<LineRenderer>();
                    l.startWidth = 0.01f;
                    l.endWidth = 0.01f;
                    l.positionCount = 2;
                    l.useWorldSpace = true;
                    l.SetPosition(0, GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position);
                    l.SetPosition(1, hitInfo.point);
                    l.material.shader = Shader.Find("Sprites/Default");
                    l.startColor = Color.red;
                    l.endColor = Color.red;
                    GameObject.Destroy(l, Time.deltaTime);
                }
            }
            else
            {
                GameObject.Destroy(pointer);
                pointer = null;
            }
        }

        public static float notifcooldown;
        public static void AntiReport()
        {
            foreach (GorillaPlayerScoreboardLine boardline in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (boardline.linePlayer != NetworkSystem.Instance.LocalPlayer || boardline.reportButton == null)
                {
                    Transform transform = boardline.reportButton.gameObject.transform;
                    foreach (VRRig vrrig in VRRigCache.ActiveRigs)
                    {
                        if (vrrig == null || vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            if ((Vector3.Distance(vrrig.rightHandTransform.position, transform.position) < 0.41 || Vector3.Distance(vrrig.leftHandTransform.position, transform.position) < 0.41) && Time.time > notifcooldown)
                            {
                                notifcooldown = Time.time + 1f;
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void SendOPRaiseEvent202(VRRig p = null)
        {
            RaiseEventOptions fuck;
            if (p != null)
            {
                fuck = new RaiseEventOptions { TargetActors = new int[] { p.Creator.ActorNumber } };
            }
            else
            {
                fuck = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            }
            PhotonNetwork.NetworkingClient.OpRaiseEvent(202, new object[]
            {
                "ello fat ass"
            }, fuck, SendOptions.SendUnreliable);
            Mods.RPCProt();
        }

        public static void RPCProt()
        {
            MonkeAgent.instance.rpcErrorMax = int.MaxValue;
            MonkeAgent.instance.rpcCallLimit = int.MaxValue;
            MonkeAgent.instance.logErrorMax = int.MaxValue;
            MonkeAgent.instance.userRPCCalls.Clear();
            MonkeAgent.instance.lastCheck = 0f;
            MonkeAgent.instance.userDecayTime = 0f;

            PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
            PhotonNetwork.QuickResends = int.MaxValue;

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public static void TagGun()
        {
            Gunlib(() =>
            {
                if (!LockedPlayer.mainSkin.material.name.Contains("fected") || !LockedPlayer.mainSkin.material.name.Contains("It"))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = LockedPlayer.transform.position;
                    GorillaTagger.Instance.offlineVRRig.leftHandTransform.position = LockedPlayer.transform.position;
                    GorillaTagger.Instance.offlineVRRig.rightHandTransform.position = LockedPlayer.transform.position;
                    GorillaGameModes.GameMode.ReportTag(RigShit.GetPlayerFromRig(LockedPlayer));
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            });
        }

        public static void TagPlayer(VRRig p)
        {
            if (p != GorillaTagger.Instance.offlineVRRig)
            {
                if (!p.mainSkin.material.name.Contains("fected"))
                {
                    GorillaGameModes.GameMode.ReportTag(p.Creator);

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = p.headConstraint.position;
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void TagAll()
        {
            foreach (VRRig p in VRRigCache.ActiveRigs)
            {
                if (p != GorillaTagger.Instance.offlineVRRig)
                {
                    TagPlayer(p);
                }
            }
        }
        public static void LagGun(float delay, int howmany)
        {
            Gunlib(() =>
            {
                if (Time.time > Shittymethod)
                {
                    for (int i = 0; i < howmany; i++)
                    {
                        if (LockedPlayer != null)
                        {
                            SendOPRaiseEvent202(LockedPlayer);
                        }
                    }
                    Shittymethod = Time.time + delay;
                }
            });
        }
        public static void LagAll(float delay, int howmany)
        {
            if (Time.time > Shittymethod)
            {
                for (int i = 0; i < howmany; i++)
                {
                    SendOPRaiseEvent202();
                }
                Shittymethod = Time.time + delay;
            }
        }
        public static VRRig LockedPlayer = null;
        public static bool LockOn;
        public static bool LockOnshit;
        public static bool teleportGunAntiRepeat;

        public static void Settings()
        {
            WristMenu.GetButton("Settings").enabled = false;
            inSettings = !inSettings;
            inOp = false;
            inMovement = false;
            pageNumber = 0;
            UpdateMenu();
        }
        public static void Op()
        {
            WristMenu.GetButton("OP Mods").enabled = false;
            inOp = !inOp;
            inSettings = false;
            inMovement = false;
            inSafety = false;
            pageNumber = 0;
            UpdateMenu();
        }
        public static void Movement()
        {
            WristMenu.GetButton("Movement Mods").enabled = false;
            inMovement = !inMovement;
            inSettings = false;
            inOp = false;
            inSafety = false;
            pageNumber = 0;
            UpdateMenu();
        }
        public static void Rig()
        {
            WristMenu.DisableButton("Rig Mods");
            inRig = !inRig;
            inSettings = false;
            inOp = false;
            inSafety = false;
            pageNumber = 0;
            UpdateMenu();
        }
        public static void Tag()
        {
            WristMenu.GetButton("Tag Mods").enabled = false;
            inTag = !inTag;
            inSettings = false;
            inOp = false;
            inSafety = false;
            WristMenu.pageNumber = 0;
            UpdateMenu();
        }
        public static void Safety()
        {
            WristMenu.GetButton("Safety Mods").enabled = false;
            inSafety = !inSafety;
            inSettings = false;
            inOp = false;
            inTag = false;
            pageNumber = 0;
            UpdateMenu();
        }
        public static void Vis()
        {
            WristMenu.GetButton("Visual Mods").enabled = false;
            pageNumber = 0;
            inVis = !inVis;
            inSettings = false;
            inOp = false;
            inTag = false;
            UpdateMenu();
        }
        public static void Favo()
        {
            WristMenu.GetButton("Music").enabled = false;
            inFav = !inFav;
            pageNumber = 0;
            UpdateMenu();
        }
        public static void OtherMenus()
        {
            WristMenu.GetButton("Other Mods").enabled = false;
            pageNumber = 0;
            inOtherMenus = !inOtherMenus;
            UpdateMenu();
        }

        public static void Tracers()
        {
            foreach (VRRig p in VRRigCache.ActiveRigs)
            {
                GameObject gameObject = new GameObject("Line");
                LineRenderer shit = gameObject.AddComponent<LineRenderer>();
                shit.startWidth = 0.01f;
                shit.endWidth = 0.01f;
                shit.positionCount = 2;
                shit.useWorldSpace = true;
                shit.SetPosition(0, GTPlayer.Instance.RightHand.controllerTransform.position);
                shit.SetPosition(1, p.transform.position);
                shit.startColor = Color.green;
                shit.endColor = Color.green;
                Object.Destroy(shit, Time.deltaTime);
            }
        }

        public static void UpdateMenu()
        {
            WristMenu.DestroyMenu();
            WristMenu.instance.Draw();
        }

        private static int Platcolor;
        public static readonly Color[] PlatColors =
        {
            Color.blue, 
            Color.red, 
            Color.green, 
            Color.cyan,
            Color.magenta,
        };
        public static readonly string[] ColorNames =
        {
            "Blue",
            "Red",
            "Green",
            "Cyan",
            "Magenta",
        };
        public static void PlatColorChange()
        {
            Platcolor = (Platcolor + 1) % PlatColors.Length;
            WristMenu.GetButton("Change Plat Color").overlapText = "Platform Color: " + ColorNames[Platcolor];
            WristMenu.DisableButton("Change Plat Color");
            PlatColor = PlatColors[Platcolor];
        }

        private static int Fly;
        private static int Car;
        public static readonly float[] FlySpeeds = 
        {
            9f,
            12f,
            15f,
            25f
        };
        public static readonly string[] SpeedNames =
        {
            "Slow",
            "Normal",
            "Fast",
            "Super Fast"
        };
        public static void FlySpeedChange()
        {
            Fly = (Fly + 1) % SpeedNames.Length;
            WristMenu.GetButton("Fly Speed").overlapText = $"Fly Speed: {SpeedNames[Fly]}";
            FlySpeed = FlySpeeds[Fly];
            WristMenu.DisableButton("Fly Speed");
        }
        public static float FlySpeed = 9f;

        public static void CarSpeedChange()
        {
            Car = (Car + 1) % SpeedNames.Length;
            WristMenu.GetButton("Car Speed").overlapText = $"Car Monkey Speed: {SpeedNames[Car]}";
            CarSpeed = FlySpeeds[Car];
            WristMenu.DisableButton("Car Speed");
        }
        public static void FullBodyESP()
        {
            foreach (VRRig vrrig in VRRigCache.ActiveRigs)
            {
                if (!vrrig.isOfflineVRRig)
                {
                    if (vrrig.mainSkin.material.name.Contains("fected") || vrrig.mainSkin.material.name.Contains("It"))
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = new Color32(255, 0, 0, 100);
                    }
                    else
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = new Color32(0, 255, 0, 100);
                    }
                }
            }
        }
        public static void DisableFullBodyESP()
        {
            foreach (VRRig vrrig in VRRigCache.ActiveRigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig && vrrig.mainSkin.material.shader == Shader.Find("GUI/Text Shader"))
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
        }
        public static void RGB()
        {
            Color c = Color.HSVToRGB(Mathf.Repeat(Time.time * 0.2f, 1f), 1f, 1f);
            if (!PhotonNetwork.InRoom) return;
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, c.r, c.g, c.b);
        }
        public static float CarSpeed = 9f;

        public static bool invisplat = false;
        public static bool stickyplatforms = false;
        public static GameObject funn;
        public static bool fpcc;
        public static Color PlatColor = Color.blue;

        private static GameObject PlatR, PlatL = null;
        public static void Platforms(bool Invis)
        {
            if (ControllerInputPoller.instance.rightGrab && PlatR == null)
            {
                PlatR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                PlatR.transform.localScale = scale;
                PlatR.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                PlatR.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                GameObject.Destroy(PlatR.GetComponent<Rigidbody>());
                PlatR.GetComponent<Renderer>().material.color = PlatColor;
                if (Invis) GameObject.Destroy(PlatR.GetComponent<Renderer>());
            }
            if (!ControllerInputPoller.instance.rightGrab && PlatR != null)
            {
                GameObject.Destroy(PlatR);
                PlatR = null;
            }
            if (ControllerInputPoller.instance.leftGrab && PlatL == null)
            {
                PlatL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                PlatL.transform.localScale = scale;
                PlatL.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                PlatL.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                GameObject.Destroy(PlatL.GetComponent<Rigidbody>());
                PlatL.GetComponent<Renderer>().material.color = PlatColor;
                if (Invis) GameObject.Destroy(PlatL.GetComponent<Renderer>());
            }
            if (!ControllerInputPoller.instance.leftGrab && PlatL != null)
            {
                GameObject.Destroy(PlatL);
                PlatL = null;
            }
        }
        public static void InvisPlatforms()
        {
            PlatformsThing(true, false);
        }
        public static void Flush()
        {
            if (!PhotonNetwork.InRoom) return;
            PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
            PhotonNetwork.RemoveBufferedRPCs();
            PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
            PhotonNetwork.QuickResends = int.MaxValue;

            if (GorillaTagger.Instance.myVRRig != null)
            {
                PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig.GetView);
            }

            MonkeAgent.instance.rpcErrorMax = int.MaxValue;
            MonkeAgent.instance.rpcCallLimit = int.MaxValue;
            MonkeAgent.instance.logErrorMax = int.MaxValue;

            PhotonNetwork.SendAllOutgoingCommands();
        }
        public static void NoTagOnJoin()
        {
            PlayerPrefs.SetString("didTutorial", "nope");
            PlayerPrefs.SetString("tutorial", "nope");
            Hashtable hasht = new Hashtable();
            hasht.Add("didTutorial", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hasht, null, null);
            PlayerPrefs.Save();
        }

        public static void LowGravity() => GorillaTagger.Instance.rigidbody.AddForce(Vector3.up * 6.66f, ForceMode.Acceleration);

        public static void HighGravity() => GorillaTagger.Instance.rigidbody.AddForce(Vector3.down * 6.66f, ForceMode.Acceleration);

        public static void ZeroGravity() => GorillaTagger.Instance.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);

        public static void Noclip()
        {
            MeshCollider[] colliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
            foreach (MeshCollider collider in colliders)
            {
                collider.enabled = !(ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f);
            }
        }

        public static void CarMonkeyandfly(float speed, bool fly)
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.position += GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * speed;
                if (fly) GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }

        public static void SpeedBoost(float speed) => GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = speed;

        public static GameObject OrbR, OrbL = null;
        public static void InvisGhostShit()
        {
            OrbR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            OrbR.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            OrbR.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
            OrbR.transform.rotation = GTPlayer.Instance.RightHand.controllerTransform.rotation;
            GameObject.Destroy(OrbR.GetComponent<Rigidbody>());
            GameObject.Destroy(OrbR.GetComponent<SphereCollider>());
            GameObject.Destroy(OrbR.GetComponent<Collider>());
            OrbR.GetComponent<Renderer>().material.color = Color.blue;
            OrbL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            OrbL.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            OrbL.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
            OrbL.transform.rotation = GTPlayer.Instance.LeftHand.controllerTransform.rotation;
            GameObject.Destroy(OrbL.GetComponent<Rigidbody>());
            GameObject.Destroy(OrbL.GetComponent<SphereCollider>());
            GameObject.Destroy(OrbL.GetComponent<Collider>());
            OrbL.GetComponent<Renderer>().material.color = Color.blue;
            GameObject.Destroy(OrbR, Time.deltaTime);        
            GameObject.Destroy(OrbL, Time.deltaTime);        
        }

        private static GameObject Orb1, Orb2 = null;
        public static bool shit;
        public static bool shit2;
        public static void GhostMonkey()
        {
            VRRig.LocalRig.enabled = !shit2;
            if (ControllerInputPoller.instance.rightControllerPrimaryButton && !shit)
                shit2 = !shit2;
            if (shit2) InvisGhostShit();
            shit = ControllerInputPoller.instance.rightControllerPrimaryButton;
        }
        public static void InvisMonkey()
        {
            VRRig.LocalRig.headBodyOffset.x = shit2 ? 180f : 0f;
            if (ControllerInputPoller.instance.rightControllerPrimaryButton && !shit)
                shit2 = !shit2;
            if (shit2) InvisGhostShit();
            shit = ControllerInputPoller.instance.rightControllerPrimaryButton;
        }

        public static void UpAndDown()
        {
            if (triggerDownR)
            {
                GorillaTagger.Instance.rigidbody.AddForce(GorillaTagger.Instance.bodyCollider.transform.up * 12f * Time.deltaTime, ForceMode.VelocityChange);

            }
            if (triggerDownL)
            {
                GorillaTagger.Instance.rigidbody.AddForce(-GTPlayer.Instance.bodyCollider.transform.up * 12f * Time.deltaTime, ForceMode.VelocityChange);
            }
        }

        public static void Gunlock()
        {
            LockOn = true;
            LockOnshit = true;
        }
        public static void NoGunlock()
        {
            LockOn = false;
            LockOnshit = false;
        }

        public static void Cube()
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);

            g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            g.transform.SetParent(GTPlayer.Instance.RightHand.controllerTransform, false);

            g.GetComponent<Renderer>().material.color = Color.black;
            GameObject.Destroy(g.GetComponent<Collider>());
            GameObject.Destroy(g.GetComponent<MeshCollider>());

            Destroy(g, Time.deltaTime);
        }

        public static GameObject pointer;
        public static void fpc()
        {
            fpcc = true;
            if (GameObject.Find("Third Person Camera") != null)
            {
                funn = GameObject.Find("Third Person Camera");
                funn.SetActive(false);
            }
            if (GameObject.Find("CameraTablet(Clone)") != null)
            {
                funn = GameObject.Find("CameraTablet(Clone)");
                funn.SetActive(false);
            }
        }

        public static void fpcoff()
        {
            fpcc = false;
            if (funn != null)
            {
                funn.SetActive(true);
                funn = null;
            }
        }
        
        public static void Save()
        {
            WristMenu.DisableButton("Save Mods");
            UpdateMenu();
            List<String> list = new List<String>();
            foreach (List<ButtonInfo> info1 in WristMenu.GetAllLists())
            {
                foreach (ButtonInfo info in info1)
                {
                    if (info.enabled == true)
                    {
                        list.Add(info.buttonText);
                    }
                }
            }
            System.IO.Directory.CreateDirectory("BreezePrefs");
            System.IO.File.WriteAllLines("BreezePrefs\\BreezeSavedPrefs.txt", list);
        }

        public static void Load()
        {
            String[] thing = System.IO.File.ReadAllLines("BreezePrefs\\BreezeSavedPrefs.txt");
            foreach (String thing2 in thing)
            {
                foreach (List<ButtonInfo> info1 in WristMenu.GetAllLists())
                {
                    foreach (ButtonInfo b in info1)
                    {
                        if (b.buttonText == thing2)
                        {
                            b.enabled = true;
                        }
                    }
                }
            }
        }

        private static void PlatformsThing(bool invis, bool sticky)
        {
            colorKeysPlatformMonke[0].color = Color.blue;
            colorKeysPlatformMonke[0].time = 0f;
            colorKeysPlatformMonke[1].color = Color.blue;
            colorKeysPlatformMonke[1].time = 0.3f;
            colorKeysPlatformMonke[2].color = Color.blue;
            colorKeysPlatformMonke[2].time = 0.6f;
            colorKeysPlatformMonke[3].color = Color.blue;
            colorKeysPlatformMonke[3].time = 1f;
            bool inputr;
            bool inputl;
                inputr = WristMenu.gripDownR;
                inputl = WristMenu.gripDownL;
            if (inputr)
            {
                if (!once_right && jump_right_local == null)
                {
                    if (sticky)
                    {
                        jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    }
                    else
                    {
                        jump_right_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    }
                    jump_right_local.GetComponent<Renderer>().material.color = Color.black;
                    if (invis)
                    {
                        UnityEngine.Object.Destroy(jump_right_local.GetComponent<Renderer>());
                    }
                    jump_right_local.transform.localScale = scale;
                    jump_right_local.transform.position = new Vector3(0f, -0.0100f, 0f) + GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position;
                    jump_right_local.transform.rotation = GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.rotation;
                    object[] eventContent = new object[2]
                    {
                    new Vector3(0f, -0.0100f, 0f) + GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position,
                    GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.rotation
                    };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others
                    };
                    PhotonNetwork.RaiseEvent(70, eventContent, raiseEventOptions, SendOptions.SendReliable);
                    once_right = true;
                    once_right_false = false;
                    ColorChanger colorChanger = jump_right_local.AddComponent<ColorChanger>();
                    Gradient gradient = new Gradient
                    {
                        colorKeys = colorKeysPlatformMonke
                    };
                    colorChanger.colors = gradient;
                    colorChanger.Start();
                }
            }
            else if (!once_right_false && jump_right_local != null)
            {
                UnityEngine.Object.Destroy(jump_right_local);
                jump_right_local = null;
                once_right = false;
                once_right_false = true;
                RaiseEventOptions raiseEventOptions2 = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others
                };
                PhotonNetwork.RaiseEvent(72, null, raiseEventOptions2, SendOptions.SendReliable);
            }
            if (inputl)
            {
                if (!once_left && jump_left_local == null)
                {
                    if (sticky)
                    {
                        jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    }
                    else
                    {
                        jump_left_local = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    }
                    jump_left_local.GetComponent<Renderer>().material.color = Color.black;
                    if (invis)
                    {
                        UnityEngine.Object.Destroy(jump_left_local.GetComponent<Renderer>());
                    }
                    jump_left_local.transform.localScale = scale;
                    jump_left_local.transform.position = new Vector3(0f, -0.0100f, 0f) + GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.position;
                    jump_left_local.transform.rotation = GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.rotation;
                    object[] eventContent2 = new object[2]
                    {
                    new Vector3(0f, -0.0100f, 0f) + GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.position,
                    GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.rotation
                    };
                    RaiseEventOptions raiseEventOptions3 = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others
                    };
                    PhotonNetwork.RaiseEvent(69, eventContent2, raiseEventOptions3, SendOptions.SendReliable);
                    once_left = true;
                    once_left_false = false;
                    ColorChanger colorChanger2 = jump_left_local.AddComponent<ColorChanger>();
                    Gradient gradient2 = new Gradient();
                    gradient2.colorKeys = colorKeysPlatformMonke;
                    colorChanger2.colors = gradient2;
                    colorChanger2.Start();
                }
            }
            else if (!once_left_false && jump_left_local != null)
            {
                UnityEngine.Object.Destroy(jump_left_local);
                jump_left_local = null;
                once_left = false;
                once_left_false = true;
                RaiseEventOptions raiseEventOptions4 = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others
                };
                PhotonNetwork.RaiseEvent(71, null, raiseEventOptions4, SendOptions.SendReliable);
            }
            if (!PhotonNetwork.InRoom)
            {
                for (int i = 0; i < jump_right_network.Length; i++)
                {
                    UnityEngine.Object.Destroy(jump_right_network[i]);
                }
                for (int j = 0; j < jump_left_network.Length; j++)
                {
                    UnityEngine.Object.Destroy(jump_left_network[j]);
                }
            }
        }

        private static Vector3 scale = new Vector3(0.0125f, 0.28f, 0.3825f);

        private static bool once_left;

        private static bool once_right;

        private static bool once_left_false;

        private static bool once_right_false;

        private static bool once_networking;

        private static GameObject[] jump_left_network = new GameObject[9999];

        private static GameObject[] jump_right_network = new GameObject[9999];

        private static GameObject jump_left_local = null;

        private static GameObject jump_right_local = null;

        private static GradientColorKey[] colorKeysPlatformMonke = new GradientColorKey[4];

        private static Vector3? checkpointPos;

    }
}
