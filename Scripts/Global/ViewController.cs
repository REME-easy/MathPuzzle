using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game;

namespace MathPuzzle.Scripts.Global
{
    public class ViewController : Control
    {
        [GetNode ("MainView/Viewport")] public Viewport MainView { get; protected set; }

        [GetNode ("Tween")] public Tween Tween { get; protected set; }

        // private static readonly PackedScene StartMenuFac =
        //     (PackedScene) ResourceLoader.Load("res://src/global/StartMenu.tscn");

        public override void _Ready ()
        {
            this.InitNode ();
            G.Inst.View = this;
            // SetScene(StartMenuFac.Instance(), true);

            new GameLevelBuilder ("original:test_level_1").Build (out _);
        }

        public void SetScene (Node @new, bool instantly = false)
        {
            if (!instantly)
            {
                MainView.AddChild (@new);
                var property = "";
                if (MainView.GetChildCount () > 1)
                {
                    var ori = MainView.GetChild (0);

                    switch (ori)
                    {
                        case Node2D _:
                            property = "position:x";
                            break;
                        case Control _:
                            property = "rect_position:x";
                            break;
                    }

                    Tween.InterpolateProperty (ori, property, 0.0F, -GetViewportRect ().Size.x, 1.0F,
                        Tween.TransitionType.Cubic);
                }

                switch (@new)
                {
                    case Node2D _:
                        property = "position:x";
                        break;
                    case Control _:
                        property = "rect_position:x";
                        break;
                }

                Tween.InterpolateProperty (@new, property, GetViewportRect ().Size.x, 0.0F, 1.0F,
                    Tween.TransitionType.Cubic);

                Tween.Start ();
                MainView.HandleInputLocally = false;
            }
            else
            {
                if (MainView.GetChildCount () > 0)
                    MainView.RemoveChild (MainView.GetChild (0));
                MainView.AddChild (@new);
            }
        }

        private void _on_Tween_tween_all_completed ()
        {
            if (MainView.GetChildCount () > 1)
                MainView.RemoveChild (MainView.GetChild (0));
            MainView.HandleInputLocally = true;
        }
    }
}