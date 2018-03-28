using System;
using GeneticSharp.Domain.Terminations;
using NSubstitute;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
    [TestFixture()]
    [Category("Terminations")]
    public class TerminationBaseTest
    {
        [Test()]
        public void HasReached_NullGeneration_Exception()
        {
            var target = Substitute.For<TerminationBase>();
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                target.HasReached(null);
            });

            Assert.AreEqual("geneticAlgorithm", actual.ParamName);
        }
    }
}

