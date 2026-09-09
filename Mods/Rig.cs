using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breeze.Mods
{
    internal class Rig
    {
        public static GameObject Orb = null;
        public static GameObject Orb2 = null;
        public static void InvisGhostOrbs()
        {
            Orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Orb.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            Orb.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Object.Destroy(Orb.GetComponent<Rigidbody>());
            Object.Destroy(Orb.GetComponent<Collider>());
            Object.Destroy(Orb.GetComponent<SphereCollider>());
            Orb.GetComponent<Renderer>().material.color = Color.red;
            Orb2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Orb2.transform.position = GorillaTagger.Instance.leftHandTransform.position;
            Orb2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Object.Destroy(Orb2.GetComponent<Rigidbody>());
            Object.Destroy(Orb2.GetComponent<Collider>());
            Object.Destroy(Orb2.GetComponent<SphereCollider>());
            Orb2.GetComponent<Renderer>().material.color = Color.red;
            Object.Destroy(Orb, Time.deltaTime);
            Object.Destroy(Orb2, Time.deltaTime);
        }

        public static void Noclip()
        {
            bool IsNoclipping = ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || Mouse.current.leftButton.isPressed;
            MeshCollider[] colliders = Resources.FindObjectsOfTypeAll<MeshCollider>();

            foreach (MeshCollider collider in colliders) collider.enabled = !IsNoclipping;
        }

        public static void GhostMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                InvisGhostOrbs();
            }
            else
                GorillaTagger.Instance.offlineVRRig.enabled = true;
        }
        public static void LoudHandTaps() => GorillaTagger.Instance.handTapVolume = 999f;
        public static void NormalHandTaps() => GorillaTagger.Instance.handTapVolume = 0.1f;
        public static void FastHandTaps() => GorillaTagger.Instance.tapCoolDown = 999f;
        public static void NormalSpeedHandTaps() => GorillaTagger.Instance.tapCoolDown = 0.15f;
        public static void InvisMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(999f, 999f, 999f);
                InvisGhostOrbs();
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }
    }
}
