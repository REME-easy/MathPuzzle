using Godot;

using MathPuzzle.Scripts.Game;

using Scripts.Game;

namespace MathPuzzle.Scripts.Global
{
    public class InputHpr : Node
    {
        public bool UpAction;
        public bool DownAction;
        public bool LeftAction;
        public bool RightAction;
        public bool ProcessAction;
        public bool UndoAction;
        public bool RestartAction;

        public bool KeyLock;

        public Timer ProcessingTimer;

        public static float WaitTime = 0.15F;

        public override void _Ready ()
        {
            ProcessingTimer = new Timer
            {
                WaitTime = WaitTime,
                ProcessMode = Timer.TimerProcessMode.Physics,
                Autostart = false,
                OneShot = true,
            };
            AddChild (ProcessingTimer);
        }

        public override void _PhysicsProcess (float delta)
        {
            ProcessReset ();
            ProcessKeyInput ();
            ProcessActions ();
        }

        private void ProcessReset ()
        {
            UpAction = false;
            DownAction = false;
            LeftAction = false;
            RightAction = false;
            ProcessAction = false;
            UndoAction = false;
            RestartAction = false;
        }

        private void ProcessKeyInput ()
        {
            var process = Input.IsActionJustPressed ("move_process");
            var undo = Input.IsActionJustPressed ("move_undo");
            var restart = Input.IsActionJustPressed ("move_restart");

            var up = Input.IsActionPressed ("move_up");
            var down = Input.IsActionPressed ("move_down");
            var left = Input.IsActionPressed ("move_left");
            var right = Input.IsActionPressed ("move_right");

            if ((!process && !undo && !restart && !up && !down && !left && !right) && !ProcessingTimer.IsStopped ())
            {
                ProcessingTimer.Stop ();
            }

            if (!ProcessingTimer.IsStopped ())
                return;

            UpAction = up;
            DownAction = down;
            LeftAction = left;
            RightAction = right;

            if (UpAction || DownAction || LeftAction || RightAction)
            {
                ProcessingTimer.Start (WaitTime);
            }

            ProcessAction = process;
            UndoAction = undo;
            RestartAction = restart;
        }

        private void ProcessActions ()
        {
            if (G.Inst.CurrentLevel.IsVictory || G.Inst.Console.IsActive)
                return;

            var dir = DirectionType.None;
            var cmt = CommandType.None;

            if (ProcessAction)
                cmt = CommandType.Process;

            if (UndoAction)
                cmt = CommandType.Undo;

            if (RestartAction)
                cmt = CommandType.Restart;

            if (cmt == CommandType.None)
            {
                if (UpAction ^ DownAction)
                    dir = UpAction ? DirectionType.Up : DirectionType.Down;

                if (LeftAction ^ RightAction)
                    dir = LeftAction ? DirectionType.Left : DirectionType.Right;

                if (dir != DirectionType.None)
                    cmt = CommandType.Move;
            }

            if (cmt == CommandType.Restart)
            {
                new GameLevelBuilder (G.Inst.CurrentLevel.LevelID).Build (out _);
            }
            else if (cmt != CommandType.None)
            {
                var cmd = new Command (cmt, dir);
                G.Inst.CurrentLevel.ProcessCommand (cmd);
                // Logger.Info ($"{cmt},{dir}");
            }
        }
    }
}