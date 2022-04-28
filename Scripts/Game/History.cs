using System;

using Godot;

using MathPuzzle.Scripts.Global;

namespace MathPuzzle.Scripts.Game
{
    public readonly struct History
    {
        private readonly Block _block;

        private readonly GameMap _fromMap;
        private readonly Vector2 _from;
        private readonly GameMap _toMap;
        private readonly Vector2 _to;

        private readonly HistoryType _type;
        private readonly bool _isFloor;

        public History (Block block, GameMap fromMap, Vector2 from, GameMap toMap, Vector2 to, HistoryType type, bool isFloor = false)
        {
            _block = block;
            _fromMap = fromMap;
            _from = from;
            _toMap = toMap;
            _to = to;

            _type = type;
            _isFloor = isFloor;
        }

        public override string ToString ()
        {
            return $"history(block={_block},position=[from:{_from},to:{_to}],type={_type},floor={_isFloor})";
        }

        public void Undo ()
        {
            if (_isFloor)
                switch (_type)
                {
                    case HistoryType.Spawn:
                        if (_fromMap.FloorMap.TryGetValue (_from, out var b) && b == _block)
                            G.Inst.CurrentLevel.Delete (_fromMap, _from, true);
                        break;
                    case HistoryType.Delete:
                        G.Inst.CurrentLevel.Spawn (_fromMap, _from, _block, true);
                        break;
                    case HistoryType.Move:
                        G.Inst.CurrentLevel.Move (_block, _toMap, _to, _fromMap, _from, true, true);
                        break;
                }
            else
                switch (_type)
                {
                    case HistoryType.Spawn:
                        if (_fromMap.BlockMap.TryGetValue (_from, out var b) && b == _block)
                            G.Inst.CurrentLevel.Delete (_fromMap, _from);
                        break;
                    case HistoryType.Delete:
                        G.Inst.CurrentLevel.Spawn (_fromMap, _from, _block);
                        break;
                    case HistoryType.Move:
                        G.Inst.CurrentLevel.Move (_block, _toMap, _to, _fromMap, _from, false, true);
                        break;
                }
        }
    }
}