using System;
using GeneticSharp.Domain.Chromosomes;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class ChromosomeBaseTest
    {
        [Test]
        public void CompareTo_Others_DiffResults()
        {
            var target = MockRepository.GenerateMock<ChromosomeBase>();
            target.Fitness = 0.5;

            var other = MockRepository.GenerateMock<ChromosomeBase>();
            other.Fitness = 0.5;

            Assert.AreEqual(-1, target.CompareTo(null));
            Assert.AreEqual(0, target.CompareTo(other));            
            
            other.Fitness = 0.4;
            Assert.AreEqual(1, target.CompareTo(other));

            other.Fitness = 0.6;
            Assert.AreEqual(-1, target.CompareTo(other));
        }

		[Test]
		public void Fitness_AnyChange_Null()
		{
			var target = MockRepository.GenerateStub<ChromosomeBase>();
			Assert.IsFalse (target.Fitness.HasValue);
			target.Fitness = 0.5;
			Assert.IsTrue (target.Fitness.HasValue);

			target.Fitness = 0.5;
			target.AddGene (new Gene());
			Assert.IsFalse (target.Fitness.HasValue);

			target.Fitness = 0.5;
			target.ClearGenes ();
			Assert.IsFalse (target.Fitness.HasValue);

			target.Fitness = 0.5;
			target.AddGenes (new Gene[] { new Gene() });
			Assert.IsFalse (target.Fitness.HasValue);

			target.Fitness = 0.5;
			target.GenerateGene (0);
			Assert.IsTrue (target.Fitness.HasValue);

			target.Fitness = 0.5;
			target.ReplaceGene (0, new Gene ());
			Assert.IsFalse (target.Fitness.HasValue);
		}

		[Test]
		public void ReplaceGene_InvalidIndex_Exception()
		{
			var target = MockRepository.GenerateStub<ChromosomeBase>();

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException("index", "There is no Gene on index 0 to be replaced."), () => {
				target.ReplaceGene (0, new Gene ());
			});

			target.AddGene (new Gene());

			ExceptionAssert.IsThrowing (new ArgumentOutOfRangeException("index", "There is no Gene on index 1 to be replaced."), () => {
				target.ReplaceGene (1, new Gene ());
			});
		}

		[Test]
		public void ReplaceGene_ValidIndex_Replaced()
		{
			var target = MockRepository.GenerateStub<ChromosomeBase> ();
			target.AddGene (new Gene(1));
			target.AddGene (new Gene(3));

			target.ReplaceGene (0, new Gene(2));
			target.ReplaceGene (1, new Gene(6));

			var actual = target.GetGenes ();
			Assert.AreEqual (2, actual.Count);
			Assert.AreEqual (2, actual [0].Value);
			Assert.AreEqual (6, actual [1].Value);
		}
    }
}