using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreakthruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
namespace Test.BreakthruGame
{
    /// <summary>
    /// Tests for PosToTranslationConverter
    /// </summary>
    [TestClass()]
    public class TestPosToTranslationConverter
    {
        #region Private variables

        private PosToTranslationConverter mUnitUnderTest;

        #endregion

        #region Test initialization

        [TestInitialize]
        public void TestInitialize()
        {
            mUnitUnderTest = new PosToTranslationConverter();
        }

        #endregion

        #region Test methods

        [TestMethod()]
        public void ConvertTest()
        {
            // Arrange
            object[] values = new object[] { new Point(1, 2), 2.0 };

            // Act
            TranslateTransform result = mUnitUnderTest.Convert(values, null, null, null) as TranslateTransform;

            // Assert
            Assert.IsNotNull(result, "Wrong type or null");
            Assert.AreEqual(2, result.X, "Incorrect calculation of X");
            Assert.AreEqual(4, result.Y, "Incorrect calculation of Y");
        }

        [TestMethod(), ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackTest()
        {
            // Arrange done in TestInitialize

            // Act
            mUnitUnderTest.ConvertBack(null, null, null, null);

            // Assert done by ExpectedException
        }

        #endregion
    }
}
