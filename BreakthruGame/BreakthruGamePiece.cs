using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BreakthruGame
{
    /// <summary>
    /// Class describing a Breakthru piece. Apart from the general game piece functionality also has a breakthru game piece type.
    /// </summary>
    public class BreakthruGamePiece : GamePiece
    {
        /// <summary>
        /// The type of the piece
        /// </summary>
        public BreakthruGamePieceType TypeOfPiece { get; private set; }

        /// <summary>
        /// Constructor taking only type. Places the piece at a default position of (0,0)
        /// </summary>
        /// <param name="type">The type the piece shall be</param>
        public BreakthruGamePiece(BreakthruGamePieceType type) : this(type, new Point(0, 0)) { }

        /// <summary>
        /// Constructor taking all information necessary for a piece.
        /// </summary>
        /// <param name="type">The type the piece shall be</param>
        /// <param name="position">The position of the piece</param>
        public BreakthruGamePiece(BreakthruGamePieceType type, Point position)
        {
            this.TypeOfPiece = type;
            this.Position = position;
        }

    }
}
