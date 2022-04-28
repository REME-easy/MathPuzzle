using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game.Blocks.Items;

namespace MathPuzzle.Scripts.Game.Blocks.Operators
{
    public class Minus : Block
    {
        public Minus ()
        {
            BlockType = BlockType.Operator;
            ProcessOrder = 1;
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
                var res = na.NumberInfo - nb.NumberInfo;
                if (na.NumberInfo == int.MaxValue || nb.NumberInfo == int.MaxValue) res = int.MaxValue;

                output = Number.NumberFac.Instance<Number> ();
                output.Init (new BlockParams ().AddParams ("number", (long) res));
                return true;
            }
            return false;
        }
    }
}