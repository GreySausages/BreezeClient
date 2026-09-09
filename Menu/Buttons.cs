using Photon.Pun;
using Breeze.Classes;
using Breeze.Mods;
using static Breeze.Settings;
using static Breeze.Menu.Main;
using UnityEngine;

namespace Breeze.Menu
{
    internal class Buttons
    {
        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[] { // Main Mods
                new ButtonInfo { buttonText = "Settings", method =() => buttonsType = 1, isTogglable = false },
                new ButtonInfo { buttonText = "Important", method =() => buttonsType = 2, isTogglable = false },
                new ButtonInfo { buttonText = "Room", method =() => buttonsType = 3, isTogglable = false },
                new ButtonInfo { buttonText = "Movement", method =() => buttonsType = 4, isTogglable = false },
                new ButtonInfo { buttonText = "Advantages", method =() => buttonsType = 5, isTogglable = false },
                new ButtonInfo { buttonText = "Rig Mods", method =() => buttonsType = 6, isTogglable = false },
                new ButtonInfo { buttonText = "Overpowered", method =() => buttonsType = 7, isTogglable = false },
                new ButtonInfo { buttonText = "Fun", method =() => buttonsType = 8, isTogglable = false },
                new ButtonInfo { buttonText = "Parter Menus", method =() => {buttonsType = 9; pageNumber = 0; }, isTogglable = false },
            },

            new ButtonInfo[] { // Menu Settings [1]
                new ButtonInfo { buttonText = "Return to Settings", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "LockOn", enableMethod =() => Advantages.LockOn = true, disableMethod =() => Advantages.LockOn = false, enabled = Advantages.LockOn},
                new ButtonInfo { buttonText = "Notifications", enableMethod =() => disableNotifications = false, disableMethod =() => disableNotifications = true, enabled = !disableNotifications },
                new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter },
                new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton },
                new ButtonInfo { buttonText = "Rounded Menu", enableMethod =() => roundmenu = true, disableMethod =() => roundmenu = false, enabled = roundmenu },
                new ButtonInfo { buttonText = "Stump text", method =() => Main.ok(), isTogglable = true, enabled = true },
                new ButtonInfo { buttonText = "ChangeTheme", overlapText = "Theme: Dark", method =() => Settings.ThemeChanger(), isTogglable = false, enabled = false },
                new ButtonInfo { buttonText = "pltcolor", overlapText = "Platform Color: Yellow", method =() => Move.ChangePlatColor(), isTogglable = false, enabled = false },
            },

            new ButtonInfo[] { // Important/Safety [2]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Anti Report", method =() => Important.AntiReport(), isTogglable = true, enabled = true },
                new ButtonInfo { buttonText = "RPCProt", method =() => Move.RPCProt(), isTogglable = true, enabled = true },
            },
            new ButtonInfo[] { // Room [3]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Disconnect", method =() => Room.Disconnect(), isTogglable = false },
                new ButtonInfo { buttonText = $"SaveRoom: {Room.room}", method =() => Room.SaveRoom(), isTogglable = false },
                new ButtonInfo { buttonText = "Join Saved Room", method =() => Room.JoinSavedRoom(), isTogglable = false },
            },
            new ButtonInfo[] { // Movement [4]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Platforms", method =() => Move.Platforms(false), isTogglable = true },
                new ButtonInfo { buttonText = "Invis Platforms", method =() => Move.Platforms(true), isTogglable = true },
                new ButtonInfo { buttonText = "Fly [RT]", method =() => Move.CarMonkeyandfly(true), isTogglable = true },
                new ButtonInfo { buttonText = "Noclip [RT]", method =() => Rig.Noclip(), isTogglable = true },
                new ButtonInfo { buttonText = "CarMonkey [RT]", method =() => Move.CarMonkeyandfly(false), isTogglable = true },
                new ButtonInfo { buttonText = "Iron Monkey [RG & LG]", method =() => Move.IronMoneyMonke(), isTogglable = true },
                new ButtonInfo { buttonText = "TPGun", method =() => Move.TPGun(), isTogglable = true },
                new ButtonInfo { buttonText = "WASD Fly", method =() => Move.WASDFly(), isTogglable = true },
            },
            new ButtonInfo[] { // Advantages [5]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Tag Self", method =() => Advantages.TagSelf(), isTogglable = true },
                new ButtonInfo { buttonText = "Tag Gun", method =() => Advantages.TagGun(), isTogglable = true },
                new ButtonInfo { buttonText = "Tag All", method =() => Advantages.TagAll(), isTogglable = true },
                new ButtonInfo { buttonText = "Flick Tag Gun", method =() => Advantages.FlickTagGun(), isTogglable = true },
                new ButtonInfo { buttonText = "Reach", method =() => Advantages.Reach(), disableMethod =() => Advantages.NoReach(), isTogglable = true },
                new ButtonInfo { buttonText = "Remove Leaves", method =() => Advantages.removeleaves(), disableMethod =() => Advantages.addleaves(), isTogglable = true },
                new ButtonInfo { buttonText = "Unlock fps", method =() => { Application.targetFrameRate = int.MaxValue; QualitySettings.vSyncCount = 0; }, disableMethod =() => Application.targetFrameRate = 144, isTogglable = true },
            },
            new ButtonInfo[] { // VRRig [6]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Long Arms", method =() => Move.LongArms(), isTogglable = true },
                new ButtonInfo { buttonText = "Ghost Monkey", method =() => Rig.GhostMonkey(), isTogglable = true },
                new ButtonInfo { buttonText = "Invis Monkey", method =() => Rig.InvisMonkey(), isTogglable = true },
                new ButtonInfo { buttonText = "Grab Rig [RG]", method =() => Move.GrabRig(), isTogglable = true },
            },
            new ButtonInfo[] { // Overpowered/Exploits [7]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Lag Gun [UND]", method =() => Advantages.LagGun(0.5f, 240), isTogglable = true },
                new ButtonInfo { buttonText = "Lag All [UND]", method =() => Advantages.LagAll(0.5f, 240), isTogglable = true },
                new ButtonInfo { buttonText = "Stutter Gun [UND]", method =() => Advantages.LagGun(11f, 3500), isTogglable = true },
                new ButtonInfo { buttonText = "Stutter All [UND]", method =() => Advantages.LagAll(11f, 3500), isTogglable = true },
                new ButtonInfo { buttonText = "Snowball Fling", method =() => Projectiles.SnowballFlingGun(), isTogglable = true },
            },
            new ButtonInfo[] { // Fun/Misc [8]
                new ButtonInfo { buttonText = "Return to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Water Splash Hands", method =() => Fun.WaterSplashHands(), isTogglable = true },
                new ButtonInfo { buttonText = "Metal Spam", method =() => Fun.SoundSpam(), isTogglable = true },
                new ButtonInfo { buttonText = "Crystal Spam", method =() => Fun.SoundSpam(20), isTogglable = true },
            },
            new ButtonInfo[] { // Parter_Menus [9]
                new ButtonInfo { buttonText = "Retrurn to Main", method =() => buttonsType = 0, isTogglable = false },
                new ButtonInfo { buttonText = "Load Genesis", method =() => Mod_Loading.LoadGenesis(), isTogglable = false },
            },
        };
    }
}