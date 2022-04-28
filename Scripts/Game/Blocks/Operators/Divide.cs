using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game.Blocks.Items;

namespace MathPuzzle.Scripts.Game.Blocks.Operators
{
    public class Divide : Block
    {
        public Divide ()
        {
            BlockType = BlockType.Operator;
            ProcessOrder = 2;
        }

        public override void _Ready ()
        {
            this.InitNode ();
            InitOnReady ();
        }

        public override bool Operate (Block a, Block b, out Block output)
        {
            output = a;
            if (a is INumber na && b is INumber nb)
            {
                if (!IsValidVariable (a, b))
                    return false;
                var res = 0;
                if (nb.NumberInfo == 0)
                    res = int.MaxValue;
                else if (na.NumberInfo == int.MaxValue && nb.NumberInfo == int.MaxValue)
                    res = 1;
                else if (nb.NumberInfo == int.MaxValue)
                    res = 0;
                else
                    res = na.NumberInfo / nb.NumberInfo;

                output = Number.NumberFac.Instance<Number> ();
                output.Init (new BlockParams ().AddParams ("number", (long) res));
                return true;
            }
            return false;
        }
    }
}