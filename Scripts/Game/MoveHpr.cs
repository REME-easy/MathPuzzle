using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Godot;

using MathPuzzle.Scripts.Game.Blocks.Items;
using MathPuzzle.Scripts.Game.Blocks.Operators;
using MathPuzzle.Scripts.Global;

namespace MathPuzzle.Scripts.Game
{
    public static class MoveHpr
    {
        public static void ForEachDir (Action<DirectionType> action)
        {
            DirectionType[] dirs = new [] { DirectionType.Up, DirectionType.Left, DirectionType.Down, DirectionType.Right };
            foreach (var dir in dirs)
            {
                action (dir);
            }
        }

        public static Vector2 GetMoveOffset (DirectionType dir)
        {
            return dir
            switch
            {
                DirectionType.Up => new Vector2 (0, -1),
                    DirectionType.Down => new Vector2 (0, 1),
                    DirectionType.Left => new Vector2 (-1, 0),
                    DirectionType.Right => new Vector2 (1, 0),
                    _ => new Vector2 (0, 0)
            };
        }

        public static void RunMovements (GameLevel level)
        {
            while (level.Movements.Count > 0)
            {
                level.Movements = level.Movements.OrderBy (m => m.ProcessOrder).ToList ();
                var move = level.Movements.Last ();
                // Logger.Debug ($"process {move}");
                switch (move.MovementType)
                {
                    case MoveType.Move:
                        RunMove (level, move);
                        break;
                    case MoveType.Refresh:
                        level.PopMove ();
                        move.Block.Refresh ();
                        break;
                    case MoveType.RefreshExpression:
                        level.PopMove ();
                        RefreshExpression (level);
                        break;
                    case MoveType.RunExpression:
                        level.PopMove ();
                        RunExpression (level);
                        break;
                    case MoveType.Undo:
                        level.PopMove ();
                        RunUndo (level);
                        break;
                    case MoveType.Victory:
                        level.PopMove ();
                        RunVictoryCheck (level);
                        break;
                }
            }
        }

        private static void RunMove (GameLevel level, Movement move)
        {
            // Logger.Debug (move);
            // 正常移动
            if (move.FromMap == move.ToMap && !move.ToMap.BlockMap.ContainsKey (move.To))
            {
                // 检查是否在地图范围内
                if (move.ToMap.CheckRange (move.To) && move.FromMap.CheckRange (move.From))
                {
                    level.Move (move.Block, move.FromMap, move.From, move.ToMap, move.To);
                }
                // 被阻挡
                else
                {
                    move.Block.CannotMoveThisProcess = true;
                    for (var i = level.Movements.Count - 1; i >= 0; i--)
                    {
                        var m = level.Movements[i];
                        if (m.MovementType == MoveType.Move && m.To == move.Block.MapPosition)
                        {
                            m.Block.CannotMoveThisProcess = true;
                        }
                    }
                }

                level.PopMove ();
                return;
            }

            // 同地图非推出地图推动其他方块
            if (move.FromMap == move.ToMap && move.ToMap.BlockMap.TryGetValue (move.To, out var target))
            {
                if (!target.CannotMoveThisProcess && !target.IsStatic)
                {
                    level.PushMove (Movement.CreateSameMapMove (target, move.ToMap, move.Direction));
                }
                else
                {
                    move.Block.CannotMoveThisProcess = true;
                    level.PopMove ();
                }

                return;
            }
        }

        private static void RefreshExpression (GameLevel level)
        {
            // 检查加载表达式
            foreach (var map in level.Maps)
            {
                map.RefreshMap ();
                map.VariableMap.Clear ();

                foreach (var block in map.Blocks)
                {
                    switch (block)
                    {
                        // 检查算式
                        case Equal eq:
                            ForEachDir ((dir) =>
                            {
                                CheckNeighbour (eq, dir, new MathExpression (dir, ExpressionType.Formula), level);
                            });
                            break;
                            // 检查赋值表达式
                        case Variable @var:
                            ForEachDir ((dir) =>
                            {
                                CheckNeighbour (@var, dir, new MathExpression (dir, ExpressionType.Assignment), level);
                            });
                            break;
                    }
                }
            }
        }

        private static void RunExpression (GameLevel level)
        {
            // 赋值表达式比算式先运算
            level.Expressions = level.Expressions.OrderBy (exp => -(int) exp.Type).ToList ();

            // 处理表达式
            var inputs = new List<Block> ();
            foreach (var exp in level.Expressions)
            {
                switch (exp.Type)
                {
                    case ExpressionType.Formula:
                        RunFormula (level, exp, inputs);
                        break;
                    case ExpressionType.Assignment:
                        RunAssignment (level, exp);
                        break;
                }
            }

            // Logger.Debug ($"{inputs.Count}");
            // foreach (var input in inputs)
            // {
            //     Logger.Debug ($"{input.GetType().Name}:{input.ParentMap},{input.MapPosition}");
            // }
            inputs.ForEach (b => level.Delete (b.ParentMap, b.MapPosition));
        }

        private static void RunFormula (GameLevel level, MathExpression exp, List<Block> inputs)
        {
            var list = new List<Block> (exp.Blocks);
            // 处理大于2位数的值方块
            for (var i = list.Count - 1; i > 0; i--)
            {
                var a = list[i];
                var b = list[i - 1];
                if (a.BlockType != BlockType.Value || b.BlockType != BlockType.Value || !b.Combine (a, out var c)) continue;
                list.Remove (a);
                list[i - 1] = c;
            }

            // var sb = new StringBuilder ();
            // exp.Blocks.ForEach (b => sb.Append ($" {b}"));
            // Logger.Debug ($"exp before: {sb}");
            // sb = new StringBuilder ();
            // list.ForEach (b => sb.Append ($" {b}"));
            // Logger.Debug ($"exp after: {sb}");

            var items = new List<Block> ();
            var ops = new Stack<Block> ();

            // 转为后缀表达式
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var block = list[i];

                // 第一个不能为操作符
                if (i == list.Count - 1 && block.BlockType == BlockType.Operator) break;

                switch (block.BlockType)
                {
                    case BlockType.Value:
                        items.Add (block);
                        break;
                    case BlockType.Operator:
                        {
                            if (ops.Count > 0)
                                while (ops.Count > 0 && ops.Peek ().ProcessOrder >= block.ProcessOrder)
                                    items.Add (ops.Pop ());
                            ops.Push (block);

                            if (i != 0) continue;
                            while (ops.Count > 0)
                                items.Add (ops.Pop ());
                            break;
                        }
                }
            }

            // 计算输出位置
            foreach (var block1 in items)
                if (block1 is Equal eq)
                {
                    var pos = eq.MapPosition - GetMoveOffset (exp.OutputDirection);
                    if (!eq.ParentMap.BlockMap.ContainsKey (pos))
                    {
                        exp.OutputPositions.Add (new OutputPosition (eq.ParentMap, pos));
                    }
                }
            if (exp.OutputPositions.Count == 0) return;

            // sb = new StringBuilder ();
            // items.ForEach (b => sb.Append ($" {b}"));
            // Logger.Debug ($"after: {sb}");

            if (items.Count < 3)
            {
                // Logger.Debug ("cannot become expression: not enough items");
                return;
            }

            // 计算
            var complete = true;
            var stk = new Stack<Block> ();
            for (var i = 0; i < items.Count; i++)
            {
                var blk = items[i];
                // 遇到值方块入栈
                if (blk.BlockType == BlockType.Value)
                {
                    stk.Push (blk);
                }
                else if (blk.BlockType == BlockType.Operator)
                {
                    // 遇到等于立刻停止
                    if (blk is Equal _)
                        break;

                    // 遇到操作符方块出栈计算
                    if (stk.Count < 2)
                    {
                        // Logger.Debug ("cannot become expression: lack item");
                        complete = false;
                        break;
                    }

                    var b = stk.Pop ();
                    var a = stk.Pop ();
                    if (!blk.Operate (a, b, out var result))
                    {
                        // Logger.Debug ("cannot become expression: invalid");
                        complete = false;
                        break;
                    }

                    stk.Push (result);
                }
            }

            // 清除输入
            // Logger.Debug ($"complete? {complete}");
            if (!complete) return;

            for (var i = 0; i < stk.Count; i++)
            {
                var output = stk.Pop ();
                var tup = exp.OutputPositions[0];
                var pos = tup.Position + i * GetMoveOffset (exp.OutputDirection);
                if (!tup.Map.BlockMap.ContainsKey (pos))
                    level.Spawn (tup.Map, pos, output);
                else
                    break;
            }

            exp.Blocks.ForEach (b =>
            {
                // Logger.Debug ("add input");
                if (b.BlockType == BlockType.Value && !b.IsStatic) inputs.Add (b);
            });
        }

        private static void RunAssignment (GameLevel level, MathExpression exp)
        {
            var list = new List<Block> (exp.Blocks);
            // 处理大于2位数的值方块
            for (var i = list.Count - 1; i > 1; i--)
            {
                var a = list[i];
                var b = list[i - 1];
                if (a.BlockType != BlockType.Value || b.BlockType != BlockType.Value || !b.Combine (a, out var c)) continue;
                list.Remove (a);
                list[i - 1] = c;
            }

            // Logger.Debug (string.Join (" ", list.Select (b => b.ToString ())));

            if (!(list[1] is Equal))
            {
                // Logger.Debug ("this is not a assignment");
                return;
            }

            var items = new List<Block> ();
            var ops = new Stack<Block> ();

            // 转为后缀表达式
            for (var i = list.Count - 1; i > 0; i--)
            {
                var block = list[i];

                // 第一个不能为操作符
                if (i == list.Count - 1 && block.BlockType == BlockType.Operator) break;

                switch (block.BlockType)
                {
                    case BlockType.Value:
                        items.Add (block);
                        break;
                    case BlockType.Operator:
                        {
                            if (ops.Count > 0)
                                while (ops.Count > 0 && ops.Peek ().ProcessOrder >= block.ProcessOrder)
                                    items.Add (ops.Pop ());
                            ops.Push (block);

                            if (i != 0) continue;
                            while (ops.Count > 0)
                                items.Add (ops.Pop ());
                            break;
                        }
                }
            }

            // 计算
            var stk = new Stack<Block> ();
            for (var i = 0; i < items.Count; i++)
            {
                var blk = items[i];
                // 遇到值方块入栈
                if (blk.BlockType == BlockType.Value)
                {
                    stk.Push (blk);
                }
                else if (blk.BlockType == BlockType.Operator)
                {
                    // 遇到等于立刻停止
                    if (blk is Equal _)
                        break;

                    // 遇到操作符方块出栈计算
                    if (stk.Count < 2)
                    {
                        // Logger.Debug ("cannot become expression: lack item");
                        break;
                    }

                    var b = stk.Pop ();
                    var a = stk.Pop ();
                    if (!blk.Operate (a, b, out var result))
                    {
                        // Logger.Debug ("cannot become expression: invalid");
                        break;
                    }

                    stk.Push (result);
                }
            }

            if (stk.Count != 1)
            {
                // Logger.Debug (string.Join (" ", stk.Select (i => i.ToString ())));
                // Logger.Debug ($"try to assign multiple values: {stk.Count}");
                return;
            }

            string @var = "";
            if (list[0] is Variable v)
                @var = v.VariableData;
            // Logger.Debug ($"try to assign {stk.Peek()} to {@var}");

            if (!exp.RootMap.VariableMap.TryGetValue (@var, out var val))
            {
                exp.RootMap.VariableMap.Add (@var, stk.Pop ());
            }
            else
            {
                exp.RootMap.VariableMap[@var] = null;
            }
        }

        private static void CheckNeighbour (Block current, DirectionType dir, MathExpression exp, GameLevel level)
        {
            exp.Blocks.Add (current);
            var found = false;
            if (current.ParentMap.BlockMap.TryGetValue (current.MapPosition + GetMoveOffset (dir), out var b) &&
                !exp.Blocks.Contains (b) && (b.BlockType == BlockType.Value || b.BlockType == BlockType.Operator) &&
                (exp.Type != ExpressionType.Formula || !(b is Equal)))
            {
                CheckNeighbour (b, dir, exp, level);
                found = true;
            }

            if (!found && exp.Blocks.Count > 1)
            {
                exp.RootMap = exp.Blocks[0].ParentMap;
                level.Expressions.Add (exp);
            }
        }

        private static void RunUndo (GameLevel level)
        {
            if (level.Turns.Count == 0)
            {
                return;
            }

            var turn = new Turn ();
            for (var i = level.Turns.Count - 1; i >= 0; i--)
            {
                var cmd = level.Turns[i].Command.CommandType;
                if (cmd != CommandType.Move && cmd != CommandType.Process) continue;
                turn = level.Turns[i];
                break;
            }

            if (turn.Histories != null)
                foreach (var history in turn.Histories)
                {
                    // Logger.Debug ($"undo move: {history}");
                    history.Undo ();
                }

            level.Turns.Remove (turn);
        }

        private static void RunVictoryCheck (GameLevel level)
        {
            var isWin = true;
            foreach (var map in level.Maps)
            {
                map.RefreshMap ();
                if (map.Floors.Any (floor => !floor.IsSatisfied)) isWin = false;
            }

            if (!isWin) return;

            Logger.Debug ("win!");
            level.IsVictory = true;

            // TODO
            G.Inst.SetNextLevel ();
        }
    }
}