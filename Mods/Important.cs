using Breeze.Notifications;
using Photon.Pun;
using UnityEngine;

namespace Breeze.Mods
{
    internal class Important
    {
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
                            if (Vector3.Distance(vrrig.rightHandTransform.position, transform.position) < 0.4 || Vector3.Distance(vrrig.leftHandTransform.position, transform.position) < 0.4 && Time.time > notifcooldown)
                            {
                                NetworkSystem.Instance.ReturnToSinglePlayer();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
