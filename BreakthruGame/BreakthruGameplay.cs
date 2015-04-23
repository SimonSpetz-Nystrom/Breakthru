using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Type = BreakthruGame.BreakthruGamePieceType;

namespace BreakthruGame
{
    #region Enum definition

    /// <summary>
    /// The different roles a player can take
    /// </summary>
    public enum PlayerRole { Gold, Silver };

    #endregion

    /// <summary>
    /// Class used to play a game of Breakthru. The class is designed to allow for the game to run in its own thread.
    /// </summary>
    public class BreakthruGameplay : INotifyPropertyChanged
    {
        #region Private variables

        private BreakthruGameBoard mBoard;
        private Player mGoldPlayer;
        private Player mSilverPlayer;
        private Tuple<Player, PlayerRole> mWinner = null;

        #endregion

        #region Public properties

        /// <summary>
        /// The winner of the game. Will be null until one player has won.
        /// </summary>
        public Tuple<Player, PlayerRole> Winner
        {
            get
            {
                return mWinner;
            }
            private set
            {
                mWinner = value;
                OnPropertyChanged("Winner");
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        /// <summary>
        /// Constructor taking the board and the two players as argument.
        /// The constructor assumes friendly parameters, e.g. no null values or bad class instances.
        /// </summary>
        /// <param name="board">The board to play on</param>
        /// <param name="goldPlayer">The player in the gold role</param>
        /// <param name="silverPlayer">The player in the silver role</param>
        public BreakthruGameplay(BreakthruGameBoard board, Player goldPlayer, Player silverPlayer)
        {
            mBoard = board;
            mGoldPlayer = goldPlayer;
            mSilverPlayer = silverPlayer;
        }

        /// <summary>
        /// Start to play the game. To allow for this method to be started as a thread it does not return anything.
        /// When the method ends (i.e. when a player wins) the Winner property (implements INotifyPropertyChanged) is changed to reflect who won. 
        /// </summary>
        public void PlayGame()
        {
            Player winner = null;

            // Start by placing all pieces.
            PlacePieces();

            // Let gold player select the starting player.
            // Set to the reverse of what is selected because player role is changed in loop.
            PlayerRole currentRole = mGoldPlayer.RequestStartingPlayer() == PlayerRole.Gold ? PlayerRole.Silver : PlayerRole.Gold;

            // Play game until one player has won.
            while ((winner = GetWinner()) == null)
            {
                currentRole = currentRole == PlayerRole.Gold ? PlayerRole.Silver : PlayerRole.Gold;
                PerformTurn(currentRole);
            }

            // The winner had the currentRole, because it has not been changed after the turn was performed.
            Winner = new Tuple<Player, PlayerRole>(winner, currentRole);
        }

        #region Private methods

        #region General methods

        /// <summary>
        /// Tests if the type of a piece belongs to a player role
        /// </summary>
        /// <param name="pieceType">The piece type</param>
        /// <param name="role">The player role</param>
        /// <returns>True if the piece belongs to the role</returns>
        private bool PieceTypeMatchesPlayerRole(Type pieceType, PlayerRole role)
        {
            return (role == PlayerRole.Silver && pieceType == Type.Silvership) || (role == PlayerRole.Gold && pieceType != Type.Silvership);
        }

        #endregion

        #region Placement phase

        /// <summary>
        /// Place all pieces (gold and silver). 
        /// Returns only when all pieces have been properly placed (unless unexpected exception is thrown before).
        /// </summary>
        private void PlacePieces()
        {
            int numberOfGoldPieces = (from BreakthruGamePiece p in mBoard.Pieces
                                      where PieceTypeMatchesPlayerRole(p.TypeOfPiece, PlayerRole.Gold)
                                      select p).Count();
            PlacementHelpFunction(IsValidGoldPlacement, mGoldPlayer, PlayerRole.Gold, numberOfGoldPieces);


            int numberOfSilverPieces = (from BreakthruGamePiece p in mBoard.Pieces
                                        where PieceTypeMatchesPlayerRole(p.TypeOfPiece, PlayerRole.Silver)
                                        select p).Count();
            PlacementHelpFunction(IsValidSilverPlacement, mSilverPlayer, PlayerRole.Silver, numberOfSilverPieces);
        }

        /// <summary>
        /// Help function to allow the same code to be used for placement of gold and silver pieces.
        /// </summary>
        /// <param name="validation">A function taking a piece and a position and returning true iff the move of the piece to the position is valid</param>
        /// <param name="player">The player to perform the placement</param>
        /// <param name="role">The role of the player to perform the placement</param>
        /// <param name="pieces">The amount of pieces to place</param>
        private void PlacementHelpFunction(Func<BreakthruGamePiece, Point, bool> validation, Player player, PlayerRole role, int pieces)
        {
            Move move = null;
            BreakthruGamePiece piece;
            Point position;
            bool legal;

            for (int i = 0; i < pieces; )
            {
                legal = true;

                // Infinite loop, ends when a valid move is received
                for (; ; )
                {
                    // Call different method depending on if last move was legal or not
                    move = legal ? player.RequestMove(mBoard, role) : player.IllegalMoveAttempt(move, mBoard, role);

                    piece = move.Piece as BreakthruGamePiece;
                    position = move.Destination;

                    // Assignment returns value that is being assigned, so if statement true iff validation returns true.
                    if (legal = validation(piece, position))
                    {
                        piece.Position = position;
                        i++;
                        break;
                    }
                }
            }
        }

        #region Placement validation functions

        /// <summary>
        /// Basic placement check that applies to all kinds of pieces. 
        /// If this method returns false, the placement is definitely illegal, no matter the piece type.
        /// This method should not be used directly but instead be used in all "real" validation functions for placement.
        /// </summary>
        /// <param name="piece">The piece to place</param>
        /// <param name="position">The position to place the piece in</param>
        /// <returns>True iff the move could be valid, depending on piece type</returns>
        private bool BasicPlacementCheck(BreakthruGamePiece piece, Point position)
        {
            return !mBoard.OnBoard(piece.Position) && mBoard.IsEmpty(position);
        }

        /// <summary>
        /// Validation function for placement of gold placement.
        /// </summary>
        /// <param name="piece">The piece to place</param>
        /// <param name="position">The position to place the piece in</param>
        /// <returns>True iff the placement is valid</returns>
        private bool IsValidGoldPlacement(BreakthruGamePiece piece, Point position)
        {
            if (!BasicPlacementCheck(piece, position)) return false;

            switch (piece.TypeOfPiece)
            {
                case Type.GoldEscort: return mBoard.InGoldArea(position) && position != BreakthruGameBoard.OriginalFlagshipPosition;
                case Type.GoldFlagship: return position == BreakthruGameBoard.OriginalFlagshipPosition;
            }

            return false;
        }

        /// <summary>
        /// Validation function for placement of gold placement.
        /// </summary>
        /// <param name="piece">The piece to place</param>
        /// <param name="position">The position to place the piece in</param>
        /// <returns>True iff the placement is valid</returns>
        private bool IsValidSilverPlacement(BreakthruGamePiece piece, Point position)
        {
            if (!BasicPlacementCheck(piece, position)) return false;

            return piece.TypeOfPiece == Type.Silvership && mBoard.OnBoard(position) && !mBoard.InGoldArea(position);
        }

        #endregion

        #endregion

        #region Play phase

        /// <summary>
        /// Perform a turn for a player role. 
        /// Returns when the player in the role has performed two moves, one capture or one flagship move (and some special cases when few pieces remain).
        /// </summary>
        /// <param name="role">The role of the player to perform a turn</param>
        private void PerformTurn(PlayerRole role)
        {
            Move move = null;
            BreakthruGamePiece piece;
            BreakthruGamePiece movedPiece = null;
            Point position;
            Player current = role == PlayerRole.Gold ? mGoldPlayer : mSilverPlayer;

            // Calculate number of movable pieces to allow for special cases.
            int numberOfMovablePieces = mBoard.Pieces.Count(p => mBoard.OnBoard(p.Position) && PieceTypeMatchesPlayerRole((p as BreakthruGamePiece).TypeOfPiece, role));

            // If no pieces can be moved just return, the turn of this player is done.
            // In this context we shall count the flagship, so this row must be before the next row.
            if (numberOfMovablePieces == 0) return;

            // Don't count flagship as movable because moving flagship is a special move that is more expensive.
            if (role == PlayerRole.Gold) numberOfMovablePieces--;

            bool firstMove = true;
            bool legal = true;
            // Infinite loop, will be ended by one of two return statements when all moves have been performed.
            for (; ; )
            {
                move = legal ? current.RequestMove(mBoard, role) : current.IllegalMoveAttempt(move, mBoard, role);
                piece = move.Piece as BreakthruGamePiece;
                position = move.Destination;

                if (mBoard.OnBoard(piece.Position) && piece != movedPiece && PieceTypeMatchesPlayerRole(piece.TypeOfPiece, role))
                {
                    if (IsValidMove(piece, position))
                    {
                        // Flagship can only be moved during first move.
                        legal = firstMove || piece.TypeOfPiece != Type.GoldFlagship;

                        if (legal)
                        {
                            piece.Position = position;
                            movedPiece = piece;

                            // Flagship moved, second move or only one movable piece means turn is over.
                            if (piece.TypeOfPiece == Type.GoldFlagship || !firstMove || numberOfMovablePieces == 1) return;
                        }
                    }
                    else if (firstMove && IsValidCapture(piece, position, role)) // Captures are only allowed on the first move
                    {
                        mBoard.RemoveFromBoard(mBoard.PieceAt(position));
                        piece.Position = position;

                        // No more move will be allowed, so return.
                        return;
                    }
                    else legal = false; // Not valid move or capture -> illegal
                }
                else legal = false; // Piece not on board, already moved or piece does not belong to player

                // The first move has been performed, so this is not the first move anymore.
                if (legal) firstMove = false;
            }
        }

        /// <summary>
        /// Checks if a move is a valid capture move, assuming the move is performed at the appropriate time by the appropriate player.
        /// </summary>
        /// <param name="piece">The piece to move</param>
        /// <param name="position">The position to move the piece to</param>
        /// <returns>True iff the move is valid regarding movement and piece capture</returns>
        private bool IsValidCapture(BreakthruGamePiece piece, Point position, PlayerRole role)
        {
            BreakthruGamePiece capturedPiece = (BreakthruGamePiece)mBoard.PieceAt(position);
            return
                Math.Abs(piece.Position.X - position.X) == 1 && Math.Abs(piece.Position.Y - position.Y) == 1 && // Piece moved one square diagonally
                capturedPiece != null && //There is a piece at the position moved to
                (
                ((piece.TypeOfPiece == Type.GoldFlagship || piece.TypeOfPiece == Type.GoldEscort) && capturedPiece.TypeOfPiece == Type.Silvership) || // Gold captures silver
                ((capturedPiece.TypeOfPiece == Type.GoldFlagship || capturedPiece.TypeOfPiece == Type.GoldEscort) && piece.TypeOfPiece == Type.Silvership) // Silver captures gold
                );
        }

        /// <summary>
        /// Checks if a move of a piece is valid. Only takes in account that the movement must be horizontal or vertical
        /// and only move over empty positions. Any other checks (e.g. piece owner)
        /// have to be performed elsewhere.
        /// </summary>
        /// <param name="piece">The piece to move</param>
        /// <param name="position">The position to move the piece to</param>
        /// <returns>True if the move is valid regarding path</returns>
        private bool IsValidMove(BreakthruGamePiece piece, Point position)
        {
            int max, min;

            if (piece.Position.X == position.X && piece.Position.Y != position.Y) // Horizontal
            {
                max = (int)Math.Max(position.Y, piece.Position.Y - 1);
                min = (int)Math.Min(position.Y, piece.Position.Y + 1);

                for (; min <= max; min++)
                {
                    if (mBoard.PieceAt(new Point(position.X, min)) != null) return false;
                }
                return true;
            }
            else if (piece.Position.Y == position.Y && piece.Position.X != position.X) // Vertical
            {
                max = (int)Math.Max(position.X, piece.Position.X - 1);
                min = (int)Math.Min(position.X, piece.Position.X + 1);

                for (; min <= max; min++)
                {
                    if (mBoard.PieceAt(new Point(min, position.Y)) != null) return false;
                }
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Get the winner. Returns null if there is no winner yet.
        /// </summary>
        /// <returns>The winning player, or null if there is no winner yet.</returns>
        private Player GetWinner()
        {
            Point flagshipPos = mBoard.GoldFlagship.Position;
            if (!mBoard.OnBoard(flagshipPos)) // Silver player wins when capturing the flagship (i.e. moves flagship outside of board)
            {
                return mSilverPlayer;
            }
            else if (flagshipPos.X == 0 || flagshipPos.Y == 0 || flagshipPos.X == mBoard.Width - 1 || flagshipPos.Y == mBoard.Height - 1) // Gold player wins when flagship is at the edge
            {
                return mGoldPlayer;
            }
            return null;
        }

        #endregion

        #endregion
    }
}
