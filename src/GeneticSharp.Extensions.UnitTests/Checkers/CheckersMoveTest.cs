using System;
using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture]
    [Category("Extensions")]
    public class CheckersMoveTest
    {
        [Test]
        public void Constructor_NullPiece_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new CheckersMove(null, new CheckersSquare(0, 0));
            });

            Assert.AreEqual("piece", actual.ParamName);
        }

        [Test]
        public void Constructor_PieceWithoutCurrentSquare_Exception()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne), null);
            }, "A piece for a move should have a current square defined.");


        }

        [Test]
        public void Constructor_ToSquareNull_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(0, 1) }, null);
            });

            Assert.AreEqual("toSquare", actual.ParamName);
        }
    }
}