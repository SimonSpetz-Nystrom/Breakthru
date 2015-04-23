using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreakthruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Test.BreakthruGame
{
    /// <summary>
    /// Tests for BreakthruGameplay
    /// </summary>
    [TestClass()]
    public class TestBreakthruGameplay
    {
        #region Test methods

        [TestMethod()]
        public void ConstructorTest()
        {
            // Arrange + Act
            BreakthruGameplay unitUnderTest = new BreakthruGameplay(new BreakthruGameBoard(), new PlayerStub(), new PlayerStub());

            // Assert
            Assert.IsNull(unitUnderTest.Winner);
        }

        [TestMethod()]
        public void PlayGameTest()
        {
            // Arrange
            PlayerStub player = new PlayerStub();
            BreakthruGameplay unitUnderTest = new BreakthruGameplay(new BreakthruGameBoard(), player, player);

            // Act
            unitUnderTest.PlayGame();

            // Assert (most assertions made in PlayerStub)
            Assert.IsNotNull(unitUnderTest.Winner, "No winner.");
            Assert.AreEqual(unitUnderTest.Winner.Item2, PlayerRole.Gold, "Wrong winner");
        }

        #endregion
    }
}
