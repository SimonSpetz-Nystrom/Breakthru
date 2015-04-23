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
    /// <summary>
    /// Tests for BreakthruGameBoard
    /// </summary>
    [TestClass()]
    public class TestBreakthruGameBoard
    {
        #region Private variables

        private BreakthruGameBoard mUnitUnderTest;

        #endregion

        #region Test initialization

        [TestInitialize]
        public void TestInitialize()
        {
            mUnitUnderTest = new BreakthruGameBoard();
        }

        #endregion

        #region Test methods

        [TestMethod()]
        public void ConstructorTest()
        {
            // Constructor called in test initialize.

            // The constructor shall initialize the board with 33 pieces.
            Assert.AreEqual(33, mUnitUnderTest.Pieces.Count);
        }

        [TestMethod()]
        public void InGoldAreaTest()
        {
            // Some false cases
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(0, 0)));
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(2, 2)));
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(2, 3)));
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(3, 2)));
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(8, 8)));
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(7, 8)));
            Assert.IsFalse(mUnitUnderTest.InGoldArea(new Point(8, 7)));

            // Some true cases
            Assert.IsTrue(mUnitUnderTest.InGoldArea(new Point(3, 3)));
            Assert.IsTrue(mUnitUnderTest.InGoldArea(new Point(5, 5)));
            Assert.IsTrue(mUnitUnderTest.InGoldArea(new Point(7, 7)));

        }

        [TestMethod()]
        public void ResetPiecesTest()
        {
            // Arrange
            Point notExpected = new Point(5, 5);
            foreach (var p in mUnitUnderTest.Pieces)
            {
                p.Position = notExpected;
            }

            // Act
            mUnitUnderTest.ResetPieces();

            // Assert
            foreach (var p in mUnitUnderTest.Pieces)
            {
                Assert.AreNotEqual(notExpected, p.Position, "Piece not moved");
            }
        }

        [TestMethod()]
        public void RemoveFromBoardTest()
        {
            // Arrange
            Point onBoard = new Point(5, 5);
            GamePiece p = mUnitUnderTest.Pieces.FirstOrDefault();
            p.Position = onBoard;

            // Act
            mUnitUnderTest.RemoveFromBoard(p);

            // Assert
            Assert.AreNotEqual(onBoard, p.Position, "Position not changed");
            Assert.IsFalse(mUnitUnderTest.OnBoard(p.Position), "New position on board");
        }

        [TestMethod()]
        public void OnBoardTest()
        {
            // Arrange
            Dictionary<Point, bool> testCases = new Dictionary<Point, bool>
            {
                // True cases    
                {new Point(0,0), true},
                {new Point(10,10), true},
                {new Point(5,5), true},
                {new Point(0,10), true},
                {new Point(10, 0), true},

                // False cases
                {new Point(-1,0), false},
                {new Point(0,-1), false},
                {new Point(0,11), false},
                {new Point(11,0), false}
            };

            // Act + Assert
            foreach (var testCase in testCases)
            {
                Assert.AreEqual(testCase.Value, mUnitUnderTest.OnBoard(testCase.Key));
            }

        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            // Arrange
            Point occupied = new Point(5, 5);
            mUnitUnderTest.Pieces.First().Position = occupied;
            Point empty = new Point(0, 0);

            // Act + Assert
            Assert.IsTrue(mUnitUnderTest.IsEmpty(empty));
            Assert.IsFalse(mUnitUnderTest.IsEmpty(occupied));
        }

        [TestMethod()]
        public void PieceAtTest()
        {
            // Arrange
            GamePiece p = mUnitUnderTest.Pieces.First();
            Point position = new Point(5, 5);
            p.Position = position;
            Point empty = new Point(0, 0);

            // Act + Assert
            Assert.AreEqual(p, mUnitUnderTest.PieceAt(position));
            Assert.IsNull(mUnitUnderTest.PieceAt(empty));
        }

        #endregion
    }
}
