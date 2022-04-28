using Godot;

using MathPuzzle.Scripts.Game;

namespace MathPuzzle.Scripts.Global
{
    public class G : Node
    {
        public static G Inst { get; private set; }

        public Settings Settings { get; private set; }
        public InputHpr InputHpr { get; private set; }

        public ViewController View { get; set; }
        public GameConsole Console { get; set; }

        public GameLevel CurrentLevel;

        public int LevelIndex = 0;

        public override void _Ready ()
        {
            Inst = this;
            Settings = new Settings ();
            AddChild (Settings);
            InputHpr = new InputHpr ();
            AddChild (InputHpr);

            // LuaMgr.Init ();
            DataMgr.Init ();
            LevelMgr.Init ();
        }

        // public static void SetNextLevel (BaseLevel newLevel) {
        //     Inst.View.SetScene(newLevel);
        // }

        public void SetNextLevel ()
        {
            LevelIndex++;
            if (LevelMgr.Levels.Count > LevelIndex)
            {
                var en = LevelMgr.Levels.GetEnumerator ();
                var index = 0;
                while (en.MoveNext ())
                {
                    if (index == LevelIndex)
                    {
                        new GameLevelBuilder (en.Current.Key).Build (out _);
                        break;
                    }
                    index++;
                }
            }
        }
    }
}