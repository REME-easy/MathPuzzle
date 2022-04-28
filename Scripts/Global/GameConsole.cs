using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game;

namespace MathPuzzle.Scripts.Global
{
    public class GameConsole : Control
    {
        [GetNode ("Panel")] public Panel Panel { get; protected set; }

        [GetNode ("Panel/LineEdit")] public LineEdit InputLine { get; protected set; }

        [GetNode ("Panel/RichTextLabel")] public RichTextLabel OutputLine { get; protected set; }

        [GetNode ("Tween")] public Tween Tween { get; protected set; }

        public bool IsActive = false;

        public override void _Ready ()
        {
            this.InitNode ();
            G.Inst.Console = this;
        }

        public override void _Process (float dt)
        {
            if (Input.IsActionJustPressed ("dev_console"))
            {
                GetParent ().MoveChild (this, GetParent ().GetChildCount () - 1);
                IsActive = !IsActive;
                if (IsActive)
                {
                    Tween.InterpolateProperty (Panel, "rect_position:y", -RectSize.y, 0.0F, 0.1F, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
                }
                else
                {
                    Tween.InterpolateProperty (Panel, "rect_position:y", 0.0F, -RectSize.y, 0.1F, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
                }
                Tween.Start ();
            }
        }

        public void AddMessage (string msg)
        {
            OutputLine.Text += $"\n{msg}";
        }

        private void _on_LineEdit_text_entered (string text)
        {
            if (IsActive)
            {
                // TODO
                var t = InputLine.Text;
                var @params = t.Split (" ").ToList ();
                if (@params.Count > 0)
                {
                    switch (@params[0])
                    {
                        case "skip":
                            G.Inst.SetNextLevel ();
                            break;
                        case "set_level":
                            new GameLevelBuilder (@params[1]).Build (out _);
                            break;
                    }
                }
            }
            InputLine.Text = "";
        }
    }
}