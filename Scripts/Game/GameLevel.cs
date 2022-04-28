using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Global;

namespace MathPuzzle.Scripts.Game
{
    public class GameLevel : Node2D
    {
        [GetNode ("UnusedBlocks")] public Node2D UnusedBlocksNode { get; protected set; }

        [GetNode ("LevelTitle")] public Label LevelTitle { get; protected set; }

        public string LevelID;
        public GameMap RootMap;
        public GameMap CurrentMap;

        public List<GameMap> Maps = new List<GameMap> ();
        public List<MathExpression> Expressions = new List<MathExpression> ();
        public List<Movement> Movements = new List<Movement> ();
        public List<Turn> Turns = new List<Turn> ();
        public Turn CurrentTurn;

        public bool IsVictory;

        public override void _Ready ()
        {
            this.InitNode ();
            G.Inst.CurrentLevel = this;
            foreach (var map in Maps)
            {
                AddChild (map);
                Logger.Debug ($"add map: {map}");
            }

            LevelTitle.Text = Tr($"{LevelID}_name");
        }

        public void PushMove (Movement move)
        {
            // Logger.Debug ($"push move:{move}");
            Movements.Add (move);
        }

        public void PopMove ()
        {
            if (Movements.Count > 0)
            {
                // Logger.Debug ($"popped move: {Movements.Last()}");
                Movements.RemoveAt (Movements.Count - 1);
            }
        }

        public void Spawn (GameMap map, Vector2 pos, Block block, bool isFloor = false)
        {
            if (!isFloor)
            {
                map.AddBlock (pos, block);
                block.GetParent ()?.RemoveChild (block);
                map.BlocksNode.AddChild (block);
            }
            else
            {
                map.AddFloor (pos, block);
                block.GetParent ()?.RemoveChild (block);
                map.FloorsNode.AddChild (block);
            }
            block.Spawn ();
            G.Inst.CurrentLevel.CurrentTurn.Histories.Add (new History (block, map, pos, null, -Vector2.One, HistoryType.Spawn, isFloor));
        }

        public void Delete (GameMap map, Vector2 pos, bool isFloor = false)
        {
            Block block = null;
            if (!isFloor)
            {
                block = map.RemoveBlock (pos);
                map.Blocks.Remove (block);
            }
            else
            {
                block = map.RemoveFloor (pos);
                map.Floors.Remove (block);
            }
            if (block == null)
                return;
            block.Delete ();
            G.Inst.CurrentLevel.CurrentTurn.Histories.Add (new History (block, map, pos, null, -Vector2.One, HistoryType.Delete, isFloor));
        }

        public void Move (Block block, GameMap fromMap, Vector2 from, GameMap toMap, Vector2 to, bool isFloor = false, bool isHistory = false)
        {
            if (!isFloor)
            {
                if (isHistory)
                {
                    if (fromMap.BlockMap.TryGetValue (from, out var t) && t == block)
                        fromMap.RemoveBlock (from);
                    toMap.SetBlock (to, block);
                }
                else
                {
                    fromMap.RemoveBlock (from);
                    toMap.AddBlock (to, block);
                }
            }
            else
            {
                if (isHistory)
                {
                    if (fromMap.FloorMap.TryGetValue (from, out var t) && t == block)
                        fromMap.RemoveFloor (from);
                    toMap.SetFloor (to, block);
                }
                else
                {
                    fromMap.RemoveFloor (from);
                    toMap.AddFloor (to, block);
                }
            }
            block.Move ();
            G.Inst.CurrentLevel.CurrentTurn.Histories.Add (new History (block, fromMap, from, toMap, to, HistoryType.Move, isFloor));
        }

        public void ProcessCommand (Command cmd)
        {
            // Logger.Debug ("====== start processing ======");
            Expressions.Clear ();
            switch (cmd.CommandType)
            {
                case CommandType.Move:
                    Maps.ForEach (m =>
                    {
                        foreach (var b in m.Blocks)
                        {
                            b.CannotMoveThisProcess = false;
                            if (b.IsPlayer)
                                PushMove (Movement.CreateSameMapMove (b, m, cmd.DirectionType));
                            if (b.BlockType == BlockType.Operator)
                                PushMove (Movement.CreateRefreshCheck (b));
                        }
                        foreach (var f in m.Floors)
                            PushMove (Movement.CreateRefreshCheck (f));
                    });
                    break;
                case CommandType.Process:
                    PushMove (Movement.CreateRunExpression ());
                    break;
                case CommandType.Undo:
                    PushMove (Movement.CreateUndo ());
                    break;
                case CommandType.None:
                default:
                    break;
            }

            PushMove (Movement.CreateRefreshExpression ());
            PushMove (Movement.CreateVictoryCheck ());

            CurrentTurn = new Turn (cmd, new List<Movement> (Movements), new List<History> ());
            // Logger.Debug ($"move count: {Movements.Count}");
            MoveHpr.RunMovements (this);
            // Logger.Debug ($"history count: {CurrentTurn.Histories.Count}");

            if (cmd.CommandType != CommandType.Undo && CurrentTurn.Histories.Count > 0)
                Turns.Add (CurrentTurn);
            // Logger.Debug ("====== process end ======");
        }

        public override void _PhysicsProcess (float delta) { }
    }
}