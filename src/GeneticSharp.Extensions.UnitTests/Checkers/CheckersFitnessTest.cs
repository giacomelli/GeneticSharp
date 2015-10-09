using GeneticSharp.Extensions.Checkers;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Checkers
{
    [TestFixture()]
    [Category("Extensions")]
    public class CheckersFitnessTest
    {
        [Test()]
        public void Evaluate_ChromosomeInvalidMove_Fitness0()
        {
            var target = new CheckersFitness(new CheckersBoard(8));
            var chromosome = new CheckersChromosome(2, 8);
            chromosome.Moves.Clear();
            chromosome.Moves.Add(new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(1, 2) }, new CheckersSquare(6, 7)));
            Assert.AreEqual(0, target.Evaluate(chromosome));
        }

        [Test()]
        public void Evaluate_ChromosomeForwardMove_Fitness05()
        {
            var target = new CheckersFitness(new CheckersBoard(8));
            var chromosome = new CheckersChromosome(2, 8);
            chromosome.Moves.Clear();
            chromosome.Moves.Add(new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(1, 2) }, new CheckersSquare(2, 3)));
            Assert.AreEqual(0.5, target.Evaluate(chromosome));
        }

        [Test()]
        public void Evaluate_ChromosomeForwardMoveAndCanCaptureAnotherOne_Fitness2()
        {
            var board = new CheckersBoard(8);
            var target = new CheckersFitness(board);
            var move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(3, 2) }, new CheckersSquare(4, 3));
            Assert.IsTrue(board.MovePiece(move));

            move = new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerTwo) { CurrentSquare = new CheckersSquare(6, 5) }, new CheckersSquare(5, 4));
            Assert.IsTrue(board.MovePiece(move));

            var chromosome = new CheckersChromosome(2, 8);
            chromosome.Moves.Clear();
            chromosome.Moves.Add(new CheckersMove(new CheckersPiece(CheckersPlayer.PlayerOne) { CurrentSquare = new CheckersSquare(4, 3) }, new CheckersSquare(6, 5)));
            target.Update(chromosome);

            Assert.AreEqual(2, target.Evaluate(chromosome));
        }
    }
}

