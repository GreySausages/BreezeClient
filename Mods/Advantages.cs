using Breeze.Menu;
using Breeze.Notifications;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GunlibTemp;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breeze.Mods
{
    internal class Advantages
    {
        public static void TagPlayer(VRRig p)
        {
            if (p != GorillaTagger.Instance.offlineVRRig)
            {
                if (!p.mainSkin.material.name.Contains("fected"))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = p.headConstraint.position;
                    GameMode.ReportTag(p.Creator);
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void FlickTagGun()
        {
            Gunlib(() =>
            {
                Reach();
                GorillaTagger.Instance.rightHandTransform.position = LockedPlayer.headConstraint.transform.position;
            }, () =>
            {

            });
        }

        public static void TagAll()
        {
            if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
            {
                Main.GetIndex("Tag All").enabled = false;
                Main.RecreateMenu();
            }
            else
            {
                foreach (VRRig p in VRRigCache.ActiveRigs)
                {
                    if (p != GorillaTagger.Instance.offlineVRRig)
                    {
                        TagPlayer(p);
                    }
                }
            }
        }

        public static void TagGun()
        {
            Gunlib(() =>
            {
                TagPlayer(LockedPlayer);
            }, () =>
            {

            });
        }

        public static void SpawnBoard()
        {
            if (GTPlayer.Instance == null || VRRig.LocalRig == null) return;
            GTPlayer.Instance.isHoverAllowed = true;
            GTPlayer.Instance.SetHoverActive(true);
        }

        public static VRRig LockedPlayer = null;
        public static bool LockOn = true;
        public static GameObject pointer = null;
        public static int NoBarrier()
        {
            return ~((IEnumerable<string>)new string[] { "TransparentFX", "Ignore Raycast", "Zone", "Gorilla Trigger", "Gorilla Boundary", "GorillaCosmetics", "GorillaParticle" }).Select((Func<string, int>)LayerMask.NameToLayer).Aggregate(0, (int num, int l) => num | (1 << l));
        }
        public static void Gunlib(Action mod, Action mod2 = null)
        {
            if (Main.gripDownR)
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
                    if (LockedPlayer == null && Main.triggerDownR)
                        LockedPlayer = hitInfo.collider?.GetComponentInParent<VRRig>();
                    else if (LockedPlayer != null && Main.triggerDownR)
                        hitInfo.point = LockedPlayer.transform.position;
                    else if (LockedPlayer != null && !Main.triggerDownR)
                        LockedPlayer = null;
                }
                else
                {
                    if (LockedPlayer == null && Main.triggerDownR)
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
                if (Main.triggerDownR)
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
                else if (!Main.triggerDownR)
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

        public static void SendOPRaiseEvent(VRRig p = null)
        {
            RaiseEventOptions options;
            if (p != null)
            {
                options = new RaiseEventOptions
                {
                    TargetActors = new int[]
                    {
                        p.Creator.ActorNumber
                    }
                };
            }
            else
            {
                options = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others
                };
            }
            PhotonNetwork.NetworkingClient.OpRaiseEvent(72, new Hashtable(), options, SendOptions.SendUnreliable);
            Move.RPCProt();
        }

        public static float LagCooldown;
        public static void LagGun(float delay, int howmany)
        {
            Gunlib(() =>
            {
                if (Time.time > LagCooldown)
                {
                    for (int i = 0; i < howmany; i++)
                    {
                        if (LockedPlayer != null)
                        {
                            SendOPRaiseEvent(LockedPlayer);
                        }
                    }
                    LagCooldown = Time.time + delay;
                }
            }, () =>
            {

            });
        }

        public static void LagAll(float delay, int howmany)
        {
            if (Time.time > LagCooldown)
            {
                for (int i = 0; i < howmany; i++)
                {
                    SendOPRaiseEvent();
                }
                LagCooldown = Time.time + delay;
            }
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
                    GameMode.ReportTag(GorillaTagger.Instance.offlineVRRig.Creator);
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    break;
                }
            }
            GorillaTagger.Instance.offlineVRRig.enabled = true;
            return;
        }

        private static float oldMaxTagDistance;
        private static float? oldTagRadius = null;

        public static void Reach()
        {
            var tagger = GorillaTagger.Instance;
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            oldTagRadius = GorillaTagger.Instance.maxTagDistance;
            oldMaxTagDistance = int.MaxValue;

            GorillaTagger.Instance.maxTagDistance = float.MaxValue;

            typeof(GorillaTagger).GetField("tagRadiusOverride", flags)?.SetValue(tagger, (float?)1f);
            typeof(GorillaTagger).GetField("tagRadiusOverrideFrame", flags)?.SetValue(tagger, Time.frameCount + 16);
        }

        public static void NoReach()
        {
            GorillaTagger.Instance.maxTagDistance = oldMaxTagDistance;

            var tagger = GorillaTagger.Instance;
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            typeof(GorillaTagger).GetField("tagRadiusOverride", flags)?.SetValue(tagger, oldTagRadius);
            typeof(GorillaTagger).GetField("tagRadiusOverrideFrame", flags)?.SetValue(tagger, Time.frameCount + 16);
        }

        public static string _leavesName;
        public static readonly List<GameObject> leaves = new List<GameObject>();
        private static readonly Dictionary<string, GameObject> objectPool = new Dictionary<string, GameObject>();

        public static void removeleaves()
        {
            if (_leavesName == null)
            {
                var path = "Environment Objects/LocalObjects_Prefab/Forest";
                if (!objectPool.TryGetValue(path, out var f))
                {
                    f = GameObject.Find(path);
                    if (f != null)
                        objectPool.Add(path, f);
                }

                if (f != null)
                {
                    var counts = new Dictionary<string, (int count, int siblingIndex)>();
                    for (int i = 0; i < f.transform.childCount; i++)
                    {
                        var t = f.transform.GetChild(i);
                        if (!t.name.StartsWith("UnityTempFile"))
                            continue;
                        if (!counts.TryGetValue(t.name, out var entry))
                            counts[t.name] = (1, t.GetSiblingIndex());
                        else
                            counts[t.name] = (entry.count + 1, entry.siblingIndex);
                    }
                    _leavesName = counts.Where(kv => kv.Value.count == 3).OrderByDescending(kv => kv.Value.siblingIndex).FirstOrDefault().Key ?? "UnityTempFile";
                }
            }

            foreach (var path in new[] { "Environment Objects/LocalObjects_Prefab/Forest", "RankedMain/Ranked_Layout/Ranked_Forest_prefab" })
            {
                if (!objectPool.TryGetValue(path, out var forest))
                {
                    forest = GameObject.Find(path);
                    if (!forest && path.Contains("/"))
                    {
                        var split = path.Split('/');
                        var tr = GameObject.Find(split[0])?.transform.Find(path[(split[0].Length + 1)..]);
                        if (tr != null)
                            forest = tr.gameObject;
                    }
                    if (forest != null)
                        objectPool.Add(path, forest);
                }

                if (forest == null)
                    continue;
                for (int i = 0; i < forest.transform.childCount; i++)
                {
                    var child = forest.transform.GetChild(i).gameObject;
                    if (!child.name.Contains(_leavesName))
                        continue;
                    child.SetActive(false);
                    leaves.Add(child);
                }
            }
        }

        public static void addleaves()
        {
            foreach (var l in leaves)
                l.SetActive(true);
            leaves.Clear();
        }
    }
}
