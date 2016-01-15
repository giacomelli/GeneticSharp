using System;
using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture()]
    [Category("Extensions")]
    public class CheckersBoardTest
    {
        [Test()]
        public void Constructos_InvalidSize_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("The minimum valid size is 8."), () =>
            {
                new CheckersBoard(7);
            });

            ExceptionAssert.IsThrowing(new ArgumentException("The minimum valid size is 8."), () =>
            {
                new CheckersBoard(-8);
            });
        }

        [Test()]
        public void Contructor_ValidSize_PlayerOnePiecedPlaced()
        {
            var target = new CheckersBoard(8);
            Assert.AreEqual(8, target.Size);

            // First row.
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(1, 0).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(3, 0).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(5, 0).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(7, 0).State);

            // second row
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(0, 1).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(2, 1).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(4, 1).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(6, 1).State);

            // third row
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(1, 2).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(3, 2).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(5, 2).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerOne, target.GetSquare(7, 2).State);
        }

        [Test()]
        public void Contructor_ValidSize_PlayerTwoPiecedPlaced()
        {
            var target = new CheckersBoard(8);
            Assert.AreEqual(8, target.Size);

            // first row.
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(0, 7).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(2, 7).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(4, 7).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(6, 7).State);

            // second row            
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(1, 6).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(3, 6).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(5, 6).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(7, 6).State);

            // third row
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(0, 5).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(2, 5).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(4, 5).State);
            Assert.AreEqual(CheckersSquareState.OccupiedByPlayerTwo, target.GetSquare(6, 5).State);
        }

        [Test()]
        public void Contructor_ValidSize_FreeAndNotPlayableSquaresOk()
        {
            var target = new CheckersBoard(8);
            Assert.AreEqual(8, target.Size);

            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 8; r++)
                {
                    var notPlayable = CheckersSquare.IsNotPlayableSquare(c, r);
                    var actual = target.GetSquare(c, r).State;

                    if (notPlayable)
                    {
                        Assert.AreEqual(CheckersSquareState.NotPlayable, actual);
                    }
                    else
                    {
                        Assert.AreNotEqual(CheckersSquareState.NotPlayable, actual);
                    }
                }
            }
        }

        [Test]
        public void IsNotPlayableSquare_DiffSquares_DiffResults()
        {
            Assert.IsTrue(CheckersSquare.IsNotPlayableSquare(0, 0));
            Assert.IsFalse(CheckersSquare.IsNotPlayableSquare(0, 1));
            Assert.IsTrue(CheckersSquare.IsNotPlayableSquare(0, 2));

            Assert.IsFalse(CheckersSquare.IsNotPlayableSquare(1, 0));
            Assert.IsTrue(CheckersSquare.IsNotPlayableSquare(1, 1));
            Assert.IsFalse(CheckersSquare.IsNotPlayableSquare(1, 2));
        }

        [Test()]
        public void GetSize_InvalidIndexes_Exception()
        {
            var target = new CheckersBoard(10);

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("columnIndex"), () =>
            {
                target.GetSquare(-1, 0);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("columnIndex"), () =>
            {
                target.GetSquare(10, 0);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("rowIndex"), () =>
            {
                target.GetSquare(0, -1);
            });

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("rowIndex"), () =>
            {
                target.GetSquare(0, 10);
            });
        }

        [Test()]
        public void MovePiece_NullMove_Exception()
        {
            var target = new CheckersBoard(10);

            ExceptionAssert.IsThrowing(new ArgumentNullException("move"), () =>
            {
                target.MovePiece(null);
            });
        }

        [Test()]
        public void MovePiece_InvalidMove_False()
        {
            var target = new CheckersBoard(8);

            // Horizontal move.
            var move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(1, 0) }, new CheckersSquare(3, 0));
            Assert.IsFalse(target.MovePiece(move));

            // Vertical move.
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(1, 0) }, new CheckersSquare(1, 2));
            Assert.IsFalse(target.MovePiece(move));

            // Back move.
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(2, 3) }, new CheckersSquare(1, 2));
            Assert.IsFalse(target.MovePiece(move));

            // Move to occupied square to right side.
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(1, 2) }, new CheckersSquare(2, 3));
            Assert.IsTrue(target.MovePiece(move));
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(2, 3) }, new CheckersSquare(3, 4));
            Assert.IsTrue(target.MovePiece(move));
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(3, 4) }, new CheckersSquare(4, 5)); // Occupied.
            Assert.IsFalse(target.MovePiece(move));

            // Move to occupied square to left side.
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(7, 2) }, new CheckersSquare(6, 3));
            Assert.IsTrue(target.MovePiece(move));
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(6, 3) }, new CheckersSquare(5, 4));
            Assert.IsTrue(target.MovePiece(move));
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(5, 4) }, new CheckersSquare(6, 5)); // Occupied.
            Assert.IsFalse(target.MovePiece(move));

            // Move more than 1 square not capturing.
            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(1, 2) }, new CheckersSquare(3, 4));
            Assert.IsFalse(target.MovePiece(move));
        }

        [Test()]
        public void MovePiece_ValidMove_True()
        {
            var target = new CheckersBoard(8);

            // Move to occupied square to right side.
            var move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(3, 2) }, new CheckersSquare(4, 3));
            Assert.IsTrue(target.MovePiece(move));

            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerTwo) { CurrentSquare = new CheckersSquare(6, 5) }, new CheckersSquare(5, 4));
            Assert.IsTrue(target.MovePiece(move));

            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(4, 3) }, new CheckersSquare(6, 5));
            Assert.IsTrue(target.MovePiece(move));
        }

        [Test()]
        public void CountCatchableByPiece_Null_Exception()
        {
            var target = new CheckersBoard(8);

            ExceptionAssert.IsThrowing(new ArgumentNullException("piece"), () =>
            {
                target.CountCatchableByPiece(null);
            });
        }

        [Test()]
        public void CountCatchableByPiece_ThereIsNoEnemyPieceAround_Zero()
        {
            var target = new CheckersBoard(8);

            foreach (var p in target.PlayerOnePieces)
            {
                Assert.AreEqual(0, target.CountCatchableByPiece(p));
            }

            foreach (var p in target.PlayerTwoPieces)
            {
                Assert.AreEqual(0, target.CountCatchableByPiece(p));
            }
        }

        [Test()]
        public void CountCatchableByPiece_ThereIsEnemyPieceAroundButCannotBeCaptured_Zero()
        {
            var target = new CheckersBoard(8);
            var piece = target.GetSquare(1, 2).CurrentPiece;
            Assert.IsTrue(target.MovePiece(new CheckersMove(piece, target.GetSquare(2, 3))));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));

            Assert.IsTrue(target.MovePiece(new CheckersMove(piece, target.GetSquare(3, 4))));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));

            Assert.IsFalse(target.MovePiece(new CheckersMove(piece, target.GetSquare(4, 5))));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));
        }

        [Test()]
        public void CountCatchableByPiece_ThereIsEnemyPieceAround_CatchableCount()
        {
            var target = new CheckersBoard(8);
            var piece = target.GetSquare(1, 2).CurrentPiece;
            Assert.IsTrue(target.MovePiece(new CheckersMove(piece, target.GetSquare(2, 3))));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));

            Assert.IsTrue(target.MovePiece(new CheckersMove(piece, target.GetSquare(3, 4))));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));

            var enemyPiece = target.GetSquare(4, 5).CurrentPiece;
            Assert.AreEqual(1, target.CountCatchableByPiece(enemyPiece));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));

            Assert.IsTrue(target.MovePiece(new CheckersMove(enemyPiece, target.GetSquare(2, 3))));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece));

            var otherPiece = target.GetSquare(2, 1).CurrentPiece;
            Assert.IsTrue(target.MovePiece(new CheckersMove(otherPiece, target.GetSquare(1, 2))));
            Assert.AreEqual(1, target.CountCatchableByPiece(otherPiece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece));
        }

        [Test()]
        public void CountCatchableByPiece_ThereIsTwoEnemyPieceAround_CatchableCountTwo()
        {
            var target = new CheckersBoard(8);
            var piece = target.GetSquare(3, 2).CurrentPiece;
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));

            var enemyPiece1 = target.GetSquare(6, 5).CurrentPiece;
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece1));
            Assert.IsTrue(target.MovePiece(new CheckersMove(enemyPiece1, target.GetSquare(5, 4))));
            Assert.AreEqual(0, target.CountCatchableByPiece(piece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece1));
            Assert.IsTrue(target.MovePiece(new CheckersMove(enemyPiece1, target.GetSquare(4, 3))));
            Assert.AreEqual(1, target.CountCatchableByPiece(piece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece1));

            var enemyPiece2 = target.GetSquare(4, 5).CurrentPiece;
            Assert.AreEqual(1, target.CountCatchableByPiece(piece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece2));
            Assert.IsTrue(target.MovePiece(new CheckersMove(enemyPiece2, target.GetSquare(3, 4))));
            Assert.AreEqual(1, target.CountCatchableByPiece(piece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece2));
            Assert.IsTrue(target.MovePiece(new CheckersMove(enemyPiece2, target.GetSquare(2, 3))));
            Assert.AreEqual(2, target.CountCatchableByPiece(piece));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece2));
            Assert.AreEqual(0, target.CountCatchableByPiece(enemyPiece1));
        }

        [Test()]
        public void CountPieceChancesToBeCaptured_Null_Exception()
        {
            var target = new CheckersBoard(8);

            ExceptionAssert.IsThrowing(new ArgumentNullException("piece"), () =>
            {
                target.CountPieceChancesToBeCaptured(null);
            });
        }

        [Test()]
        public void CountPieceChancesToBeCaptured_ThereIsNoEnemyPieceAround_Zero()
        {
            var target = new CheckersBoard(8);

            foreach (var p in target.PlayerOnePieces)
            {
                Assert.AreEqual(0, target.CountPieceChancesToBeCaptured(p));
            }

            foreach (var p in target.PlayerTwoPieces)
            {
                Assert.AreEqual(0, target.CountPieceChancesToBeCaptured(p));
            }
        }

        [Test()]
        public void CountPieceChancesToBeCaptured_CanAndCannotCapture_CapturedCount()
        {
            var target = new CheckersBoard(8);
            var piece = target.GetSquare(1, 2).CurrentPiece;
            Assert.IsTrue(target.MovePiece(new CheckersMove(piece, target.GetSquare(2, 3))));
            Assert.AreEqual(0, target.CountPieceChancesToBeCaptured(piece));

            Assert.IsTrue(target.MovePiece(new CheckersMove(piece, target.GetSquare(3, 4))));
            Assert.AreEqual(2, target.CountPieceChancesToBeCaptured(piece));

            var enemyPiece = target.GetSquare(4, 5).CurrentPiece;
            Assert.AreEqual(0, target.CountPieceChancesToBeCaptured(enemyPiece));

            Assert.IsFalse(target.MovePiece(new CheckersMove(piece, target.GetSquare(4, 5))));
            Assert.AreEqual(2, target.CountPieceChancesToBeCaptured(piece));
        }
    }
}

