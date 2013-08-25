using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;
using Rhino.Mocks;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture]
    public class FitnessStagnationTerminationTest
    {
        [Test()]
        public void HasReached_NoStagnation_False()
        {
            var target = new FitnessStagnationTermination(3);
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.1 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.2 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.3 } );
			}

			repository.ReplayAll ();

			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga)); 
        }

        [Test()]
        public void HasReached_StagnantButNotReachTheGenerationsNumber_False()
        {

			var target = new FitnessStagnationTermination(4);
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.1 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.1 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.1 } );
			}
			repository.ReplayAll ();

			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
		}

        [Test()]
        public void HasReached_StagnantAndReachGenerationNumber_True()
        {
			var target = new FitnessStagnationTermination(3);
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.2} );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.2 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.3 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.3 } );
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub () { Fitness = 0.3 } );
			}
			repository.ReplayAll ();

			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsTrue(target.HasReached(ga));
		}
    }
}