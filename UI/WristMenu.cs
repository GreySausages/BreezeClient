using BepInEx;
using BreezeClient.Backend;
using BreezeClient.UI;
using BreezeClient.Utilities;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

//Template is based off mango
namespace BreezeClient.UI
{
    public class ButtonInfo
    {
        public string buttonText = "Error";
        public string overlapText = null;
        public Action method = null;
        public Action disableMethod = null;
        public bool? enabled = false;
        public string toolTip = "";
        public bool Fav = false;
    }

    internal class WristMenu : MonoBehaviour
    {

        public static PhotonView rig2view(VRRig p)
        {
            return p.netView.punView;
        }

        //settings
        public static List<ButtonInfo> settingsbuttons = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Settings", method =() => Mods.Settings(), enabled = false},
            new ButtonInfo { buttonText = "First Person Camera", method =() => Mods.fpc(), disableMethod =() => Mods.fpcoff(), enabled = false},
            new ButtonInfo { buttonText = "Gunlock", method =() => Mods.Gunlock(), disableMethod =() => Mods.NoGunlock(), enabled = true},
            new ButtonInfo { buttonText = "Gunlib", method =() => Mods.EmptyGunlib(), enabled = false},
            new ButtonInfo { buttonText = "Change Plat Color", overlapText = "Platform Color: Blue", method =() => Mods.PlatColorChange(), enabled = false},
            new ButtonInfo { buttonText = "Fly Speed", overlapText = "Fly Speed: Slow", method =() => Mods.FlySpeedChange(), enabled = false},
            new ButtonInfo { buttonText = "Car Speed", overlapText = "Car Monkey Speed: Slow", method =() => Mods.CarSpeedChange(), enabled = false},
            new ButtonInfo { buttonText = "DC Button", method =() => dc = true, disableMethod =() => dc = false, enabled = true},
        };
        //norma

        public static List<ButtonInfo> buttons = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Settings", method =() => Mods.Settings(), enabled = false},
            new ButtonInfo { buttonText = "OP Mods", method =() => Mods.Op(), enabled = false},
            new ButtonInfo { buttonText = "Movement Mods", method =() => Mods.Movement(), enabled = false},
            new ButtonInfo { buttonText = "Rig Mods", method =() => Mods.Rig(), enabled = false},
            new ButtonInfo { buttonText = "Tag Mods", method =() => Mods.Tag(), enabled = false},
            new ButtonInfo { buttonText = "Visual Mods", method =() => Mods.Vis(), enabled = false},
            new ButtonInfo { buttonText = "Safety Mods", method =() => Mods.Safety(), enabled = false},
            new ButtonInfo { buttonText = "Music", method =() => Mods.Favo(), enabled = false},
            new ButtonInfo { buttonText = "Other Mods", method =() => Mods.OtherMenus(), enabled = false},
        };

        public static List<ButtonInfo> OP = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "OP Mods", method =() => Mods.Op(), enabled = false},
            new ButtonInfo { buttonText = "Lag Gun", method =() => Mods.LagGun(0.5f, 240), enabled = false, toolTip = "Lags QUEST Players!"},
            new ButtonInfo { buttonText = "Lag All", method =() => Mods.LagAll(0.5f, 240), enabled = false, toolTip = "Lags QUEST Players!"},
            new ButtonInfo { buttonText = "Lag Gun v2", method =() => Mods.LagGun(3f, 1000), enabled = false, toolTip = "Lags QUEST Players!"},
            new ButtonInfo { buttonText = "Lag All v2", method =() => Mods.LagAll(3f, 1000), enabled = false, toolTip = "Lags QUEST Players!"},
            new ButtonInfo { buttonText = "Crash Gun", method =() => Mods.LagGun(8f, 3500), enabled = false, toolTip = "Lags QUEST Players!"},
            new ButtonInfo { buttonText = "Crash All", method =() => Mods.LagAll(8f, 3500), enabled = false, toolTip = "Lags QUEST Players!"},
        };
        public static List<ButtonInfo> Movement = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Movement Mods", method =() => Mods.Movement(), enabled = false},
            new ButtonInfo { buttonText = "Platforms", method =() => Mods.Platforms(false), enabled = false},
            new ButtonInfo { buttonText = "TP Gun", method =() => Mods.ProcessTeleportGun(), enabled = false},
            new ButtonInfo { buttonText = "Fly (A)", method =() => Mods.CarMonkeyandfly(Mods.FlySpeed, true), enabled = false},
            new ButtonInfo { buttonText = "Car Monkey (A)", method =() => Mods.CarMonkeyandfly(Mods.CarSpeed, false), enabled = false},
            new ButtonInfo { buttonText = "Noclip", method =() => Mods.Noclip(), enabled = false},
            new ButtonInfo { buttonText = "SpeedBoost", method =() => Mods.SpeedBoost(9f), enabled = false},
            new ButtonInfo { buttonText = "MosaBoost", method =() => Mods.SpeedBoost(7f), enabled = false},
            new ButtonInfo { buttonText = "Invis Plats", method =() => Mods.Platforms(true), enabled = false},
            new ButtonInfo { buttonText = "Up And Down", method =() => Mods.UpAndDown(), enabled = false},
            new ButtonInfo { buttonText = "Iron Monke", method =() => Mods.IronMoneyMonke(), enabled = false},
            new ButtonInfo { buttonText = "WASD Fly", method =() => Mods.WASDFly(), enabled = false},
        };
        public static List<ButtonInfo> Rig = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Rig Mods", method =() => Mods.Rig(), enabled = false},
            new ButtonInfo { buttonText = "Ghost Monkey", method =() => Mods.GhostMonkey(), enabled = false},
            new ButtonInfo { buttonText = "Invis Monkey", method =() => Mods.InvisMonkey(), enabled = false},
            new ButtonInfo { buttonText = "Bug Gun", method =() => Mods.BugGun(), enabled = false},
        };
        public static List<ButtonInfo> Tag = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Tag Mods", method =() => Mods.Tag(), enabled = false},
            new ButtonInfo { buttonText = "Tag All", method =() => Mods.TagAll(), enabled = false},
            new ButtonInfo { buttonText = "Tag Gun", method =() => Mods.TagGun(), enabled = false},
            new ButtonInfo { buttonText = "Tag Self", method =() => Mods.TagSelf(), enabled = false},
            new ButtonInfo { buttonText = "No Tag On Join", method =() => Mods.NoTagOnJoin(), enabled = false},
        };
        public static List<ButtonInfo> Safety = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Safety Mods", method =() => Mods.Safety(), enabled = false},
            new ButtonInfo { buttonText = "Anti Report", method =() => Mods.AntiReport(), enabled = true},
        };
        public static List<ButtonInfo> Visual = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Visual Mods", method =() => Mods.Vis(), enabled = false},
            new ButtonInfo { buttonText = "RGB Monkey (Stump)", method =() => Mods.RGB(), enabled = false},
            new ButtonInfo { buttonText = "Chams", method =() => Mods.FullBodyESP(), disableMethod =() => Mods.DisableFullBodyESP(), enabled = false},
            new ButtonInfo { buttonText = "Name Tags", method =() => Mods.NameTags(), enabled = false},
            new ButtonInfo { buttonText = "Name Tag Gun", method =() => Mods.NameTagGun(), enabled = false},
            new ButtonInfo { buttonText = "Unity Cube", method =() => Mods.Cube(), enabled = false},
        };
        public static List<ButtonInfo> Music = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Music", method =() => Mods.Favo(), enabled = false},
            new ButtonInfo { buttonText = "Play | Pause", method =() => Mods.PlayPause(), enabled = false},
            new ButtonInfo { buttonText = "Next", method =() => Mods.Next(), enabled = false},
            new ButtonInfo { buttonText = "Previous", method =() => Mods.Previous(), enabled = false},
        };
        public static List<ButtonInfo> Genesis = new List<ButtonInfo>
        {
            new ButtonInfo { buttonText = "Other Mods", method =() => Mods.OtherMenus(), enabled = false},
            new ButtonInfo { buttonText = "Genesis Reborn", method =() => Mods.LoadGenesis.LoadGenesisReborn(), enabled = false},
            new ButtonInfo { buttonText = "Untitled", method =() => Mods.LoadGenesis.LoadUntitled(), enabled = false},
            new ButtonInfo { buttonText = "Undefined", method =() => Mods.LoadGenesis.LoadUndefined(), enabled = false},
            new ButtonInfo { buttonText = "Real Genesis (D)", method =() => Mods.LoadGenesis.LoadGenesisReal(), enabled = false},
            new ButtonInfo { buttonText = "Genesis Reborn (Plon)", method =() => Mods.LoadGenesis.LoadGenesisPlon(), enabled = false},
            new ButtonInfo { buttonText = "Cube Client", method =() => Mods.LoadGenesis.LoadCubeClient(), enabled = false},
            new ButtonInfo { buttonText = "Parrot Client", method =() => Mods.LoadGenesis.LoadParrotClient(), enabled = false},
            new ButtonInfo { buttonText = "Unload Mods (Restarts Game)", method =() => Mods.RestartGame(), enabled = false},
        };
        public static List<List<ButtonInfo>> what = new List<List<ButtonInfo>>
        {
            new List<ButtonInfo>
            {

            },
            new List<ButtonInfo>
            {
                
            },
        };

        public static bool arraylisthit = false;
        void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, DrawWindow, "Breeze Client");
            if (showInputWindow)
            {
                inputWindowRect = GUI.Window(1, inputWindowRect, DrawInputWindow, "Room Joiner");
            }
            if (h > maxh) h = 0;
            if (h < 0) h = maxh;
        }

        private Rect windowRect = new Rect(20, 20, 300, 335);
        private int h = 0;
        private int maxh = 1;

        private bool showInputWindow = false;
        private string inputText = "";
        private Rect inputWindowRect = new Rect(330, 20, 300, 150);

        private void DrawWindow(int windowID)
        {
            if ((!Mods.inOp && !Mods.inMovement) && (!Mods.inTag && !Mods.inSettings))
            {

                windowRect.height = 245;
                if (GUI.Button(new Rect(20, 40, 260, 40), "Misc"))
                {
                    Mods.inSettings = true;
                }

                if (GUI.Button(new Rect(20, 90, 260, 40), "Movement"))
                {
                    Mods.inMovement = !Mods.inMovement;
                }

                if (GUI.Button(new Rect(20, 140, 260, 40), "Tag Mods"))
                {
                    Mods.inTag = true;
                }

                if (GUI.Button(new Rect(20, 190, 260, 40), "OP"))
                {
                    Mods.inOp = !Mods.inOp;
                }

            }

            if (Mods.inOp)
            {
                windowRect.height = 335;

                switch (h)
                {
                    case 0:

                        if (GUI.Button(new Rect(20, 40, 260, 40), "Home"))
                        {
                            Mods.inOp = false;
                            windowRect.height = 245;
                        }

                        if (GUI.Button(new Rect(20, 90, 260, 40), $"Lag Gun: {GetButton1("Lag Gun").enabled}"))
                        {
                            GetButton1("Lag Gun").enabled = !GetButton1("Lag Gun").enabled;
                        }

                        if (GUI.Button(new Rect(20, 140, 260, 40), $"Lag All: {GetButton1("Lag All").enabled}"))
                        {
                            GetButton1("Lag All").enabled = !GetButton1("Lag All").enabled;
                        }

                        if (GUI.Button(new Rect(20, 190, 260, 40), $"Crash Gun: {GetButton1("Crash Gun").enabled}"))
                        {
                            GetButton1("Crash Gun").enabled = !GetButton1("Crash Gun").enabled;
                        }
                        break;

                    case 1:

                        if (GUI.Button(new Rect(20, 40, 260, 40), $"Crash All: {GetButton1("Crash All").enabled}"))
                        {
                            GetButton1("Crash All").enabled = !GetButton1("Crash All").enabled;
                        }

                        if (GUI.Button(new Rect(20, 90, 260, 40), "Placeholder"))
                        {

                        }

                        if (GUI.Button(new Rect(20, 140, 260, 40), "Placeholder"))
                        {
                        }

                        if (GUI.Button(new Rect(20, 190, 260, 40), "Placeholder"))
                        {
                        }

                        break;
                }

                if (GUI.Button(new Rect(20, 240, 260, 40), "<"))
                {
                    h--;
                }

                if (GUI.Button(new Rect(20, 290, 260, 40), ">"))
                {
                    h++;
                }
            }

            if (Mods.inMovement)
            {
                if (GUI.Button(new Rect(20, 40, 260, 40), "Home"))
                {
                    Mods.inMovement = false;
                }

                if (GUI.Button(new Rect(20, 90, 260, 40), $"WASD Fly: {GetButton1("WASD Fly").enabled}"))
                {
                    GetButton1("WASD Fly").enabled = !GetButton1("WASD Fly").enabled;
                }

                if (GUI.Button(new Rect(20, 140, 260, 40), $"TP Gun: {GetButton1("TP Gun").enabled}"))
                {
                    GetButton1("TP Gun").enabled = !GetButton1("TP Gun").enabled;
                }

                if (GUI.Button(new Rect(20, 190, 260, 40), $"Iron Monke: {GetButton1("Iron Monke").enabled}"))
                {
                    GetButton1("Iron Monke").enabled = !GetButton1("Iron Monke").enabled;
                }
            }

            if (Mods.inTag)
            {
                if (GUI.Button(new Rect(20, 40, 260, 40), "Home"))
                {
                    Mods.inTag = false;
                }

                if (GUI.Button(new Rect(20, 90, 260, 40), $"Tag Gun: {GetButton1("Tag Gun").enabled}"))
                {
                    GetButton1("Tag Gun").enabled = !GetButton1("Tag Gun").enabled;
                }

                if (GUI.Button(new Rect(20, 140, 260, 40), $"Tag All: {GetButton1("Tag All").enabled}"))
                {
                    GetButton1("Tag All").enabled = !GetButton1("Tag All").enabled;
                }

                if (GUI.Button(new Rect(20, 190, 260, 40), "Placeholder"))
                {
                }
            }

            if (Mods.inSettings)
            {
                windowRect.height = 335;

                if (GUI.Button(new Rect(20, 40, 260, 40), "Home"))
                {
                    Mods.inSettings = false;
                    windowRect.height = 245;
                }

                if (GUI.Button(new Rect(20, 90, 260, 40), $"Gunlock: {GetButton1("Gunlock").enabled}"))
                {
                    GetButton1("Gunlock").enabled = !GetButton1("Gunlock").enabled;
                }

                if (GUI.Button(new Rect(20, 140, 260, 40), $"Room Joiner: { showInputWindow}"))
                {
                    showInputWindow = !showInputWindow;
                }

                if (GUI.Button(new Rect(20, 190, 260, 40), $"Anti Report: {GetButton1("Anti Report").enabled}"))
                {
                    GetButton1("Anti Report").enabled = !GetButton1("Anti Report").enabled;
                }
            }

            GUI.DragWindow(new Rect(0, 0, 300, 30));
        }

        private void DrawInputWindow(int windowID)
        {
            GUI.Label(new Rect(20, 30, 260, 25), "Enter Room");

            inputText = GUI.TextField(new Rect(20, 60, 260, 30), inputText);

            if (GUI.Button(new Rect(20, 100, 260, 30), "Join Room"))
            {
                PhotonNetworkController.Instance.AttemptToAutoJoinSpecificRoom(inputText, GorillaNetworking.JoinType.Solo);
            }

            GUI.DragWindow(new Rect(0, 0, 300, 25));
        }

        public static void Cube()
        {
            Mods.Cube();
        }

        public static ButtonInfo GetButton1(string buttonText)
        {
            List<List<ButtonInfo>> currentList = GetAllLists();

            foreach (List<ButtonInfo> button1 in currentList)
            {
                foreach (ButtonInfo button in button1)
                    if (button.buttonText == buttonText) return button;
            }
            return null;
        }
        public static ButtonInfo GetButton(string buttonText)
        {
            List<ButtonInfo> currentList = GetCurrentList();

                foreach (ButtonInfo button in currentList)
                    if (button.buttonText == buttonText) return button;
            return null;
        }


        
        public static bool ChangingColors = true;
        public static Color FirstColor = Color.blue;
        public static Color SecondColor = Color.magenta;


        public static Color NormalColor = Color.black;


        public static Color ButtonColorDisable = Color.yellow;
        public static Color ButtonColorEnabled = Color.magenta;
        public static Color ButtonTextColor = Color.black;


        public static string MenuTitle = "Breeze Client";


        private static int lastPressedButtonIndex = -1;
        public static GameObject menu = null;
        private static GameObject canvasObj = null;
        private static GameObject reference = null;
        private static int pageSize = 4;
        public static int pageNumber = 0;
        public static bool gripDownR;
        public static bool triggerDownR;
        public static bool abuttonDown;
        public static bool bbuttonDown;
        public static bool xbuttonDown;
        public static bool gripDownL;
        public static bool triggerDownL;
        public static bool joystickR;
        public static bool joystickL;
        public static Vector2 joystickaxisR;
        public static WristMenu instance = new WristMenu();
        public static GameObject menuObj;
        public static Color colorToFade1 = Color.black;
        public static int selectedButton = 1;
        public static Color colorToFade2 = Color.blue;
        private static Text tooltipText;
        private static string tooltipString;
        public static bool toggle = false;
        public static bool toggle1 = false;
        public static bool toggle2 = false;
        public static bool toggle3 = false;
        public static bool toggle4 = false;
        public static bool dc = false;
        public static bool hasPanel = false;

        public static string CheckSelectedButton()
        {
            List<ButtonInfo> currentList = GetCurrentList();

            int index = pageNumber * pageSize + selectedButton;

            if (index < 0 || index >= currentList.Count)
                return null;

            return currentList[index].buttonText;
        }

        public static List<List<ButtonInfo>> GetAllLists()
        {
            return new List<List<ButtonInfo>>
            {
               buttons,
               settingsbuttons,
               OP,
               Movement,
               Rig,
               Tag,
               Safety,
               Visual,
               Music,
               Genesis,
            };
        }

        static bool fun = false;
        public static bool vr = false;
        void Update()
        {
            try
            {
                gripDownL = ControllerInputPoller.instance.leftGrab;
                vr = !Mouse.current.rightButton.isPressed;
                gripDownR = vr ? ControllerInputPoller.instance.rightGrab : Mouse.current.rightButton.isPressed;
                triggerDownL = ControllerInputPoller.instance.leftControllerIndexFloat == 1f;
                triggerDownR = vr ? ControllerInputPoller.instance.rightControllerIndexFloat == 1f : Mouse.current.leftButton.isPressed;
                abuttonDown = ControllerInputPoller.instance.rightControllerPrimaryButton;
                bbuttonDown = ControllerInputPoller.instance.rightControllerSecondaryButton;
                xbuttonDown = ControllerInputPoller.instance.leftControllerPrimaryButton;
                joystickaxisR = ControllerInputPoller.instance.rightControllerPrimary2DAxis;
                if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                {
                    if (menu == null)
                    {
                        instance.Draw();
                    }
                    else
                    {
                        menu.transform.position = GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.position;
                        menu.transform.rotation = GorillaLocomotion.GTPlayer.Instance.LeftHand.controllerTransform.rotation;
                        if (reference == null)
                        {
                            reference = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            GradientColorKey[] array = new GradientColorKey[4];
                            array[0].color = FirstColor;
                            array[0].time = 0f;
                            array[1].color = FirstColor;
                            array[1].time = 0.3f;
                            array[2].color = SecondColor;
                            array[2].time = 0.6f;
                            array[3].color = FirstColor;
                            array[3].time = 1f;
                            ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
                            colorChanger.colors = new Gradient
                            {
                                colorKeys = array
                            };
                            colorChanger.Start();
                            reference.name = "buttonPresser";
                        }
                        reference.transform.parent = GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform;
                        reference.transform.localPosition = new Vector3(0f, -0.1f, 0f) * GorillaLocomotion.GTPlayer.Instance.scale;
                        reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f) * GorillaLocomotion.GTPlayer.Instance.scale;
                    }
                }
                else if (!ControllerInputPoller.instance.leftControllerPrimaryButton && menu != null)
                {
                    Object.Destroy(menu);
                    Object.Destroy(canvasObj);
                    Object.Destroy(reference);
                    menu = null;
                    menuObj = null;
                    canvasObj = null;
                    reference = null;
                    Debug.Log("Closed Breezed Mod Menu");
                }

                //button clicking thingys

                foreach (List<ButtonInfo> list in GetAllLists())
                {
                    foreach (ButtonInfo buttonInfo in list)
                    {
                        if (buttonInfo.method == null)
                            continue;

                        if (buttonInfo.enabled == true)
                        {
                            buttonInfo.method.Invoke();
                        }

                        if (buttonInfo.enabled == false && buttonInfo.disableMethod != null)
                        {
                            buttonInfo.disableMethod.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                Debug.LogException(ex);
            }
        }

        bool sentbefore = false;

        private static string GetButtonTooltip(int index)
        {
            List<ButtonInfo> currentList = GetCurrentList();


            if (index >= 0 && index < currentList.Count)
            {
                ButtonInfo buttonInfo = currentList[index];
                return $"{buttonInfo.buttonText}: {buttonInfo.toolTip}";
            }

            return null;
        }

        public void Draw()
        {
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(menu.GetComponent<Rigidbody>());
            Object.Destroy(menu.GetComponent<BoxCollider>());
            Object.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f) * 1f;
            menuObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(menuObj.GetComponent<Rigidbody>());
            Object.Destroy(menuObj.GetComponent<BoxCollider>());
            menuObj.transform.parent = menu.transform;
            menuObj.transform.rotation = Quaternion.identity;
            menuObj.transform.localScale = new Vector3(0.1f, 1f, 1f) * 1f;
            if (ChangingColors)
            {
                GradientColorKey[] array = new GradientColorKey[4];
                array[0].color = FirstColor;
                array[0].time = 0f;
                array[1].color = FirstColor;
                array[1].time = 0.3f;
                array[2].color = SecondColor;
                array[2].time = 0.6f;
                array[3].color = FirstColor;
                array[3].time = 1f;
                ColorChanger colorChanger = menuObj.AddComponent<ColorChanger>();
                colorChanger.colors = new Gradient
                {
                    colorKeys = array
                };
                colorChanger.Start();
            }
            else
            {
                menuObj.GetComponent<Renderer>().material.color = NormalColor;
            }
            menuObj.transform.position = new Vector3(0.05f, 0f, 0f) * 1f;
            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;
            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text.gameObject.name = "name";
            titiel = text;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            int yau = pageNumber + 1;
            text.text = MenuTitle;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            component.position = new Vector3(0.06f, 0f, 0.175f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            AddPageButtons();

            List<ButtonInfo> currentList = GetCurrentList();

            string[] pageButtons = currentList
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(button => button.buttonText)
                .ToArray();

            for (int i = 0; i < pageButtons.Length; i++)
            {
                AddButton(i * 0.13f + 0.26f, pageButtons[i]);
            }
            GameObject tooltipObj = new GameObject();
            tooltipObj.transform.SetParent(canvasObj.transform);
            tooltipObj.transform.localPosition = new Vector3(0, 0, 1) * 1f;

            tooltipText = tooltipObj.GetComponent<Text>();
            if (tooltipText == null)
                tooltipText = tooltipObj.AddComponent<Text>();
            tooltipText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            tooltipText.text = "";
            tooltipText.fontSize = 20;
            tooltipText.alignment = TextAnchor.MiddleCenter;
            tooltipText.resizeTextForBestFit = true;
            tooltipText.resizeTextMinSize = 0;
            tooltipText.color = ButtonTextColor;

            RectTransform componenttooltip = tooltipObj.GetComponent<RectTransform>();
            componenttooltip.localPosition = Vector3.zero;
            componenttooltip.sizeDelta = new Vector2(0.2f, 0.03f) * 1f;
            componenttooltip.position = new Vector3(0.06f, 0f, -0.18f) * 1f;
            componenttooltip.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (dc)
            {
                GameObject gameObject6 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject6.name = "disconnect";
                gameObject6.GetComponent<BoxCollider>().isTrigger = true;
                gameObject6.transform.parent = WristMenu.menu.transform;
                gameObject6.transform.localPosition = new Vector3(0.56f, -1f, 0.19f);
                gameObject6.transform.localScale = new Vector3(0.045f, 0.66f, 0.17f);
                gameObject6.AddComponent<BtnCollider>().relatedText = "DisconnectingButton";
                gameObject6.GetComponent<Renderer>().material.color = Color.red;
                Text text3 = new GameObject
                {
                    name = "disconnecting text",
                    transform =
                {
                    parent = WristMenu.canvasObj.transform
                }
                }.AddComponent<Text>();
                text3.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text3.text = "Disconnect";
                text3.fontSize = 1;
                text3.alignment = TextAnchor.MiddleCenter;
                text3.resizeTextForBestFit = true;
                text3.resizeTextMinSize = 0;
                RectTransform rect = text3.GetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.sizeDelta = new Vector2(1f, 0.03f);
                rect.localPosition = new Vector3(0.07f, -0.297f, 0.08f);
                rect.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
        }

        public static Text titiel;

        public static void DisableButton(string buttonText)
        {
            List<ButtonInfo> currentList = GetCurrentList();

            foreach (ButtonInfo btninfo in currentList)
            {
                if (btninfo.buttonText == buttonText)
                {
                    btninfo.enabled = false;
                    Mods.UpdateMenu();
                    return;
                }
            }
        }
        private static void AddPageButtons()
        {
            List<ButtonInfo> currentList = GetCurrentList();


            int num = (currentList.Count + pageSize - 1) / pageSize;

            if (pageNumber >= num)
                pageNumber = 0;

            int num2 = pageNumber + 1;
            int num3 = pageNumber - 1;

            if (num2 >= num)
                num2 = 0;

            if (num3 < 0)
                num3 = num - 1;

            float num4 = 0f;
            GameObject gameObject = GameObject.CreatePrimitive((PrimitiveType)3);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f) * GorillaLocomotion.GTPlayer.Instance.scale;
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num4) * GorillaLocomotion.GTPlayer.Instance.scale;
            gameObject.AddComponent<BtnCollider>().relatedText = "PreviousPage";
            gameObject.GetComponent<Renderer>().material.color = ButtonColorDisable;
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = ButtonColorDisable;
            array[0].time = 0f;
            array[1].color = ButtonColorDisable;
            array[1].time = 0.5f;
            array[2].color = ButtonColorDisable;
            array[2].time = 1f;
            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = new Gradient
            {
                colorKeys = array
            };
            colorChanger.Start();
            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text.font = (Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font);
            text.text = "[" + num3.ToString() + "] << Prev";
            text.color = Color.black;
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f) * GorillaLocomotion.GTPlayer.Instance.scale;
            component.localPosition = new Vector3(0.064f, 0f, 0.111f - num4 / 2.55f) * GorillaLocomotion.GTPlayer.Instance.scale;
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            num4 = 0.13f;
            GameObject gameObject2 = GameObject.CreatePrimitive((PrimitiveType)3);
            Object.Destroy(gameObject2.GetComponent<Rigidbody>());
            gameObject2.GetComponent<BoxCollider>().isTrigger = true;
            gameObject2.transform.parent = menu.transform;
            gameObject2.transform.rotation = Quaternion.identity;
            gameObject2.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f) * GorillaLocomotion.GTPlayer.Instance.scale;
            gameObject2.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num4);
            gameObject2.AddComponent<BtnCollider>().relatedText = "NextPage";
            gameObject2.GetComponent<Renderer>().material.color = ButtonColorDisable;
            GradientColorKey[] array2 = new GradientColorKey[3];
            array2[0].color = ButtonColorDisable;
            array2[0].time = 0f;
            array2[1].color = ButtonColorDisable;
            array2[1].time = 0.5f;
            array2[2].color = ButtonColorDisable;
            array2[2].time = 1f;
            ColorChanger colorChanger2 = gameObject2.AddComponent<ColorChanger>();
            colorChanger2.colors = new Gradient
            {
                colorKeys = array2
            };
            colorChanger2.Start();
            Text text2 = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();
            text2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text2.text = "Next >> [" + num2.ToString() + "]";
            text2.color = Color.black;
            text2.fontSize = 1;
            text2.alignment = TextAnchor.MiddleCenter;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;
            RectTransform component2 = text2.GetComponent<RectTransform>();
            component2.localPosition = Vector3.zero;
            component2.sizeDelta = new Vector2(0.2f, 0.03f) * GorillaLocomotion.GTPlayer.Instance.scale;
            component2.localPosition = new Vector3(0.064f, 0f, 0.111f - num4 / 2.55f);
            component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }
        public static void DestroyMenu()
        {
            if (menu != null)
            {
                Object.Destroy(menu);
                Object.Destroy(canvasObj);
                Object.Destroy(reference);
                menu = null;
                menuObj = null;
                canvasObj = null;
                reference = null;
                Debug.Log("Closed Breezed Mod Menu");
            }
        }
        private static void AddButton(float offset, string text)
        {
            GameObject gameObject = GameObject.CreatePrimitive((PrimitiveType)3);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);

            gameObject.AddComponent<BtnCollider>().relatedText = text;

            List<ButtonInfo> currentList = GetCurrentList();


            int num = -1;

            for (int i = 0; i < currentList.Count; i++)
            {
                if (text == currentList[i].buttonText)
                {
                    num = i;
                    break;
                }
            }


            Text text2 = new GameObject
            {
                transform =
                {
                    parent = canvasObj.transform
                }
            }.AddComponent<Text>();

            text2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text2.text = text;
            text2.fontSize = 1;
            text2.alignment = TextAnchor.MiddleCenter;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;


            if (num != -1 && currentList[num].overlapText != null)
            {
                text2.text = currentList[num].overlapText;
            }


            RectTransform component = text2.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.55f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (num != -1)
            {
                if (currentList[num].enabled == true)
                {
                    gameObject.GetComponent<Renderer>().material.color = ButtonColorEnabled;
                    text2.color = ButtonTextColor;
                }
                else
                {
                    gameObject.GetComponent<Renderer>().material.color = ButtonColorDisable;
                    text2.color = ButtonTextColor;
                }
            }
        }
        public static List<ButtonInfo> GetCurrentList()
        {
            if (Mods.inSettings)
                return settingsbuttons;
            if (Mods.inOp)
                return OP;
            if (Mods.inMovement)
                return Movement;
            if (Mods.inRig)
                return Rig;
            if (Mods.inTag)
                return Tag;
            if (Mods.inSafety)
                return Safety;
            if (Mods.inVis)
                return Visual;
            if (Mods.inFav)
                return Music;
            if (Mods.inOtherMenus)
                return Genesis;

            return buttons;
        }
        public static List<ButtonInfo> fav = new List<ButtonInfo>();
        public static void Toggle(string relatedText)
        {
            List<ButtonInfo> currentList = GetCurrentList();

            int maxPages = Mathf.Max(1, (currentList.Count + pageSize - 1) / pageSize);

            ButtonInfo method = GetButton(relatedText);

            switch (relatedText)
            {
                case "NextPage":
                    pageNumber++;
                    if (pageNumber >= maxPages)
                        pageNumber = 0;

                    Mods.UpdateMenu();
                    return;

                case "PreviousPage":
                    pageNumber--;
                    if (pageNumber < 0)
                        pageNumber = maxPages - 1;

                    Mods.UpdateMenu();
                    return;

                case "DisconnectingButton":
                    PhotonNetwork.Disconnect();
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.25f);
                    return;
            }
            ButtonInfo button = GetButton(relatedText);

            if (button == null)
                return;

            bool isCurrentCategory = currentList == GetCurrentList();

            if (button.enabled != null)
                button.enabled = !button.enabled;

            if (button.enabled == true)
                button.method?.Invoke();
            else
                button.disableMethod?.Invoke();

            if (!ReferenceEquals(currentList, GetCurrentList()))
                return;

            tooltipString = $"{button.buttonText}: {button.toolTip}";

            Mods.UpdateMenu();
        }
    }
}

internal class BtnCollider : MonoBehaviour
{
    public static int framePressCooldown = 0;
    private void OnTriggerEnter(Collider collider)
    {
        if (Time.frameCount >= framePressCooldown + 20 && collider.name == "buttonPresser")
        {
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.1f);
            GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2, GorillaTagger.Instance.tagHapticDuration / 2);
            WristMenu.Toggle(this.relatedText);
            framePressCooldown = Time.frameCount;
        }
    }

    public string relatedText;
}