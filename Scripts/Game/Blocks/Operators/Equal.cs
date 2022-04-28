using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MathPuzzle.Scripts.Extensions;

namespace MathPuzzle.Scripts.Game.Blocks.Operators
{
    public class Equal : Block
    {
        public Equal ()
        {
            BlockType = BlockType.Operator;
        }

        public override void _Ready ()
        {
            this.InitNode ();
            InitOnReady ();
        }
    }
}