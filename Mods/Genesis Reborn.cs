using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using BepInEx;
using UnityEngine.Networking;
using System.Reflection;
using System.Linq;
using PlayFab.ClientModels;

namespace Breeze.Mods
{
    public class Mod_Loading : MonoBehaviour
    {
        public static Mod_Loading Instance;
        public static IEnumerator DownloadAndLoadMod(string DownloadLink, string name = "Genesis")
        {
            string path = Path.Combine(Paths.GameRootPath + $"{name}.dll");

            using (UnityWebRequest a = UnityWebRequest.Get(DownloadLink))
            {
                yield return a.SendWebRequest();

                byte[] data = a.downloadHandler.data;

                File.WriteAllBytes(path, data);

                Assembly ass = Assembly.Load(data);
                Type type = ass.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(BaseUnityPlugin)) && !t.IsAbstract);
                GameObject obj = new GameObject(name);
                DontDestroyOnLoad(obj);
                obj.AddComponent(type);
            }
        }

        public static void LoadGenesis()
        {
            if (Instance == null)
            {
                var obj = new GameObject("Load_Genesis");
                DontDestroyOnLoad(obj);
                Instance = obj.AddComponent<Mod_Loading>();
            }
            Instance.StartCoroutine(DownloadAndLoadMod("https://github.com/incharilla1/ShibaGT-Genesis-Reborn/releases/latest/download/ShibaGTGenesisReborn.dll"));
        }
    }
}
