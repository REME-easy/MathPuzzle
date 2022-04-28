using System;
using System.Text;

using Godot;

namespace MathPuzzle.Scripts.Global
{
    public enum LogType
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
    }

    public static class Logger
    {
        public static int LogLevel;

        private static string GetDateText ()
        {
            return DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss.fff");
        }

        private static void Record (LogType type, params object[] msg)
        {
            var sb = new StringBuilder ();
            foreach (var str in msg)
            {
                sb.Append ($"{str} ");
            }

            var text = $"[ {GetDateText()} ][ {type.ToString()} ] {sb.ToString()}";
            if ((int) type >= 3)
                GD.PrintErr (text);
            else
                GD.Print (text);
        }

        public static void Debug (params object[] str)
        {
            Record (LogType.Debug, str);
        }

        public static void Info (params object[] str)
        {
            Record (LogType.Info, str);
        }

        public static void Error (params object[] str)
        {
            Record (LogType.Error, str);
        }

    }
}