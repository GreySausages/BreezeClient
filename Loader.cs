using System;
using UnityEngine;

namespace Loading
{
    public class Loader
    {
        public static GameObject g = null;
        public static void Load()
        {
            g = new GameObject("wa");
            UnityEngine.Object.DontDestroyOnLoad(g);
            g.AddComponent<Breeze.HarmonyPatches>();
        }
        public static void Unload()
        {
            GameObject.Destroy(g);
            g = null;
        }
    }
}