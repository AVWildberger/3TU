using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    public abstract class TTT
    {
        public enum GameStatus {
            None,
            X,
            O,
            Tie
        }

        protected abstract GameStatus CheckWin();
    }
}
