using BreakthruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestMove = System.Tuple<System.Windows.Point, System.Windows.Point>;

namespace Test.BreakthruGame
{
    /// <summary>
    /// Stub player class to test game play. To be used as both gold and silver player.
    /// Performs a number of legal and illegal move with assertions to make sure everything works as expected.
    /// When this player has finished playing agains itself gold player should have won.
    /// </summary>
    class PlayerStub : Player
    {
        #region Private variables/constants

        private readonly List<TestMove> mGoldMoves = new List<TestMove>{
            // Placement
            new TestMove(new Point(-1.5, 0), new Point(4,5)), // Illegal (0)
            new TestMove(new Point(-1.5, 0), new Point(5,5)),
            new TestMove(new Point(-1, 1), new Point(3,2)), // Illegal (2)
            new TestMove(new Point(-1, 1), new Point(4,3)),
            new TestMove(new Point(-1, 2), new Point(4,4)),
            new TestMove(new Point(-1, 3), new Point(4,5)),
            new TestMove(new Point(-1, 4), new Point(4,6)),
            new TestMove(new Point(-1, 5), new Point(4,7)),
            new TestMove(new Point(-1, 6), new Point(3,3)),
            new TestMove(new Point(-2, 1), new Point(3,7)),
            new TestMove(new Point(-2, 2), new Point(7,3)),
            new TestMove(new Point(-2, 3), new Point(7,4)),
            new TestMove(new Point(-2, 4), new Point(7,5)),
            new TestMove(new Point(-2, 5), new Point(7,6)),
            new TestMove(new Point(-2, 6), new Point(7,7)),

            // Gameplay
            new TestMove(new Point(3, 3), new Point(2,2)),
            new TestMove(new Point(7, 3), new Point(7,0)),
            new TestMove(new Point(4, 3), new Point(7,3)),
            new TestMove(new Point(5, 5), new Point(2,5)), // Illegal (18)
            new TestMove(new Point(5, 5), new Point(5,0)) // Wins game
        };
        private readonly List<int> mIllegalGoldMoves = new List<int> { 0, 2, 18 };

        private readonly List<TestMove> mSilverMoves = new List<TestMove>{
            // Placement
            new TestMove(new Point(11,0), new Point(5,5)), // Illegal (0)
            new TestMove(new Point(11,0), new Point(3,3)), // Illegal (1)
            new TestMove(new Point(11,0), new Point(2,2)),
            new TestMove(new Point(11,1), new Point(2,8)),
            new TestMove(new Point(11,2), new Point(4,2)),
            new TestMove(new Point(11,3), new Point(0,0)),
            new TestMove(new Point(11,4), new Point(0,1)),
            new TestMove(new Point(11,5), new Point(0,2)),
            new TestMove(new Point(11,6), new Point(0,3)),
            new TestMove(new Point(11,7), new Point(0,4)),
            new TestMove(new Point(11,8), new Point(0,5)),
            new TestMove(new Point(11,9), new Point(0,6)),
            new TestMove(new Point(12,0), new Point(0,7)),
            new TestMove(new Point(12,1), new Point(0,8)),
            new TestMove(new Point(12,2), new Point(0,9)),
            new TestMove(new Point(12,3), new Point(0,10)),
            new TestMove(new Point(12,4), new Point(1,0)),
            new TestMove(new Point(12,5), new Point(1,1)),
            new TestMove(new Point(12,6), new Point(1,2)),
            new TestMove(new Point(12,7), new Point(1,3)),
            new TestMove(new Point(12,8), new Point(1,4)),
            new TestMove(new Point(12,9), new Point(1,5)),

            // Gameplay
            new TestMove(new Point(1,5), new Point(3,5)),
            new TestMove(new Point(1,4), new Point(1,10)),
            new TestMove(new Point(2,8), new Point(3,7)),
            new TestMove(new Point(3,7), new Point(5,5)), // Illegal (25)
            new TestMove(new Point(3,7), new Point(4,6)), 
        };
        private readonly List<int> mIllegalSilverMoves = new List<int> { 0, 1, 25 };

        private bool mIllegalExpected = false;
        private int mSilverCount = 0;
        private int mGoldCount = 0;

        #endregion

        #region Player implementation

        /// <summary>
        /// Implementation of Player.RequestMove. Performs a predefined move and performs some
        /// assertions to ensure everything works as expected.
        /// </summary>
        /// <param name="board">The board the game is played on</param>
        /// <param name="role">The current role of the player</param>
        /// <returns>The current predefined test move</returns>
        public override Move RequestMove(BreakthruGameBoard board, PlayerRole role)
        {
            int index;
            List<int> illegalMoveList;
            List<TestMove> moveList;
            GetVariablesForRole(role, out index, out moveList, out illegalMoveList);

            Assert.IsFalse(mIllegalExpected, String.Format("Expected illegal, move {0} for {1}.", index, role));
            Assert.IsTrue(moveList.Count > index, String.Format("Game not ended. ({0} {1})", role, index));

            TestMove move = moveList[index];
            mIllegalExpected = illegalMoveList.Contains(index);

            return new Move(board.PieceAt(move.Item1), move.Item2);
        }

        /// <summary>
        /// Implementation of Player.IllegalMoveAttempt. See description of PlayerStub.RequestMove for behavior.
        /// </summary>
        public override Move IllegalMoveAttempt(Move move, BreakthruGameBoard board, PlayerRole role)
        {
            int index;
            List<int> illegalMoveList;
            List<TestMove> moveList;
            GetVariablesForRole(role, out index, out moveList, out illegalMoveList);

            Assert.IsTrue(mIllegalExpected, String.Format("Expected legal, move {0} for {1}.", index, role));
            Assert.IsTrue(moveList.Count > index, String.Format("Game not ended. ({0} {1})", role, index));

            TestMove newMove = moveList[index];
            mIllegalExpected = illegalMoveList.Contains(index);

            return new Move(board.PieceAt(newMove.Item1), newMove.Item2);

        }

        /// <summary>
        /// Implementation of Player.RequestStartingPlayer. Returns PlayerRole.Silver.
        /// </summary>
        /// <returns>PlayerRole.Silver</returns>
        public override PlayerRole RequestStartingPlayer()
        {
            return PlayerRole.Silver;
        }

        #endregion

        #region Private help methods

        /// <summary>
        /// Get the variables associated with a specific role.
        /// </summary>
        /// <param name="role">The role the variables shall be associated to</param>
        /// <param name="index">The current move index of the role</param>
        /// <param name="moves">The list of moves the role performs</param>
        /// <param name="illegalMoves">The list of indices for illegal moves that this role will attempt to perform</param>
        private void GetVariablesForRole(PlayerRole role, out int index, out List<TestMove> moves, out List<int> illegalMoves)
        {
            if (role == PlayerRole.Gold)
            {
                index = mGoldCount++;
                moves = mGoldMoves;
                illegalMoves = mIllegalGoldMoves;
            }
            else
            {
                index = mSilverCount++;
                moves = mSilverMoves;
                illegalMoves = mIllegalSilverMoves;
            }
        }

        #endregion
    }
}
