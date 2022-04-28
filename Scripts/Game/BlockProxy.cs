using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace MathPuzzle.Scripts.Game
{
    public class BlockProxy
    {
        public Block Target;

        [MoonSharpHidden]
        public BlockProxy (Block target)
        {
            Target = target;
        }
    }
}