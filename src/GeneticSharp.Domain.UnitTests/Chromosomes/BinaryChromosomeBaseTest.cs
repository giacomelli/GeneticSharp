using System;
using NUnit.Framework;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
	[TestFixture]
	public class BinaryChromosomeBaseTest
	{
		[Test]
		public void FlipBit_Index_ValueFlip()
		{
			var target = new BinaryChromosomeStub (2);
			target.ReplaceGenes (0, new Gene[] {
				new Gene(0), new Gene(1)
			});

			Assert.AreEqual ("01", target.ToString ());

			target.FlipGene (0);
			Assert.AreEqual ("11", target.ToString ());

			target.FlipGene (1);
			Assert.AreEqual ("10", target.ToString ());
		}

		[Test]
		public void GenerateGene_Index_ZeroOrOne()
		{
			var chromosomes = new List<BinaryChromosomeBase> ();

			for (int i = 0; i < 100; i++) {
				var target = new BinaryChromosomeStub (2);
				var gene0 = target.GenerateGene (0);
				Assert.IsInstanceOf<int> (gene0.Value);

				var gene1 = target.GenerateGene (1);
				Assert.IsInstanceOf<int> (gene1.Value);

				target.ReplaceGenes(0, new Gene[] { gene0, gene1 });

				chromosomes.Add (target);
			}

			Assert.IsTrue (chromosomes.Any (c => c.GetGenes ().Any (g => ((int)g.Value) == 0)));
		}
	}
}

