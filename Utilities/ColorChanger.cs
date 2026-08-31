using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Pasted directly from mango
namespace BreezeClient.Utilities
{
    public class TimedBehaviour : MonoBehaviour
    {
        public virtual void Start()
        {
            this.startTime = Time.time;
        }
        public virtual void Update()
        {
            bool flag = !this.complete;
            if (flag)
            {
                this.progress = Mathf.Clamp((Time.time - this.startTime) / this.duration, 0f, 1f);
                bool flag2 = Time.time - this.startTime > this.duration;
                if (flag2)
                {
                    bool flag3 = this.loop;
                    if (flag3)
                    {
                        this.OnLoop();
                    }
                    else
                    {
                        this.complete = true;
                    }
                }
            }
        }
        public virtual void OnLoop()
        {
            this.startTime = Time.time;
        }
        public bool complete = false;
        public bool loop = true;
        public float progress = 0f;
        protected bool paused = false;
        protected float startTime;
        protected float duration = 2f;
    }


    public class ColorChanger : TimedBehaviour
    {
        public override void Start()
        {
            base.Start();
            if (base.GetComponent<Renderer>() != null)
            {
                this.gameObjectRenderer = base.GetComponent<Renderer>();
            }
        }

        public override void Update()
        {
            base.Update();
            bool flag = this.colors != null;
            if (flag)
            {
                bool flag2 = this.timeBased;
                if (flag2)
                {
                    this.color = this.colors.Evaluate(this.progress);
                }
                this.gameObjectRenderer.material.color = this.color;
                this.gameObjectRenderer.material.SetColor("_EmissionColor", this.color);
            }
        }
        public Renderer gameObjectRenderer;
        public Gradient colors = null;
        public Color color;
        public bool timeBased = true;
    }
}