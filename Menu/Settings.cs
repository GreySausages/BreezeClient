using Breeze.Classes;
using UnityEngine;
using static Breeze.Menu.Main;

namespace Breeze
{
    internal class Settings
    {
        public static ExtGradient backgroundColor = new ExtGradient
        {
            colors = GetSolidGradient(Color.black)
        };

        public static ExtGradient[] buttonColors = new ExtGradient[]
        {
            new ExtGradient
            {
                colors = GetSolidGradient(new Color(0.06f, 0.06f, 0.06f))
            }, 
            new ExtGradient
            {
                colors = GetSolidGradient(new Color(0.06f, 0.06f, 0.06f))
            }
        };

        public static Color[] textColors = new Color[]
        {
            Color.white, // Disabled
            Color.magenta  // Enabled
        };

        public static readonly string[] ThemeNames = new string[]
        {
            "Dark",
            "Rainbow",
            "Breeze",
            "Blue",
            "Red",
            "Orange",
            "Blue 2",
        };

        public static void ThemeChanger()
        {
            themeindex = (themeindex + 1) % ThemeNames.Length;
            GetIndex("ChangeTheme").overlapText = $"Theme: {ThemeNames[themeindex]}";
            SetTheme();
        }

        public static void SetTheme()
        {
            if (themeindex == 1)
            {
                if (buttonColors[0] != new ExtGradient { isRainbow = true }) RecreateMenu();
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient
                    {
                        isRainbow = true,
                    },
                    new ExtGradient
                    {
                        isRainbow = true,
                    }
                };
                backgroundColor = new ExtGradient
                {
                    isRainbow = true,
                };
            }
            if (themeindex == 0)
            {
                if (buttonColors[0] != new ExtGradient { colors = GetSolidGradient(new Color(0.06f, 0.06f, 0.06f)) }) RecreateMenu();
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0.06f, 0.06f, 0.06f))
                    },
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0.06f, 0.06f, 0.06f))
                    }
                };
                textColors = new Color[]
                {
                    Color.white,
                    Color.magenta
                };
                backgroundColor = new ExtGradient
                {
                    colors = GetSolidGradient(Color.black)
                };
            }
            if (themeindex == 2)
            {
                if (buttonColors[0] != new ExtGradient { colors = GetSolidGradient(new Color(0f, 0.9f, 0.9f)) }) RecreateMenu();
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0f, 0.9f, 0.9f))
                    },
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0f, 1f, 1f))
                    }
                };
                textColors = new Color[]
                {
                    Color.white,
                    Color.white
                };
                backgroundColor = new ExtGradient
                {
                    colors = GetSolidGradient(new Color(0f, 1f, 1f))
                };
            }
            if (themeindex == 3)
            {
                if (buttonColors[0] != new ExtGradient { colors = GetSolidGradient(new Color(0f, 0f, 0.8f)) }) RecreateMenu();
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0f, 0f, 0.8f))
                    },
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0f, 0f, 1f))
                    }
                };
                backgroundColor = new ExtGradient
                {
                    colors = GetSolidGradient(new Color(0f, 0f, 1f))
                };
            }
            if (themeindex == 4)
            {
                if (buttonColors[0] != new ExtGradient { colors = GetSolidGradient(new Color(0.8f, 0f, 0f)) }) RecreateMenu();
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0.8f, 0f, 0f))
                    },
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(1f, 0f, 0f))
                    }
                };
                backgroundColor = new ExtGradient
                {
                    colors = GetSolidGradient(new Color(1f, 0f, 0f))
                };
            }
            if (themeindex == 5)
            {
                if (buttonColors[0] != new ExtGradient { colors = GetSolidGradient(new Color(0.9f, 0.4f, 0f)) }) RecreateMenu();
                buttonColors = new ExtGradient[]
                {
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(0.9f, 0.4f, 0f))
                    },
                    new ExtGradient
                    {
                        colors = GetSolidGradient(new Color(1f, 0.5f, 0f))
                    }
                };
                backgroundColor = new ExtGradient
                {
                    colors = GetSolidGradient(new Color(1f, 0.5f, 0f))
                };
                if (themeindex == 6)
                {
                    if (buttonColors[0] != new ExtGradient { colors = GetSolidGradient(new Color(0.9f, 0.4f, 0f)) }) RecreateMenu();
                    buttonColors = new ExtGradient[]
                    {
                    new ExtGradient
                    {
                        colors = GetSolidGradient(Color.blue)
                    },
                    new ExtGradient
                    {
                        colors = GetSolidGradient(Color.blue)
                    }
                    };
                    backgroundColor = new ExtGradient
                    {
                        colors = GetSolidGradient(Color.blue)
                    };
                    textColors = new Color[]
                    {
                        Color.white,
                        Color.magenta
                }   ;
                }
            }
        }

        public static Font currentFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        public static bool fpsCounter = true;
        public static bool disconnectButton = true;
        public static bool rightHanded = false;
        public static bool disableNotifications = false;
        public static bool roundmenu = true;

        public static KeyCode keyboardButton = KeyCode.LeftAlt;

        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f); // Depth, Width, Height
        public static int buttonsPerPage = 8;
    }
}