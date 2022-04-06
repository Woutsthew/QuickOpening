using System.Collections.Generic;
using System.Windows.Forms;
using static QuickOpening.KeyboardManager;

namespace QuickOpening
{
    public static class Constants
    {
        public const string images = "images";

        public const string fileSettings = "settings.txt";

        public static readonly List<string> extensionFile = new List<string> { "exe", "lnk", "sln", "txt", "zip", "rar", "7z",
            "pdf", "doc", "docx", "pptx", "ppt", "accdb", "pub", "xlsx", "xls", "mp3",
            "mp4", "avi", "mkv" };

        public static readonly List<string> extensionImage = new List<string> { "png", "jpg", "bmp", "gif" };

        public static readonly List<Keys> keys = new List<Keys>
        {
            Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6,
            Keys.NumPad7, Keys.NumPad8, Keys.NumPad9
        };

        public static readonly List<KeyModifiers> modifirs = new List<KeyModifiers>
        {
            KeyModifiers.Control, KeyModifiers.Alt
        };
    }
}
