using BepInEx;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using static Breeze.Patches.Menu;

namespace Breeze
{
    [Description(Breeze.PluginInfo.Description)]
    [BepInPlugin(Breeze.PluginInfo.GUID, Breeze.PluginInfo.Name, Breeze.PluginInfo.Version)]
    public class HarmonyPatches : BaseUnityPlugin
    {
        private void OnEnable()
        {
            ApplyHarmonyPatches();
            Application.quitting += DeletePartnerMenus;
        }

        public static void DeletePartnerMenus()
        {
            if (File.Exists(Path.Combine(Paths.GameRootPath, "Genesis.dll")))
                File.Delete(Path.Combine(Paths.GameRootPath, "Genesis.dll"));

            if (File.Exists(Path.Combine(Paths.GameRootPath, "Breeze.dll")))
                File.Delete(Path.Combine(Paths.GameRootPath, "Breeze.dll"));
        }

        private void OnDisable()
        {
            RemoveHarmonyPatches();
        }
    }
}
