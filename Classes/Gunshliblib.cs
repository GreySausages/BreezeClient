using BepInEx;
using Breeze.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace GunlibTemp
{
    internal class Gunlib
    {
        // colors 
        public static Color pointercolor = Color.red;
        public static GradientColorKey[] pointergradient = new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.4f, 0.2f, 0.6f), 0f),
                new GradientColorKey(new Color(0.3f, 0.15f, 0.5f), 0.33f),
                new GradientColorKey(new Color(0.35f, 0.25f, 0.65f), 0.66f),
                new GradientColorKey(new Color(0.4f, 0.2f, 0.6f), 1f)
            };

        public static VRRig LockedPlayer;
        public static GameObject spherepointer;
        public static RaycastHit nray;

        private static float pointerBaseScale = 0.12f;

        private static GameObject trailObj;
        private static LineRenderer trailLine;
        private const int TrailSegments = 32;
        public static float TrailWidth = 0.02f;
        public static float BendStrength = 0.1f;
        private static Vector3 bendOffset = Vector3.zero;
        private static Vector3 bendVelocity = Vector3.zero;
        private static bool isHoldingTrigger;

        public static void StartBothGuns(Action action, bool lockOn)
        {
            if (XRSettings.isDeviceActive) StartVrGun(action, lockOn);
            else StartPcGun(action, lockOn);
        }

        public static void StartVrGun(Action action, bool lockOn)
        {
            bool gripHeld = ControllerInputPoller.instance.rightGrab;
            bool triggerHeld = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;
            if (triggerHeld)
            {
                pointercolor = Color.green;
                if (!isHoldingTrigger)
                {
                    UnityEngine.Object.Destroy(spherepointer);
                    isHoldingTrigger = true;
                }   
                if (!triggerHeld)
                {
                    isHoldingTrigger = false;
                }
            }
            else
            {
                pointercolor = Color.red;
                UnityEngine.Object.Destroy(spherepointer);
            }

            if (!gripHeld)
            {
                Cleanup();
                return;
            }
            Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out nray, float.MaxValue);
            HandleGunLogic(action, lockOn, XR: true, triggerHeld: triggerHeld);
        }
        public static void StartPcGun(Action action, bool lockOn)
        {
            GameObject shoulderObj = GameObject.Find("Shoulder Camera");
            Camera cam = shoulderObj != null ? shoulderObj.GetComponent<Camera>() : null;
            if (cam == null) cam = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            bool gripHeld = Mouse.current.rightButton.isPressed;
            bool triggerPressed = Mouse.current.leftButton.isPressed;
            if (triggerPressed)
            {
                pointercolor = Color.green;
            }
            else
            {
                pointercolor = Color.red;
            }

                Physics.Raycast(ray.origin, ray.direction, out nray, 100f);
            HandleGunLogic(action, lockOn, XR: false, triggerHeld: gripHeld && triggerPressed);
        }

        private static void HandleGunLogic(Action action, bool lockOn, bool XR, bool triggerHeld)
        {
            bool gripHeld = XR ? ControllerInputPoller.instance.rightGrab : UnityInput.Current.GetMouseButton(1);
            if (!gripHeld)
            {
                Cleanup();
                return;
            }
            if (spherepointer == null) CreatePointer();
            if (trailLine == null) CreateTrail();
            Vector3 handPos = GorillaTagger.Instance.rightHandTransform.position;
            Vector3 targetPos = handPos;
            if (lockOn && LockedPlayer != null)
                targetPos = LockedPlayer.transform.position;
            else if (nray.collider != null)
                targetPos = nray.point;
            spherepointer.transform.position = targetPos;
            spherepointer.transform.localScale = Vector3.one * pointerBaseScale;
            UpdateTrail(handPos, targetPos);
            if (triggerHeld && lockOn && LockedPlayer == null)
                LockedPlayer = nray.collider?.GetComponentInParent<VRRig>();
            if (triggerHeld && action != null && (!lockOn || LockedPlayer != null))
                action();
        }

        private static void CreatePointer()
        {
            spherepointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(spherepointer.GetComponent<Collider>());
            spherepointer.transform.localScale = Vector3.zero;

            Color pointerColor = pointercolor;
            Material mat = new Material(Shader.Find("GorillaTag/UberShader"));
            mat.SetColor("_BaseColor", pointerColor);
            mat.SetColor("_EmissionColor", pointerColor);
            mat.EnableKeyword("_EMISSION");
            spherepointer.GetComponent<Renderer>().material = mat;
        }

        private static void CreateTrail()
        {
            trailObj = new GameObject("GunTrail");
            trailObj.hideFlags = HideFlags.HideAndDontSave;

            trailLine = trailObj.AddComponent<LineRenderer>();
            trailLine.positionCount = TrailSegments;
            trailLine.useWorldSpace = true;
            trailLine.receiveShadows = false;
            trailLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            AnimationCurve widthCurve = new AnimationCurve(
                new Keyframe(0.0f, TrailWidth * 0.3f),
                new Keyframe(0.3f, TrailWidth * 1.0f),
                new Keyframe(0.7f, TrailWidth * 1.0f),
                new Keyframe(1.0f, TrailWidth * 0.3f)
            );
            trailLine.widthCurve = widthCurve;
            trailLine.widthMultiplier = 1f;

            Gradient grad = new Gradient();
            grad.SetKeys(
                pointergradient,
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            trailLine.colorGradient = grad;

            Color pointerColor = pointercolor;
            Material mat = new Material(Shader.Find("GUI/Text Shader"));
            mat.SetColor("_BaseColor", pointerColor);
            mat.SetColor("_EmissionColor", pointerColor);
            mat.EnableKeyword("_EMISSION");
            trailLine.material = mat;
        }

        private static void UpdateTrail(Vector3 start, Vector3 end)
        {
            if (trailLine == null) return;

            Vector3 forward = end - start;
            float dist = forward.magnitude;

            if (dist < 0.001f)
            {
                Vector3[] collapsed = new Vector3[TrailSegments];
                for (int i = 0; i < TrailSegments; i++) collapsed[i] = start;
                trailLine.SetPositions(collapsed);
                return;
            }

            Vector3 forwardNorm = forward.normalized;

            Vector3 refUp = (Mathf.Abs(forwardNorm.y) < 0.95f) ? Vector3.up : Vector3.forward;
            Vector3 perpA = Vector3.Cross(forwardNorm, refUp).normalized;
            Vector3 perpB = Vector3.Cross(forwardNorm, perpA).normalized;

            float t = Time.time;
            Vector3 targetBend = perpA * (Mathf.Sin(t * 1.1f) * BendStrength * dist * 0.18f) + perpB * (Mathf.Sin(t * 0.7f + 1.3f) * BendStrength * dist * 0.14f);

            float spring = 8f;
            float damping = 3f;
            float dt = Time.deltaTime;
            bendVelocity += (targetBend - bendOffset) * spring * dt;
            bendVelocity *= (1f - damping * dt);
            bendOffset += bendVelocity * dt;

            Vector3 ctrl1 = start + forward * 0.33f + bendOffset * 0.7f;
            Vector3 ctrl2 = end - forward * 0.33f + bendOffset * 0.4f;

            Vector3[] positions = new Vector3[TrailSegments];
            for (int i = 0; i < TrailSegments; i++)
            {
                float u = i / (float)(TrailSegments - 1);
                positions[i] = CubicBezier(start, ctrl1, ctrl2, end, u);
            }

            trailLine.SetPositions(positions);
        }

        private static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float mt = 1f - t;
            float mt2 = mt * mt;
            float t2 = t * t;
            return mt2 * mt * p0 + 3f * mt2 * t * p1 + 3f * mt * t2 * p2 + t2 * t * p3;
        }

        private static void Cleanup()
        {
            if (spherepointer != null)
                UnityEngine.Object.Destroy(spherepointer);
            spherepointer = null;

            if (trailObj != null)
                UnityEngine.Object.Destroy(trailObj);
            trailObj = null;
            trailLine = null;

            LockedPlayer = null;
            bendOffset = Vector3.zero;
            bendVelocity = Vector3.zero;
        }

        public static void TestGunlib()
        {
            Gunlib.StartBothGuns(() => { }, false);
        }
        public static void TestGunlibLocked()
        {
            Gunlib.StartBothGuns(() => { }, true);
        }
    }
}