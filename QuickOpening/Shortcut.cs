using System;
using System.Diagnostics;
using System.IO;
using IWRL = IWshRuntimeLibrary;

namespace QuickOpening
{
    public static class Shortcut
    {
        static public void CreateStartupShortcut(string Description = null, string Arguments = null, string HotKey = null, string IconLocation = null)
        {
            CreateShortcut(Process.GetCurrentProcess().MainModule.FileName,
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                Description, Arguments, HotKey, IconLocation);
        }

        static public void DeleteStartupShortcut()
        {
            DeleteShortcut(Process.GetCurrentProcess().MainModule.FileName,
                Environment.GetFolderPath(Environment.SpecialFolder.Startup));
        }

        static public void CreateShortcut(string OriginalPathAndName, string DestinationSavePath,
            string Description = null, string Arguments = null, string HotKey = null, string IconLocation = null)
        {
            string FileName = Path.GetFileNameWithoutExtension(OriginalPathAndName);
            string OriginalFilePath = Path.GetDirectoryName(OriginalPathAndName);

            string link = Path.Combine(DestinationSavePath, $"{FileName}.lnk");

            IWRL.WshShell shell = new IWRL.WshShell();
            IWRL.IWshShortcut shortcut = (IWRL.IWshShortcut)shell.CreateShortcut(link);

            if (shortcut != null)
            {
                shortcut.TargetPath = OriginalPathAndName;
                shortcut.WorkingDirectory = OriginalFilePath;

                if (!String.IsNullOrEmpty(Description))
                    shortcut.Description = Description;

                if (!String.IsNullOrEmpty(Arguments))
                    shortcut.Arguments = Arguments;

                if (!String.IsNullOrEmpty(HotKey))
                    shortcut.Hotkey = HotKey;

                if (!String.IsNullOrEmpty(IconLocation))
                    shortcut.IconLocation = IconLocation;

                shortcut.Save();
            }
        }

        static public void DeleteShortcut(string OriginalPathAndName, string DestinationSavePath)
        {
            string FileName = Path.GetFileNameWithoutExtension(OriginalPathAndName);

            string link = Path.Combine(DestinationSavePath, $"{FileName}.lnk");

            File.Delete(link);
        }
    }
}
