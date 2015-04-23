using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;

namespace BreakthruGame
{
    #region Event related (Delegates and EventArgs implementations)

    /// <summary>
    /// Describes event arguments for move requests.
    /// </summary>
    public class MoveRequestEventArgs : EventArgs
    {
        public MoveRequestEventArgs(BreakthruGameBoard board, PlayerRole role)
        {
            Board = board;
            Role = role;
        }

        public BreakthruGameBoard Board { get; private set; }
        public PlayerRole Role { get; private set; }
    }
    public delegate Move MoveRequestEventHandler(object sender, MoveRequestEventArgs e);

    /// <summary>
    /// Describes event arguments for illegal move events.
    /// </summary>
    public class IllegalMoveEventArgs : EventArgs
    {
        public IllegalMoveEventArgs(Move move, BreakthruGameBoard board, PlayerRole role)
        {
            Move = move;
            Board = board;
            Role = role;
        }

        public Move Move { get; private set; }
        public BreakthruGameBoard Board { get; private set; }
        public PlayerRole Role { get; private set; }
    }
    public delegate Move IllegalMoveEventHandler(object sender, IllegalMoveEventArgs e);

    public delegate PlayerRole StartingPlayerRequestHandler(object sender, EventArgs e);

    #endregion

    /// <summary>
    /// A class describing a player that does not perform any actual decisions, but lets someone else do them.
    /// An example of usage of this class is letting a user do the moves, e.g. via some kind of GUI.
    /// 
    /// This class raises events that someone is supposed to handle and then performs the moves reported by the event handler.
    /// </summary>
    public class ExternalPlayer : Player
    {
        /// <summary>
        /// Raised when the player has received a move request.
        /// </summary>
        public event MoveRequestEventHandler MoveRequested;

        /// <summary>
        /// Request a move from this player.
        /// </summary>
        /// <param name="board">The board the move is to be performed on.</param>
        /// <param name="role">The role of the player</param>
        /// <returns>A move, or null if no external player (event handler) is available</returns>
        public override Move RequestMove(BreakthruGameBoard board, PlayerRole role)
        {
            MoveRequestEventHandler handler = MoveRequested;
            if (handler != null)
            {
                return handler(this, new MoveRequestEventArgs(board, role));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Raised when starting player is requested. Should only be raised for gold players.
        /// </summary>
        public event StartingPlayerRequestHandler StartingPlayerRequested;

        /// <summary>
        /// Request a starting player
        /// </summary>
        /// <returns>The player role chosen, or Gold if no external player (event handler) is available</returns>
        public override PlayerRole RequestStartingPlayer()
        {
            StartingPlayerRequestHandler handler = StartingPlayerRequested;
            if (handler != null)
            {
                return handler(this, new EventArgs());
            }
            else
            {
                // Always start if no external player is there to decide
                return PlayerRole.Gold;
            }
        }

        /// <summary>
        /// Raised when the player has attempted an illegal move
        /// </summary>
        public event IllegalMoveEventHandler IllegalMoveAttempted;

        /// <summary>
        /// Tell the player an illegal move has been attempted and request a new move.
        /// </summary>
        /// <param name="move">The previous, illegal, move</param>
        /// <param name="board">The board to perform a move on</param>
        /// <param name="role">The role of the player</param>
        /// <returns>A move, or null if no external player (event handler) is available</returns>
        public override Move IllegalMoveAttempt(Move move, BreakthruGameBoard board, PlayerRole role)
        {
            IllegalMoveEventHandler handler = IllegalMoveAttempted;
            if (handler != null)
            {
                return handler(this, new IllegalMoveEventArgs(move, board, role));
            }
            else
            {
                return null;
            }
        }

    }
}
