using System;
using Godot;

namespace MathPuzzle.Scripts.Global
{
    public class Settings : Node
    {
        public float WindowSize { get; set; } = 1.0F;

        public Vector2 DefaultFullScreenSize { get; set; } = new Vector2(1920, 1080);

        public override void _Ready()
        {
            WindowSize = OS.WindowSize.x / (int) ProjectSettings.GetSetting("display/window/size/width");
            // SetFullScreen ();
            Logger.Info($"game window ratio: {WindowSize}");
            Logger.Info($"Memory use: {OS.GetDynamicMemoryUsage()}");
            Logger.Info($"OS Name: {OS.GetName()}");
        }

        public void SetFullScreen()
        {
            var size = OS.GetScreenSize();
            if (OS.GetName() == "Windows" && size == DefaultFullScreenSize)
            {
                OS.WindowFullscreen = true;
            }
            else
            {
                var scale = Math.Min(size.x / DefaultFullScreenSize.x, size.y / DefaultFullScreenSize.y);
                var scaledSize = (DefaultFullScreenSize * scale).Round();

                var margins = new Vector2(size.x - scaledSize.x, size.y - scaledSize.y);
                var screenRect = new Rect2((margins / 2).Round(), scaledSize);

                OS.WindowBorderless = true;
                OS.WindowPosition = OS.GetScreenPosition();
                OS.WindowSize = new Vector2(size.x, size.y + 1);

                var root = GetTree().Root;
                root.Size = scaledSize;
                root.SetAttachToScreenRect(screenRect);
            }
        }
    }
}