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
		public void PutPiece_PlayerSquare_False()
		{
			var square = new CheckersSquare(3, 2);
			square.PutPiece (new CheckersPiece (CheckersPlayer.PlayerOne));

			Assert.IsFalse(square.PutPiece(new CheckersPiece(CheckersPlayer.PlayerOne)));
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

		[Test]
		public void RemovePiece_CurrentSquareNull_False()
		{
			var square = new CheckersSquare(3, 2);
		
			Assert.IsFalse(square.RemovePiece());
		}

		[Test]
		public void Equals_NotPiece_False()
		{
			var square = new CheckersSquare(3, 2);

			Assert.IsFalse(square.Equals("square"));
		}

		[Test]
		public void Equals_OtherDiffSquare_False()
		{
			var square = new CheckersSquare(3, 2);
			var other = new CheckersSquare(3, 3);

			Assert.IsFalse(square.Equals(other));
		}

		[Test]
		public void Equals_OtherEqualSquare_True()
		{
			var square = new CheckersSquare(3, 3);
			var other = new CheckersSquare(3, 3);

			Assert.IsTrue(square.Equals(other));
		}

		[Test]
		public void GetHashCode_DiffSquares_DiffHashes()
		{
			var square = new CheckersSquare(3, 3);
			var other = new CheckersSquare(3, 2);

			Assert.AreNotEqual (square.GetHashCode (), other.GetHashCode ());
		}
    }
}