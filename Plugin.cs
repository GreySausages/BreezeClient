using BepInEx;
using BreezeClient.Backend;
using BreezeClient.UI;
using HarmonyLib;
using Loading;
using System;
using System.IO;
using System.Collections;
using System.Net;
using diddy.hello;
using UnityEngine;
using static BreezeClient.Plugin;
using Object = UnityEngine.Object;

namespace BreezeClient
{
    [BepInPlugin(Name, GUID, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Name = "BreezeClient";
        public const string GUID = "org.breeze.dev.plon";
        public const string Version = "1.0";

        private bool patchedHarmony = false;
        private static GameObject Gameobject;
        [System.Serializable]
        public class LoginData
        {
            public string license;

        }
        void Awake()
        {
            if (!patchedHarmony && Loader.loaded == false)
            {
                Harmony harmony = new Harmony(GUID);
                harmony.PatchAll();
                patchedHarmony = true;
                Loader.loaded = true;
            }
            Application.quitting += DeleteMods;
        }
        private void DeleteMods()
        {
            try
            {
                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Breeze.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Breeze.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Undefined.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Undefined.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Genesis Reborn.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Genesis Reborn.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"GorillaLibrary.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"GorillaLibrary.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Actual Genesis.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Actual Genesis.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Newtonsoft.Json.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Newtonsoft.Json.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Computer Interface.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Computer Interface.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"ShibaGTGenesisReborn.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"ShibaGTGenesisReborn.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"Untitled.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"Untitled.dll"));

                if (File.Exists(Path.Combine(Paths.GameRootPath, $"CubeClient.dll")))
                    File.Delete(Path.Combine(Paths.GameRootPath, $"CubeClient.dll"));
            }
            catch (Exception ex)
            {

            }
        }
    }
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "FixedUpdate")]
    internal class UpdatePatch
    {
        private static bool alreadyInit;
        public static GameObject Gameobject;

        static void Postfix()
        {
           
            if (!alreadyInit)
            {
                alreadyInit = true;
                Gameobject = new GameObject();
                Gameobject.AddComponent<Plugin>();
                Gameobject.AddComponent<WristMenu>();
                Gameobject.AddComponent<RigShit>();
                Gameobject.AddComponent<Mods>();
                Gameobject.AddComponent<GhostPatch>();
                Mods.Load();
                Object.DontDestroyOnLoad(Gameobject);
            }
        }
    }
}
