using System;
using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture]
    [Category("Extensions")]
    public class CheckersMoveTest
    {
        [Test]
        public void Constructor_NullPiece_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("piece"), () =>
            {
                new CheckersMove(null, new CheckersSquare(0, 0));
            });
        }

        [Test]
        public void Constructor_PieceWithoutCurrentSquare_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("A piece for a move should have a current square defined."), () =>
            {
                new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne), null);
            });
        }

        [Test]
        public void Constructor_ToSquareNull_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("toSquare"), () =>
            {
                new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(0, 1) }, null);
            });
        }
    }
}