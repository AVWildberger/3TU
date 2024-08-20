using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal class States : IWinnable
    {
        public enum State { None, Won, Tie }

        public State Status { get; set; }
        public Player.PlayerStates Winner { get; set; }

        public Player.PlayerStates GetWinner()
        {
            return Winner;
        }
    }
}
