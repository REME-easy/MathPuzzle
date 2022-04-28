using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;

namespace MathPuzzle.Scripts.Global
{
    public static class DataMgr
    {

        private static readonly Dictionary<string, string> BlockScenes = new Dictionary<string, string> ();
        public static IReadOnlyDictionary<string, string> Scenes => BlockScenes;

        public static void Init ()
        {
            RegisterBlocks ();
        }

        public static void RegisterBlocks ()
        {
            FileMgr.LoadDir ("res://Scenes/Blocks", (dir, fileName) =>
            {
                var path = $"{dir.GetCurrentDir()}/{fileName}";

                Logger.Debug (Path.GetFileNameWithoutExtension (fileName).ToSnakeCase (), path);
                BlockScenes.Add (Path.GetFileNameWithoutExtension (fileName).ToSnakeCase (), path);
            });
        }

    }
}