using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Randomizations
{
    [TestFixture()]
    [Category("Randomizations")]
    public class RandomizationProviderTest
    {
        [Test()]
        public void Current_Default_IsNotNull()
        {
            Assert.IsNotNull(RandomizationProvider.Current);
        }
    }
}

