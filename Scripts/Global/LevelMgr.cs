using System.Collections.Generic;

using Tomlyn;
using Tomlyn.Model;

namespace MathPuzzle.Scripts.Global
{
    public static class LevelMgr
    {
        private static readonly Dictionary<string, TomlTable> _levels = new Dictionary<string, TomlTable> ();
        public static IReadOnlyDictionary<string, TomlTable> Levels => _levels;

        public static void Init ()
        {
            LoadLevels ();
        }

        public static void LoadLevels ()
        {
            FileMgr.LoadDir ("res://Assets/Mod/OriginalGame", (dir, fileName) =>
            {
                AddLevel ($"{dir.GetCurrentDir()}/{fileName}");
            });
        }

        public static void AddLevel (string path)
        {
            var file = new Godot.File ();
            var err = file.Open (path, Godot.File.ModeFlags.Read);

            if (err == Godot.Error.Ok)
            {
                var model = Toml.ToModel (file.GetAsText ());
                var id = ((TomlTable) model["info"] !) ["id"] as string;
                var category = ((TomlTable) model["info"] !) ["category"] as string;
                if (id != null && category != null)
                {
                    Logger.Debug ($"add level:{category}:{id}");
                    _levels.Add ($"{category}:{id}", model);
                }
            }
        }
    }
}