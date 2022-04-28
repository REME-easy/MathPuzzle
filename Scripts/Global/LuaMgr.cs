using System.IO;

using Godot;

using MathPuzzle.Scripts.Global;

using MoonSharp.Interpreter;
using LuaScript = MoonSharp.Interpreter.Script;

namespace MathPuzzle.Scripts.Global
{
    public struct Mod
    {
        public ModInfo ModInfo { get; set; }
        public string Path { get; set; }
    }

    public struct ModInfo
    {
        public string Name { get; set; }
        public string[] Authors { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string[] GameVersions { get; set; }
        public string[] Dependencies { get; set; }
    }

    public static class LuaMgr
    {
        public static LuaScript Script;

        public static void Init ()
        {
            Script = new LuaScript ();
            LoadLuaScripts ("res://Assets/Mod");

            var dir = new Godot.Directory ();
            dir.MakeDir ("user://Mods");
            LoadLuaScripts ("user://Mods");
        }

        public static void LoadLuaScripts (string directory)
        {
            FileMgr.LoadDir (directory, (dir, fileName) =>
            {
                var absolutePath = $"{dir.GetCurrentDir()}/{fileName}";

                var luaScript = new Godot.File ();
                var err = luaScript.Open (absolutePath, Godot.File.ModeFlags.Read);

                if (err == Godot.Error.Ok)
                    Script.DoString (luaScript.GetAsText ());
                else
                    Logger.Error ($"Could not open file: {absolutePath}");
            });
        }

        public static void Call (string v, params object[] args)
        {
            try
            {
                Script.Call (Script.Globals[v], args);
            }
            catch (ScriptRuntimeException e)
            {
                Logger.Error (e.DecoratedMessage);
            }
        }

        public static void Set () { }
    }
}