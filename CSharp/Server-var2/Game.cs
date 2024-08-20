using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal class Game:TTT
    {
        private Field[] gameFields;
        private GameStatus[] fieldStatuses;

        private int nextField = 0;
        private char nextPlayer = 'X';
        
        public int NextField { get { return nextField; } }
        public char NextPlayer { get { return nextPlayer; } }

        public GameStatus State { get; private set; }

        public string GetBoardAsString()
        {
            string board = "";

            // row 1 from field 1, then row 1 from field 2, then row 1 from field 3, then row 2 from field 1, etc.

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        board += gameFields[i * 3 + k].GetRow(j);
                    }
                }
            }

            return board;
        }

        public string GetFieldStatuses()
        {
            string fieldStatusesString = "";

            for (int i = 0; i < fieldStatuses.Length; i++)
            {
                char c = ' ';

                switch (fieldStatuses[i])
                {
                    case GameStatus.X:
                        c = 'X';
                        break;
                    case GameStatus.O:
                        c = 'O';
                        break;
                    case GameStatus.Tie:
                        c = 'T';
                        break;
                    default:
                        c = '_';
                        break;
                }

                fieldStatusesString += c;
            }

            return fieldStatusesString;
        }

        public Game()
        {
            gameFields = InitGameFields();
            fieldStatuses = new GameStatus[9];
        }

        public void ResetGame()
        {
            gameFields = InitGameFields();
            fieldStatuses = new GameStatus[9];
        }

        public static Field[] InitGameFields()
        {
            Field[] fields = new Field[9];

            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = new Field();
            }

            return fields;
        }

        // returns what field to play next in. 0 if every field is allowed. -1 if the input notation is invalid.
        public int PlacePlayer(string notation)
        {
            char player = notation[0];
            int targetField = notation[1] - '0' - 1;
            int targetCell = notation[2] - '0' - 1;

            if (targetField < 0 || targetField  >  8 || gameFields[targetField].State != GameStatus.None)
            {
                return -1;
            }

            if (player != nextPlayer)
            {
                // Client should resync
                return -2;
            }

            if (gameFields[targetField].PlacePlayer(player, targetCell) == true)
            {
                if (gameFields[targetField].State != GameStatus.None)
                {
                    fieldStatuses[targetField] = gameFields[targetField].State;
                    State = CheckWin();
                }

                nextPlayer = player == 'X' ? 'O' : 'X';

                nextField = GetNextField(targetCell);
                return nextField;
            }

            return -1;
        }

        private int GetNextField(int target)
        {
            if (gameFields[target].State != GameStatus.None)
            {
                return 0;
            }

            return target+1;
        }

        protected override GameStatus CheckWin()
        {
            /* Check rows and columns */
            for (int i = 0; i < 3; i++)
            {
                /* Check rows */
                if (fieldStatuses[i * 3] == fieldStatuses[i * 3 + 1] && fieldStatuses[i * 3] == fieldStatuses[i * 3 + 2])
                {
                    if (fieldStatuses[i * 3] != GameStatus.None)
                    {
                        return fieldStatuses[i * 3];
                    }
                }
                /* Check columns */
                if (fieldStatuses[i] == fieldStatuses[i + 3] && fieldStatuses[i] == fieldStatuses[i + 6])
                {
                    if (fieldStatuses[i] != GameStatus.None)
                    {
                        return fieldStatuses[i];
                    }
                }
            }

            /* Check diagonals */
            if (fieldStatuses[0] == fieldStatuses[4] && fieldStatuses[0] == fieldStatuses[8])
            {
                if (fieldStatuses[0] != GameStatus.None)
                {
                    return fieldStatuses[0];
                }
            }
            if (fieldStatuses[2] == fieldStatuses[4] && fieldStatuses[2] == fieldStatuses[6])
            {
                if (fieldStatuses[2] != GameStatus.None)
                {
                    return fieldStatuses[2];
                }
            }

            /* Check for tie */
            if (fieldStatuses.All(x => x != GameStatus.None))
            {
                return GameStatus.Tie;
            }

            return GameStatus.None;
        }
    }
}
