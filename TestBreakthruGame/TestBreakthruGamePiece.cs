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
    /// Tests for BreakthruGamePiece
    /// </summary>
    [TestClass()]
    public class TestBreakthruGamePiece
    {
        #region Test methods

        [TestMethod()]
        public void Constructor1Test()
        {
            // Arrange + Act
            BreakthruGamePiece flagship = new BreakthruGamePiece(BreakthruGamePieceType.GoldFlagship);
            BreakthruGamePiece goldEscort = new BreakthruGamePiece(BreakthruGamePieceType.GoldEscort);
            BreakthruGamePiece silverShip = new BreakthruGamePiece(BreakthruGamePieceType.Silvership);

            // Assert
            Assert.AreEqual(BreakthruGamePieceType.GoldFlagship, flagship.TypeOfPiece);
            Assert.AreEqual(BreakthruGamePieceType.GoldEscort, goldEscort.TypeOfPiece);
            Assert.AreEqual(BreakthruGamePieceType.Silvership, silverShip.TypeOfPiece);
        }

        [TestMethod()]
        public void Constructor2Test()
        {
            // Arrange + Act
            Point flagshipPos = new Point(5, 5);
            Point goldEscortPos = new Point(0, 0);
            Point silverShipPos = new Point(10, 10);
            BreakthruGamePiece flagship = new BreakthruGamePiece(BreakthruGamePieceType.GoldFlagship, flagshipPos);
            BreakthruGamePiece goldEscort = new BreakthruGamePiece(BreakthruGamePieceType.GoldEscort, goldEscortPos);
            BreakthruGamePiece silverShip = new BreakthruGamePiece(BreakthruGamePieceType.Silvership, silverShipPos);

            // Assert
            Assert.AreEqual(BreakthruGamePieceType.GoldFlagship, flagship.TypeOfPiece);
            Assert.AreEqual(BreakthruGamePieceType.GoldEscort, goldEscort.TypeOfPiece);
            Assert.AreEqual(BreakthruGamePieceType.Silvership, silverShip.TypeOfPiece);

            Assert.AreEqual(flagshipPos, flagship.Position);
            Assert.AreEqual(goldEscortPos, goldEscort.Position);
            Assert.AreEqual(silverShipPos, silverShip.Position);
        }

        #endregion
    }
}
