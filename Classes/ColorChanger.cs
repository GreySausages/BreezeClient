using UnityEngine;

namespace Breeze.Classes
{
    public class ColorChanger : TimedBehaviour
    {
        public static ColorChanger Instance { get; private set; }
        public override void Start()
        {
            base.Start();
            renderer = base.GetComponent<Renderer>();
            Instance = this;
            Update();
        }

        public override void Update()
        {
            base.Update();
            if (colorInfo != null)
            {
                if (!colorInfo.copyRigColors)
                {
                    Color color = new Gradient { colorKeys = colorInfo.colors }.Evaluate((Time.time / 2f) % 1);
                    if (colorInfo.isRainbow)
                    {
                        float h = (Time.frameCount / 360f) % 1f;
                        color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    }
                    renderer.material.color = color;
                }
                else
                {
                    renderer.material = GorillaTagger.Instance.offlineVRRig.mainSkin.material;
                }
            }
        }

        public Renderer renderer;
        public ExtGradient colorInfo;
    }
}
