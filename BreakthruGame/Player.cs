using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakthruGame
{
    /// <summary>
    /// An abstract player class for Breakthru players. 
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Requests a move from the player.
        /// </summary>
        /// <param name="board">The board to perform the move on</param>
        /// <param name="role">The role of the player in the current context</param>
        /// <returns>A Tuple containing the piece to move and the position to move the piece to, or null if the player refuses to move.</returns>
        public abstract Move RequestMove(BreakthruGameBoard board, PlayerRole role);

        /// <summary>
        /// Called when player has tried an illegal move and a new (preferrably legal) move is requested.
        /// </summary>
        /// <param name="move">The illegal move attempted</param>
        /// <param name="board">The board the move was attempted on</param>
        /// <param name="role">The role of the player in the current context</param>
        /// <returns>A new move that will be attempted</returns>
        public abstract Move IllegalMoveAttempt(Move move, BreakthruGameBoard board, PlayerRole role);

        /// <summary>
        /// Requests a decision about starting player from the gold player.
        /// </summary>
        /// <returns>The player role that shall start the game</returns>
        public abstract PlayerRole RequestStartingPlayer();
    }
}
