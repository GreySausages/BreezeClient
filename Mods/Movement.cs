using Breeze.Classes;
using BepInEx;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GunlibTemp;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Breeze.Menu;

namespace Breeze.Mods
{
    internal class Move
    {
        private static Color PlatColor = Color.yellow;
        private static int PlatColorint;

        private static GameObject PlatL = null;
        private static GameObject PlatR = null;

        private static Renderer rpr;
        private static Renderer lpr;

        private static Material originalMat;

        private static Renderer SetupPlatform(GameObject obj, bool invis) //this is something incharilla added for no reason when i had a non bloated and good working method
        {
            obj.transform.localScale = new Vector3(0.0125f, 0.28f, 0.3825f);

            GameObject.Destroy(obj.GetComponent<Rigidbody>());

            Renderer rend = obj.GetComponent<Renderer>();

            if (originalMat == null)
                originalMat = rend.material;

            rend.material.shader = Shader.Find("GorillaTag/UberShader");
            rend.material.color = PlatColor;

            if (invis)
                GameObject.Destroy(obj.GetComponent<MeshRenderer>());

            return rend;
        }

        public static void IronMoneyMonke()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                GorillaTagger.Instance.rigidbody.AddForce(-GorillaTagger.Instance.leftHandTransform.right * 12f * Time.deltaTime, ForceMode.VelocityChange);
                ShootFire(selfrig.leftHandTransform.position);

            }
            if (ControllerInputPoller.instance.rightGrab)
            {
                GorillaTagger.Instance.rigidbody.AddForce(GorillaTagger.Instance.rightHandTransform.right * 12f * Time.deltaTime, ForceMode.VelocityChange);
                ShootFire(selfrig.rightHandTransform.position);
            }
        }
        public static void UpAndDown()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                GorillaTagger.Instance.rigidbody.AddForce(-GorillaTagger.Instance.transform.up * 12f * Time.deltaTime, ForceMode.VelocityChange);
                ShootFire(selfrig.leftHandTransform.position);

            }
            if (ControllerInputPoller.instance.rightGrab)
            {
                GorillaTagger.Instance.rigidbody.AddForce(GorillaTagger.Instance.transform.up * 12f * Time.deltaTime, ForceMode.VelocityChange);
                ShootFire(selfrig.rightHandTransform.position);
            }
        }

        public static void ShootFire(Vector3 position)
        {
            GameObject fireEffect = new GameObject("FireEffect");
            fireEffect.transform.position = position;

            ParticleSystem fireParticles = fireEffect.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = fireParticles.main;

            mainModule.startColor = new ParticleSystem.MinMaxGradient(Color.orange, Color.black);
            mainModule.startSize = 0.1f;
            mainModule.startSpeed = 2f;
            mainModule.startLifetime = 3f;
            mainModule.loop = true;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
            mainModule.maxParticles = 150;

            ParticleSystemRenderer particleRenderer = fireParticles.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            particleRenderer.material.color = Color.orange;

            ParticleSystem.EmissionModule emission = fireParticles.emission;
            emission.rateOverTime = 10f;

            ParticleSystem.ShapeModule shape = fireParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 20f;
            shape.radius = 0.1f;

            UnityEngine.Object.Destroy(fireEffect, 0.5f);
        }

        public static void RPCProt()
        {
            if (!PhotonNetwork.InRoom) return;
            MonkeAgent.instance.rpcErrorMax = int.MaxValue;
            MonkeAgent.instance.rpcCallLimit = int.MaxValue;
            MonkeAgent.instance.logErrorMax = int.MaxValue;
            MonkeAgent.instance.userRPCCalls.Clear();
            MonkeAgent.instance.userDecayTime = 0f;

            PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
            PhotonNetwork.QuickResends = int.MaxValue;

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public static float Shittymethod;
        public static void LagAllOP(float delay, int howmany)
        {
            if (Time.time > delay)
            {
                for (int i = 0; i < howmany; i++)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(202, new object[]
                    {
                        "AHHHHH"
                    }, new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others
                    },SendOptions.SendUnreliable);
                    RPCProt();
                }

                delay = Time.time + delay;
            }
        }
        public static void LaggunOP(float delay, int howmany)
        {
            Gunlib.StartBothGuns(() =>
            {
                if (Time.time > delay)
                {
                    for (int i = 0; i < howmany; i++)
                    {
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(202, new object[]
                        {
                            "Get Fucked Gorilla Tag"
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[]
                            {
                                Gunlib.LockedPlayer.Creator.ActorNumber
                            }
                        },
                            SendOptions.SendUnreliable
                        );
                        RPCProt();
                    }

                    delay = Time.time + delay;
                }
            }, true);
        }

        private static bool GrabRigButton = ControllerInputPoller.instance.rightGrab;
        public static void GrabRig()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position;
                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }
        public static VRRig selfrig = GorillaTagger.Instance.offlineVRRig;
        public static void LongArms()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        public static void SpeedBoost(float a) => GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = a;

        public static void Platforms(bool invis)
        {
            if (PlatR == null && ControllerInputPoller.instance.rightGrab)
            {
                PlatR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                PlatR.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                PlatR.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                rpr = SetupPlatform(PlatR, invis);
            }

            if (!ControllerInputPoller.instance.rightGrab && PlatR != null)
            {
                GameObject.Destroy(PlatR);
                PlatR = null;
                rpr = null;
            }

            if (PlatL == null && ControllerInputPoller.instance.leftGrab)
            {
                PlatL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                PlatL.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                PlatL.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;

                lpr = SetupPlatform(PlatL, invis);
            }

            if (!ControllerInputPoller.instance.leftGrab && PlatL != null)
            {
                GameObject.Destroy(PlatL);
                PlatL = null;
                lpr = null;
            }
        }

        public static void ChangePlatColor()
        {
            switch (PlatColorint)
            {
                case 0:
                    PlatColor = Color.red;
                    Main.GetIndex("pltcolor").overlapText = "Platform Color: Red";
                    PlatColorint++;
                    break;
                case 1:
                    PlatColor = Color.blue;
                    Main.GetIndex("pltcolor").overlapText = "Platform Color: Blue";
                    PlatColorint++;
                    break;
                case 2:
                    PlatColor = Color.magenta;
                    Main.GetIndex("pltcolor").overlapText = "Platform Color: Magenta";
                    PlatColorint++;
                    break;
                case 3: 
                    PlatColor = Color.green;
                    Main.GetIndex("pltcolor").overlapText = "Platform Color: Green";
                    PlatColorint++;
                    break;
                case 4:
                    PlatColor = Color.yellow;
                    Main.GetIndex("pltcolor").overlapText = "Platform Color: Yellow";
                    PlatColorint++;
                    break;
            }
            if (PlatColorint > 4)
            {
                PlatColorint = 0;
            }
        }

        public static void CarMonkeyandfly(bool isfly)
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.position += GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * 9f;
                if (isfly) GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
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

        private static bool HasTeleported;
        public static void TPGun()
        {
            Advantages.Gunlib(() =>
            {
                if (!HasTeleported)
                {
                    Vector3 pos = Gunlib.spherepointer.transform.position;
                    GorillaLocomotion.GTPlayer.Instance.TeleportTo(pos - GTPlayer.Instance.bodyCollider.transform.position + GTPlayer.Instance.transform.position, GTPlayer.Instance.transform.rotation, false);
                    VRRig.LocalRig.transform.position = pos;
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                    HasTeleported = true;
                }
            }, () =>
            {
                HasTeleported = false;
            });
        }
    }
}
