using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;

namespace MathPuzzle.Scripts.Game.Blocks.Items
{
    public class Number : Block, INumber
    {
        [GetNode ("Label")] public Label NumberLabel { get; protected set; }

        private int numberData;
        public int NumberData
        {
            get => numberData;
            set
            {
                numberData = value;
                if (NumberLabel != null)
                    if (IsMax || numberData == int.MaxValue)
                        NumberLabel.Text = "∞";
                    else
                        NumberLabel.Text = numberData.ToString ();
            }
        }

        public int NumberInfo { get => NumberData; set => NumberData = value; }

        public bool IsMax;

        public static readonly PackedScene NumberFac = (PackedScene) ResourceLoader.Load ("res://Scenes/Blocks/Items/Number.tscn");

        public Number ()
        {
            BlockType = BlockType.Value;
        }

        public override void _Ready ()
        {
            this.InitNode ();
            InitOnReady ();
        }

        public override void InitOnReady ()
        {
            base.InitOnReady ();
            if (NumberLabel != null)
                if (IsMax || NumberData == int.MaxValue)
                    NumberLabel.Text = "∞";
                else
                    NumberLabel.Text = NumberData.ToString ();
            if (IsFloor)
            {
                var c = Modulate;
                c.a /= 2.0F;
                Modulate = c;
            }
        }

        public override void Init (BlockParams blockParams)
        {
            base.Init (blockParams);
            if (Params.TryGet<long> ("number", out var n))
            {
                NumberData = (int) n;
                if (n == int.MaxValue)
                    IsMax = true;
            }
            if (Params.TryGet<bool> ("max", out var max))
            {
                NumberData = int.MaxValue;
                IsMax = max;
            }
        }

        public override string ToString ()
        {
            return $"{base.ToString ()}({NumberData})";
        }

        public override void Refresh ()
        {
            IsSatisfied = false;
            if (IsFloor && ParentMap.BlockMap.TryGetValue (MapPosition, out var above))
            {
                if (above is Number n && n.NumberData == NumberData)
                {
                    IsSatisfied = true;
                }
            }
        }

        public override bool Combine (Block b, out Block output)
        {
            output = this;
            if (b is Number bn)
            {
                long res = 0;
                if (IsMax || bn.IsMax)
                {
                    res = int.MaxValue;
                }
                else
                {
                    res = int.Parse (bn.NumberData.ToString () + NumberData.ToString ());
                }
                output = NumberFac.Instance<Number> ();
                output.Init (new BlockParams ().AddParams ("number", res));
                return true;
            }
            return false;
        }
    }
}