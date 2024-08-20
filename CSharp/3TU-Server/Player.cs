using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    public class Player : IWinnable
    {
        public enum PlayerStates { Null, X, O }
        public PlayerStates State { get; set; }

        public Player.PlayerStates GetWinner()
        {
            return this.State;
        }
    }
}
