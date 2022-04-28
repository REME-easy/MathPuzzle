using System;

using Godot;

using MathPuzzle.Scripts.Global;

namespace MathPuzzle.Scripts.Global
{
    public static class FileMgr
    {
        public static string GetProjectPath ()
        {
            string pathExeDir;

            if (OS.HasFeature ("standalone"))
                // set to exported release dir
                pathExeDir = $"{System.IO.Directory.GetParent(OS.GetExecutablePath()).FullName}";
            else
                // set to project dir
                pathExeDir = ProjectSettings.GlobalizePath ("res://");

            return pathExeDir;
        }

        public static bool LoadDir (string path, Action<Directory, string> action)
        {
            var dir = new Directory ();
            var error = dir.Open (path);
            if (error != Error.Ok)
            {
                Logger.Error ($"Failed to open \"{path}\", error: {error.ToString()}");
                return false;
            }

            dir.ListDirBegin (true);
            var fileName = dir.GetNext ();
            while (fileName != "")
            {
                if (dir.CurrentIsDir ())
                {
                    LoadDir ($"{dir.GetCurrentDir()}/{fileName}", action);
                }
                else
                {
                    action (dir, fileName);
                }
                fileName = dir.GetNext ();
            }
            dir.ListDirEnd ();

            return true;
        }
    }
}