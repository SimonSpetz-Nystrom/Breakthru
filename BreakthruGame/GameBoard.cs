using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BreakthruGame
{
    /// <summary>
    /// An abstract class describing a rectangle (Width * Height) game board with a number of pieces. 
    /// In the default version positions are zero-based, i.e. the lowest x/y values are 0. This can be changed by any concrete subclass.
    /// </summary>
    public abstract class GameBoard
    {
        /// <summary>
        /// Width of the board.
        /// </summary>
        public abstract int Width { get; }
        /// <summary>
        /// Height of the board.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// The pieces currently on the board.
        /// </summary>
        public abstract ICollection<GamePiece> Pieces { get; }

        /// <summary>
        /// Removes a piece from the board.
        /// </summary>
        /// <param name="piece"></param>
        public abstract void RemoveFromBoard(GamePiece piece);

        /// <summary>
        /// Checks if a point is on the board.
        /// </summary>
        /// <param name="position">The point to test.</param>
        /// <returns>True iff the point is on the board.</returns>
        public virtual bool OnBoard(Point position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        /// <summary>
        /// Checks if a point is empty or not.
        /// </summary>
        /// <param name="position">The point to check</param>
        /// <returns>True iff there is no piece on the position</returns>
        public virtual bool IsEmpty(Point position)
        {
            return !Pieces.Any(p => p.Position == position);
        }

        /// <summary>
        /// Get the piece at a position.
        /// </summary>
        /// <param name="position">The position to get the piece from</param>
        /// <returns>The piece at the position, or null if there is no such piece</returns>
        public virtual GamePiece PieceAt(Point position)
        {
            return Pieces.FirstOrDefault(p => p.Position == position);
        }
    }
}
