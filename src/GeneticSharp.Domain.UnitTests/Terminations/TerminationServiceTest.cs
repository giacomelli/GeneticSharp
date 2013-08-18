using System;
using NUnit.Framework;
using GeneticSharp.Domain.Terminations;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class TerminationServiceTest
	{
		[Test()]
		public void GetTerminationTypes_NoArgs_AllAvailableTerminations ()
		{
			var actual = TerminationService.GetTerminationTypes ();

			Assert.AreEqual (2, actual.Count);
			Assert.AreEqual (typeof(GenerationNumberTermination), actual [0]);
			Assert.AreEqual (typeof(TimeTermination), actual [1]);
		}

		[Test()]
		public void GetTerminationNames_NoArgs_AllAvailableTerminationsNames ()
		{
			var actual = TerminationService.GetTerminationNames ();

			Assert.AreEqual (2, actual.Count);
			Assert.AreEqual ("Generation Number", actual [0]);
			Assert.AreEqual ("Time", actual [1]);
		}

		[Test()]
		public void CreateTerminationByName_InvalidName_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("There is no ITermination implementation with name 'Test'.", "name"), () => {
				TerminationService.CreateTerminationByName("Test");
			});
		}

		[Test()]
		public void CreateTerminationByName_ValidNameButInvalidConstructorArgs_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("A ITermination's implementation with name 'Generation Number' was found, but seems the constructor args were invalid.", "constructorArgs"), () => {
				TerminationService.CreateTerminationByName("Generation Number", 1f);
			});
		}

		[Test()]
		public void CreateTerminationByName_ValidName_TerminationCreated()
		{
			ITermination actual = TerminationService.CreateTerminationByName ("Generation Number") as GenerationNumberTermination;
			Assert.IsNotNull (actual);

			actual = TerminationService.CreateTerminationByName ("Time") as TimeTermination;
			Assert.IsNotNull (actual);
		}

		[Test()]
		public void GetTerminationTypeByName_InvalidName_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("There is no ITermination implementation with name 'Test'.", "name"), () => {
				TerminationService.GetTerminationTypeByName("Test");
			});
		}

		[Test()]
		public void GetTerminationTypeByName_ValidName_CrossoverTpe()
		{
			var actual = TerminationService.GetTerminationTypeByName ("Generation Number");
			Assert.AreEqual (typeof(GenerationNumberTermination), actual);

			actual = TerminationService.GetTerminationTypeByName ("Time");
			Assert.AreEqual (typeof(TimeTermination), actual);
		}
	}
}