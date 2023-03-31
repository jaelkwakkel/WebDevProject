using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Setup.Models.Tests
{
    [TestClass()]
    public class GameGroupTests
    {
        [TestMethod()]
        public void CreateGameBoardCorrectLength(int length)
        {
            var gameBoard = GameGroup.CreateGameBoard(10);
            Assert.AreEqual(gameBoard.Length, 10);
        }

        [TestMethod()]
        public void CreateGameBoardDefaultBuildingTypeIsGrass()
        {
            var gameBoard = GameGroup.CreateGameBoard(10);
            for (int i = 0; i < gameBoard.Length; i++)
            {
                for (int j = 0; j < gameBoard[i].Length; j++)
                {
                    if (gameBoard[i][j].BuildingType == BuildingType.Grass)
                    {
                        Assert.Fail();
                    }
                }
            }
            Assert.IsTrue(gameBoard.Length == 10);
        }
    }
}