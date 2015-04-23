using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreakthruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
namespace Test.BreakthruGame
{
    [TestClass()]
    public class TestExternalPlayer
    {
        #region Private variables/constants

        private bool mMoveRequestExpected = false;
        private bool mStartingPlayerRequestExpected = false;
        private bool mIllegalMoveAttemptRequested = false;

        private const PlayerRole ExpectedPlayerRole = PlayerRole.Silver;
        private readonly Move ExpectedMove = new Move(new BreakthruGamePiece(BreakthruGamePieceType.GoldEscort), new Point(11, 11));
        private readonly BreakthruGameBoard ExpectedBoard = new BreakthruGameBoard();

        private ExternalPlayer mUnitUnderTest;

        #endregion

        #region Test initialization

        [TestInitialize]
        public void TestInitialize()
        {
            mUnitUnderTest = new ExternalPlayer();
            mUnitUnderTest.IllegalMoveAttempted += IllegalMoveAttempted;
            mUnitUnderTest.MoveRequested += MoveRequested;
            mUnitUnderTest.StartingPlayerRequested += StartingPlayerRequested;
        }

        #endregion

        #region Test methods

        [TestMethod()]
        public void RequestMoveTest()
        {
            // Arrange (partly done in TestInitialize)
            mMoveRequestExpected = true;

            // Act + Assert (partly done in event handler)
            Assert.AreEqual(ExpectedMove, mUnitUnderTest.RequestMove(ExpectedBoard, ExpectedPlayerRole));
        }

        [TestMethod()]
        public void RequestStartingPlayerTest()
        {
            // Arrange (partly done in TestInitialize)
            mStartingPlayerRequestExpected = true;

            // Act + Assert (partly done in event handler)
            Assert.AreEqual(ExpectedPlayerRole, mUnitUnderTest.RequestStartingPlayer());
        }

        [TestMethod()]
        public void IllegalMoveAttemptTest()
        {
            // Arrange (partly done in TestInitialize)
            mIllegalMoveAttemptRequested = true;

            // Act + Assert (partly done in event handler)
            Assert.AreEqual(ExpectedMove, mUnitUnderTest.IllegalMoveAttempt(ExpectedMove, ExpectedBoard, ExpectedPlayerRole));
        }

        #endregion

        #region Private help methods (event handlers)

        private PlayerRole StartingPlayerRequested(object sender, EventArgs e)
        {
            Assert.IsTrue(mStartingPlayerRequestExpected);

            mStartingPlayerRequestExpected = false;
            return ExpectedPlayerRole;
        }

        private Move MoveRequested(object sender, MoveRequestEventArgs e)
        {
            Assert.IsTrue(mMoveRequestExpected);
            Assert.AreEqual(ExpectedBoard, e.Board);
            Assert.AreEqual(ExpectedPlayerRole, e.Role);

            mMoveRequestExpected = false;
            return ExpectedMove;
        }

        private Move IllegalMoveAttempted(object sender, IllegalMoveEventArgs e)
        {
            Assert.IsTrue(mIllegalMoveAttemptRequested);
            Assert.AreEqual(ExpectedMove, e.Move);
            Assert.AreEqual(ExpectedBoard, e.Board);
            Assert.AreEqual(ExpectedPlayerRole, e.Role);

            mIllegalMoveAttemptRequested = false;
            return ExpectedMove;
        }

        #endregion

    }
}
