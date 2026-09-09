using Breeze.Classes;
using Breeze.Patches;
using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag.Cosmetics.Summer;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breeze.Mods
{
    internal class Projectiles
    {
        public class ProjectileEntry
        {
            public string Name;
            public SnowballThrowable ThrowableLeft;
            public SnowballThrowable ThrowableRight;

            public SnowballThrowable Throwable
            {
                get
                {
                    return ThrowableRight;
                }
            }

            public int ThrowableIndex
            {
                get
                {
                    if (Throwable != null)
                    {
                        return Throwable.throwableMakerIndex;
                    }

                    return -1;
                }
            }
        }

        private static ProjectileEntry snowballEntry;
        private static bool isInitializing;

        public static void InitializeSnowball()
        {
            if (snowballEntry != null)
            {
                return;
            }

            if (isInitializing)
            {
                return;
            }

            if (CosmeticsController.instance == null)
            {
                return;
            }

            if (CosmeticsController.instance.v2_allCosmetics == null)
            {
                return;
            }

            isInitializing = true;

            try
            {
                foreach (var info in CosmeticsController.instance.v2_allCosmetics)
                {
                    if (!info.isThrowable)
                    {
                        continue;
                    }

                    if (!info.displayName.Contains(
                        "Snowball",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string rightId;
                    string leftId;

                    bool gotRightId =
                        CosmeticsV2Spawner_Dirty.GetPlayfabIdFromThrowableIndex(
                            false,
                            info.throwableIndex,
                            out rightId);

                    bool gotLeftId =
                        CosmeticsV2Spawner_Dirty.GetPlayfabIdFromThrowableIndex(
                            true,
                            info.throwableIndex,
                            out leftId);

                    if (!gotRightId || !gotLeftId)
                    {
                        continue;
                    }

                    var registry = VRRig.LocalRig?.cosmeticsObjectRegistry;

                    if (registry == null)
                    {
                        continue;
                    }

                    registry.Cosmetic(leftId);
                    registry.Cosmetic(rightId);

                    GrowingSnowballThrowable leftSnowball = null;
                    GrowingSnowballThrowable rightSnowball = null;

                    if (SnowballMaker.leftHandInstance != null)
                    {
                        foreach (var snowball in SnowballMaker.leftHandInstance.snowballs)
                        {
                            if (snowball is GrowingSnowballThrowable)
                            {
                                GrowingSnowballThrowable growingSnowball =
                                    (GrowingSnowballThrowable)snowball;

                                if (snowball.throwableMakerIndex ==
                                    info.throwableIndex)
                                {
                                    leftSnowball = growingSnowball;
                                }
                            }
                        }
                    }

                    if (SnowballMaker.rightHandInstance != null)
                    {
                        foreach (var snowball in SnowballMaker.rightHandInstance.snowballs)
                        {
                            if (snowball is GrowingSnowballThrowable)
                            {
                                GrowingSnowballThrowable growingSnowball =
                                    (GrowingSnowballThrowable)snowball;

                                if (snowball.throwableMakerIndex ==
                                    info.throwableIndex)
                                {
                                    rightSnowball = growingSnowball;
                                }
                            }
                        }
                    }

                    if (leftSnowball != null && rightSnowball != null)
                    {
                        leftSnowball.velocityEstimator =
                            SnowballMaker.leftHandInstance.velocityEstimator;

                        rightSnowball.velocityEstimator =
                            SnowballMaker.rightHandInstance.velocityEstimator;

                        snowballEntry = new ProjectileEntry();

                        snowballEntry.Name = "Growing Snowball";
                        snowballEntry.ThrowableLeft = leftSnowball;
                        snowballEntry.ThrowableRight = rightSnowball;

                        break;
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogError(
                    "InitializeSnowball failed: " + error);
            }

            isInitializing = false;
        }

        public enum ThrowableHand
        {
            Left,
            Right,
            Both,
            Dynamic
        }

        public static void UpdateNetworkedProjectile(
            int index,
            ThrowableHand hand)
        {
            if (hand == ThrowableHand.Left ||
                hand == ThrowableHand.Both)
            {
                VRRig.LocalRig.LeftThrowableProjectileIndex = index;
            }

            if (hand == ThrowableHand.Right ||
                hand == ThrowableHand.Both)
            {
                VRRig.LocalRig.RightThrowableProjectileIndex = index;
            }

            VRRig.LocalRig.myBodyDockPositions
                .RefreshTransferrableItems();
        }

        public static bool biig;

        public static void SendSnowball(
            Vector3 position,
            Vector3 velocity,
            Color? color = null,
            ThrowableHand hand = ThrowableHand.Dynamic)
        {
            try
            {
                if (snowballEntry == null)
                {
                    InitializeSnowball();

                    if (snowballEntry == null)
                    {
                        return;
                    }
                }

                Color32 finalColor;

                if (color.HasValue)
                {
                    finalColor = color.Value;
                }
                else
                {
                    finalColor = Color.white;
                }

                GrowingSnowballThrowable throwable =
                    snowballEntry.ThrowableRight as GrowingSnowballThrowable;

                if (throwable == null)
                {
                    throwable =
                        snowballEntry.Throwable as GrowingSnowballThrowable;
                }

                if (throwable == null)
                {
                    return;
                }

                UpdateNetworkedProjectile(
                    snowballEntry.ThrowableIndex,
                    hand);

                VRRig.LocalRig.SetThrowableProjectileColor(
                    true,
                    finalColor);

                int projectileIndex = GetProjectileIncrement(
                    position,
                    velocity,
                    throwable.transform.lossyScale.x);

                int scale = 0;

                if (!biig)
                {
                    scale = 5;
                }
                else
                {
                    scale = 0;
                }

                if (NetworkSystem.Instance.InRoom)
                {
                    FieldInfo changeSizeField =
                        typeof(GrowingSnowballThrowable).GetField(
                            "changeSizeEvent",
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);

                    FieldInfo snowballThrowField =
                        typeof(GrowingSnowballThrowable).GetField(
                            "snowballThrowEvent",
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);

                    PhotonEvent changeSizeEvent = null;
                    PhotonEvent snowballThrowEvent = null;

                    if (changeSizeField != null)
                    {
                        changeSizeEvent =
                            (PhotonEvent)changeSizeField.GetValue(throwable);
                    }

                    if (snowballThrowField != null)
                    {
                        snowballThrowEvent =
                            (PhotonEvent)snowballThrowField.GetValue(throwable);
                    }

                    if (changeSizeEvent == null ||
                        snowballThrowEvent == null)
                    {
                        return;
                    }

                    FieldInfo eventIdField =
                        typeof(PhotonEvent).GetField(
                            "_eventId",
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);

                    if (eventIdField == null)
                    {
                        return;
                    }

                    int changeSizeId =
                        (int)eventIdField.GetValue(changeSizeEvent);

                    int snowballThrowId =
                        (int)eventIdField.GetValue(snowballThrowEvent);

                    PhotonNetwork.RaiseEvent(
                        PhotonEvent.PHOTON_EVENT_CODE,
                        new object[]
                        {
                            changeSizeId,
                            scale
                        },
                        new RaiseEventOptions
                        {
                            Receivers = ReceiverGroup.All
                        },
                        SendOptions.SendReliable);

                    PhotonNetwork.RaiseEvent(
                        PhotonEvent.PHOTON_EVENT_CODE,
                        new object[]
                        {
                            snowballThrowId,
                            position,
                            velocity,
                            projectileIndex
                        },
                        new RaiseEventOptions
                        {
                            Receivers = ReceiverGroup.All
                        },
                        SendOptions.SendReliable);

                    Move.RPCProt();
                }
                else
                {
                    MethodInfo spawnMethod =
                        typeof(GrowingSnowballThrowable).GetMethod(
                            "SpawnGrowingSnowball",
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);

                    if (spawnMethod == null)
                    {
                        return;
                    }

                    object[] spawnArguments = new object[]
                    {
                        velocity,
                        throwable.snowballSizeLevels[scale].snowballScale
                    };

                    SlingshotProjectile projectile =
                        (SlingshotProjectile)spawnMethod.Invoke(
                            throwable,
                            spawnArguments);

                    if (projectile == null)
                    {
                        return;
                    }

                    Vector3 spawnedVelocity =
                        (Vector3)spawnArguments[0];

                    projectile.Launch(
                        position,
                        spawnedVelocity,
                        VRRig.LocalRig.Creator,
                        false,
                        false,
                        projectileIndex,
                        throwable.snowballSizeLevels[scale].snowballScale,
                        true,
                        finalColor);
                }
            }
            catch (Exception error)
            {
                Debug.LogError(
                    "SendSnowball error: " + error);
            }
        }

        public static int GetProjectileIncrement(
            Vector3 position,
            Vector3 velocity,
            float scale)
        {
            try
            {
                GameObject container =
                    new GameObject("SlingshotProjectileHolder");

                SlingshotProjectile projectile =
                    container.AddComponent<SlingshotProjectile>();

                int projectileIndex = Time.frameCount;

                Type trackerType =
                    typeof(GrowingSnowballThrowable)
                        .Assembly
                        .GetType("ProjectileTracker");

                if (trackerType == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        trackerType =
                            assembly.GetType("ProjectileTracker");

                        if (trackerType != null)
                        {
                            break;
                        }
                    }
                }

                if (trackerType != null)
                {
                    MethodInfo addMethod =
                        trackerType.GetMethod(
                            "AddAndIncrementLocalProjectile",
                            BindingFlags.Public |
                            BindingFlags.Static);

                    if (addMethod != null)
                    {
                        projectileIndex =
                            (int)addMethod.Invoke(
                                null,
                                new object[]
                                {
                                    projectile,
                                    velocity,
                                    position,
                                    scale
                                });
                    }
                }

                GameObject.Destroy(container);

                return projectileIndex;
            }
            catch
            {
                return Time.frameCount;
            }
        }

        public static float cooldown = 0f;

        public static void ProjectileSpam(
            Vector3 position,
            Vector3 velocity,
            Color color)
        {
            if (Time.time > cooldown + 0.7f)
            {
                if (ControllerInputPoller.instance.rightGrab)
                {
                    SendSnowball(
                        position,
                        velocity,
                        color,
                        ThrowableHand.Right);

                    cooldown = Time.time;
                }
            }
        }

        public static VRRig s = Advantages.LockedPlayer;

        public static void SnowballFlingGun()
        {
            VRRig s = Advantages.LockedPlayer;

            Advantages.Gunlib(
                () =>
                {
                    if (s != null)
                    {
                        ProjectileSpam(
                            s.transform.position,
                            -s.transform.up * 10f,
                            Color.white);
                    }
                },
                () =>
                {
                });
        }
    }
}
