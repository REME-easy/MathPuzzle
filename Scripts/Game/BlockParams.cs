using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathPuzzle.Scripts.Game
{
    public class BlockParams
    {
        public readonly Dictionary<string, object> Params;

        public BlockParams ()
        {
            Params = new Dictionary<string, object> ();
        }

        public BlockParams AddParams (string key, object obj)
        {
            Params.Add (key, obj);
            return this;
        }

        public T Get<T> (string key)
        {
            return Params.TryGetValue (key, out var val) ? (T) val : default;
        }

        public bool TryGet<T> (string key, out T val)
        {
            var flag = Params.TryGetValue (key, out var v);
            val = flag ? (T) v : default;
            return flag;
        }
    }
}