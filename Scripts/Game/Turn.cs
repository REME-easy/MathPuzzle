using System.Collections.Generic;
using Scripts.Game;

namespace MathPuzzle.Scripts.Game
{
    public struct Turn
    {
        public Command Command;
        public List<Movement> Movements;
        public List<History> Histories;

        public Turn (Command command, List<Movement> movements, List<History> histories)
        {
            Command = command;
            Movements = movements;
            Histories = histories;
        }
    }
}