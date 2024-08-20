using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal interface IWinnable
    {
        public Player.PlayerStates GetWinner();
    }
}
