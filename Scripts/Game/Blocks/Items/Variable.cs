using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;

namespace MathPuzzle.Scripts.Game.Blocks.Items
{
    public class Variable : Block, INumber
    {
        [GetNode ("Label")] public Label VariableLabel { get; protected set; }

        private string variableData;
        public string VariableData
        {
            get => variableData;
            set
            {
                variableData = value;
                if (VariableLabel != null)
                {
                    VariableLabel.Text = variableData;
                }
            }
        }

        public int NumberInfo
        {
            get
            {
                if (ParentMap.VariableMap.TryGetValue (VariableData, out var blk) && blk is Number n)
                    return n.NumberData;
                return 0;
            }
            set { }
        }

        public bool IsMax;

        public static readonly PackedScene VariableFac = (PackedScene) ResourceLoader.Load ("res://Scenes/Blocks/Items/Variable.tscn");

        public Variable ()
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
            if (VariableLabel != null)
            {
                VariableLabel.Text = variableData;
            }
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
            if (Params.TryGet<string> ("var", out var @var))
            {
                VariableData = @var;
            }
        }

        public override string ToString ()
        {
            return $"{base.ToString ()}({VariableData})";
        }

        public override void Refresh ()
        {
            IsSatisfied = false;
        }

        public override bool Combine (Block left, out Block output)
        {
            output = this;
            if (left is INumber bn)
            {
                long res = 0;
                if (NumberInfo == int.MaxValue || bn.NumberInfo == int.MaxValue)
                {
                    res = int.MaxValue;
                }
                else
                {
                    res = NumberInfo * bn.NumberInfo;
                }
                output = VariableFac.Instance<Number> ();
                output.Init (new BlockParams ().AddParams ("number", res));
                return true;
            }
            return false;
        }
    }
}