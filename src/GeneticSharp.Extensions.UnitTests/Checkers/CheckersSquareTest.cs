using System;
using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture]
    public class CheckersSquareTest
    {
        [Test]
        public void Constructor_ColumnAndRowIndex_FreeOrNotPlayable()
        {
            var target = new CheckersSquare(0, 0);
            Assert.AreEqual(CheckersSquareState.NotPlayable, target.State);

            target = new CheckersSquare(7, 7);
            Assert.AreEqual(CheckersSquareState.NotPlayable, target.State);

            target = new CheckersSquare(1, 0);
            Assert.AreEqual(CheckersSquareState.Free, target.State);

            target = new CheckersSquare(2, 0);
            Assert.AreEqual(CheckersSquareState.NotPlayable, target.State);

            target = new CheckersSquare(3, 0);
            Assert.AreEqual(CheckersSquareState.Free, target.State);

            target = new CheckersSquare(4, 0);
            Assert.AreEqual(CheckersSquareState.NotPlayable, target.State);
        }

        [Test]
        public void PutPiece_NoPlayableSquare_Exception()
        {
            var square = new CheckersSquare(0, 0);
            
            ExceptionAssert.IsThrowing(new ArgumentException("Attempt to put a piece in a not playable square."), () =>
            {
                square.PutPiece(new CheckersPiece(CheckersPlayer.PlayerOne));
            });
        }
    }
}