using System;
using NUnit.Framework;
using GeneticSharp.Domain.Crossovers;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
	[TestFixture()]
	public class CrossoverServiceTest
	{
		[Test()]
		public void GetCrossoverTypes_NoArgs_AllAvailableCrossovers ()
		{
			var actual = CrossoverService.GetCrossoverTypes ();

			Assert.AreEqual (4, actual.Count);
			Assert.AreEqual (typeof(OnePointCrossover), actual [0]);
			Assert.AreEqual (typeof(OrderedCrossover), actual [1]);
			Assert.AreEqual (typeof(TwoPointCrossover), actual [2]);
			Assert.AreEqual (typeof(UniformCrossover), actual [3]);
		}

		[Test()]
		public void GetCrossoverNames_NoArgs_AllAvailableCrossoversNames ()
		{
			var actual = CrossoverService.GetCrossoverNames ();

			Assert.AreEqual (4, actual.Count);
			Assert.AreEqual ("One-Point", actual [0]);
			Assert.AreEqual ("Ordered (OX1)", actual [1]);
			Assert.AreEqual ("Two-Point", actual [2]);
			Assert.AreEqual ("Uniform", actual [3]);
		}

		[Test()]
		public void CreateCrossoverByName_InvalidName_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("There is no ICrossover implementation with name 'Test'.", "name"), () => {
				CrossoverService.CreateCrossoverByName("Test");
			});
		}

		[Test()]
		public void CreateCrossoverByName_ValidNameButInvalidConstructorArgs_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("A ICrossover's implementation with name 'One-Point' was found, but seems the constructor args was invalid.", "constructorArgs"), () => {
				CrossoverService.CreateCrossoverByName("One-Point");
			});
		}

		[Test()]
		public void CreateCrossoverByName_ValidName_CrossoverCreated()
		{
			ICrossover actual = CrossoverService.CreateCrossoverByName ("One-Point", 1) as OnePointCrossover;
			Assert.IsNotNull (actual);

			actual = CrossoverService.CreateCrossoverByName ("Ordered (OX1)") as OrderedCrossover;
			Assert.IsNotNull (actual);

			actual = CrossoverService.CreateCrossoverByName ("Two-Point", 1, 2) as TwoPointCrossover;
			Assert.IsNotNull (actual);

			actual = CrossoverService.CreateCrossoverByName ("Uniform", 1f) as UniformCrossover;
			Assert.IsNotNull (actual);
		}
	}
}