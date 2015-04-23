using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BreakthruGame
{
    /// <summary>
    /// Simple class to describe the move of a game piece.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Constructor. Takes all details to describe a move as arguments.
        /// </summary>
        /// <param name="piece">The piece to move</param>
        /// <param name="destination">The point to move the piece to</param>
        public Move(GamePiece piece, Point destination)
        {
            Piece = piece;
            Destination = destination;
        }

        /// <summary>
        /// The piece to move.
        /// </summary>
        public GamePiece Piece { get; set; }

        /// <summary>
        /// The point to move the piece to
        /// </summary>
        public Point Destination { get; set; }
    }
}
