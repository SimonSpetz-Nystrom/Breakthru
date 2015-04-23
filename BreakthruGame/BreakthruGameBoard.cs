using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Type = BreakthruGame.BreakthruGamePieceType;

namespace BreakthruGame
{
    /// <summary>
    /// Class representing a breakthru game board. Contains no actual logic for the game, shall be considered as a container only.
    /// </summary>
    public class BreakthruGameBoard : GameBoard
    {
        #region Private variables

        private List<GamePiece> mPieces;

        // Used to allow for removed pieces positions to not overlap.
        private int mSilverPiecesRemoved = 0;
        private int mGoldPiecesRemoved = 0;

        #endregion

        #region Public properties

        /// <summary>
        /// The position the flagship must be in when the game starts.
        /// </summary>
        public static readonly Point OriginalFlagshipPosition = new Point(5, 5);

        /// <summary>
        /// The width of the board.
        /// </summary>
        public override int Width
        {
            get { return 11; }
        }

        /// <summary>
        /// The height of the board
        /// </summary>
        public override int Height
        {
            get { return 11; }
        }

        /// <summary>
        /// An ICollection with all the pieces on this board.
        /// </summary>
        public override ICollection<GamePiece> Pieces
        {
            get
            {
                return (ICollection<GamePiece>)mPieces;
            }
        }

        /// <summary>
        /// Gets the gold flagship piece.
        /// </summary>
        public BreakthruGamePiece GoldFlagship
        {
            get
            {
                return (from BreakthruGamePiece p in Pieces
                        where p.TypeOfPiece == Type.GoldFlagship
                        select p).FirstOrDefault();
            }
        }

        #endregion

        /// <summary>
        /// Constructor. Will create all pieces and place them on default positions (outside of the board)
        /// </summary>
        public BreakthruGameBoard()
        {
            mPieces = new List<GamePiece>(33);

            // Add pieces without caring about positioning, then let ResetPieces fix positioning.
            for (int i = 0; i < 20; i++)
            {
                mPieces.Add(new BreakthruGamePiece(Type.Silvership));
            }
            for (int i = 0; i < 12; i++)
            {
                mPieces.Add(new BreakthruGamePiece(Type.GoldEscort));
            }
            mPieces.Add(new BreakthruGamePiece(Type.GoldFlagship));

            // This will position the pieces.
            ResetPieces();
        }

        #region Public methods

        /// <summary>
        /// Tests if a point is within the "gold area" (i.e. where the gold pieces are positioned at the start of the game).
        /// </summary>
        /// <param name="position">The postiion to test</param>
        /// <returns>True iff the position is in the gold area</returns>
        public bool InGoldArea(Point position)
        {
            return position.X >= 3 && position.X <= 7 && position.Y >= 3 && position.Y <= 7;
        }

        /// <summary>
        /// Resets the pieces to the default positions. 
        /// In the current solution gold pieces are placed to the left of the board (negative x values) and
        /// silver pieces are placed to the right of the board (x values >= Width)
        /// </summary>
        public void ResetPieces()
        {
            var silverPieces = (from BreakthruGamePiece p in Pieces
                                where p.TypeOfPiece == Type.Silvership
                                select p).ToList();
            // Silver pieces
            for (int i = 0; i < silverPieces.Count; i++)
            {
                // Place the pieces to the right of the board in two columns with ten pieces in each.
                silverPieces[i].Position = new Point(Width + i / 10, i % 10);
            }

            var goldPieces = (from BreakthruGamePiece p in Pieces
                              where p.TypeOfPiece == Type.GoldEscort
                              select p).ToList();
            // Gold escorts
            for (int i = 0; i < goldPieces.Count; i++)
            {
                // Place the pieces to the left of the board in two columns with six pieces in each.
                goldPieces[i].Position = new Point(-i / 6 - 1, i % 6 + 1);
            }

            // Gold flagship
            GoldFlagship.Position = new Point(-1.5, 0);

            mSilverPiecesRemoved = 0;
            mGoldPiecesRemoved = 0;
        }

        /// <summary>
        /// Remove a piece from the board. In the current solution gold pieces are placed to the right of the board and
        /// silver pieces to the left of the board (reverse from ResetPieces).
        /// Fails silently if the piece is not part of the board.
        /// </summary>
        /// <param name="piece">The piece to remove</param>
        public override void RemoveFromBoard(GamePiece piece)
        {
            BreakthruGamePiece p = piece as BreakthruGamePiece;
            if (p != null && mPieces.Contains(p))
            {
                switch (p.TypeOfPiece)
                {
                    case Type.GoldEscort:
                    case Type.GoldFlagship:
                        p.Position = new Point(Width + mGoldPiecesRemoved / 10, mGoldPiecesRemoved % 10);
                        mGoldPiecesRemoved++;
                        break;
                    case Type.Silvership:
                        p.Position = new Point(-mSilverPiecesRemoved / 10 - 1, mSilverPiecesRemoved % 10);
                        mSilverPiecesRemoved++;
                        break;
                }
            }
        }

        #endregion
    }
}
