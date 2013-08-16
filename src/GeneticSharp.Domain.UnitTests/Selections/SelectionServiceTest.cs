using System;
using NUnit.Framework;
using GeneticSharp.Domain.Selections;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Selections
{
	[TestFixture()]
	public class SelectionServiceTest
	{
		[Test()]
		public void GetSelectionTypes_NoArgs_AllAvailableSelections ()
		{
			var actual = SelectionService.GetSelectionTypes ();

			Assert.AreEqual (2, actual.Count);
			Assert.AreEqual (typeof(EliteSelection), actual [0]);
			Assert.AreEqual (typeof(RouletteWheelSelection), actual [1]);
		}

		[Test()]
		public void GetSelectionNames_NoArgs_AllAvailableSelectionsNames ()
		{
			var actual = SelectionService.GetSelectionNames ();

			Assert.AreEqual (2, actual.Count);
			Assert.AreEqual ("Elite", actual [0]);
			Assert.AreEqual ("Roulette Wheel", actual [1]);
		}

		[Test()]
		public void CreateSelectionByName_InvalidName_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("There is no ISelection implementation with name 'Test'.", "name"), () => {
				SelectionService.CreateSelectionByName("Test");
			});
		}

		[Test()]
		public void CreateSelectionByName_ValidNameButInvalidConstructorArgs_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("A ISelection's implementation with name 'Elite' was found, but seems the constructor args were invalid.", "constructorArgs"), () => {
				SelectionService.CreateSelectionByName("Elite", 1);
			});
		}

		[Test()]
		public void CreateSelectionByName_ValidName_SelectionCreated()
		{
			ISelection actual = SelectionService.CreateSelectionByName ("Elite") as EliteSelection;
			Assert.IsNotNull (actual);

			actual = SelectionService.CreateSelectionByName ("Roulette Wheel") as RouletteWheelSelection;
			Assert.IsNotNull (actual);
		}

		[Test()]
		public void GetSelectionTypeByName_InvalidName_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentException ("There is no ISelection implementation with name 'Test'.", "name"), () => {
				SelectionService.GetSelectionTypeByName("Test");
			});
		}

		[Test()]
		public void GetSelectionTypeByName_ValidName_SelectionTpe()
		{
			var actual = SelectionService.GetSelectionTypeByName ("Elite");
			Assert.AreEqual (typeof(EliteSelection), actual);

			actual = SelectionService.GetSelectionTypeByName ("Roulette Wheel");
			Assert.AreEqual (typeof(RouletteWheelSelection), actual);
		}
	}
}