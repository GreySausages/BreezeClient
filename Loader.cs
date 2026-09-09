using System;
using UnityEngine;

namespace Loading
{
    public class Loader
    {
        public static void Load()
        {
            GameObject go = new GameObject("wa");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<Breeze.HarmonyPatches>();
        }
    }
}