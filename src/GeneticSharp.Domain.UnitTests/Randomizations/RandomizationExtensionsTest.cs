using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Randomizations
{
    [TestFixture]
    public class RandomizationExtensionsTest
    {
        [Test]
        [Category("Randomizations")]
        public void GetEvenInt_MinMax_OnlyEven()
        {
            var rnd = new BasicRandomization();

            for (int i = 0; i < 10; i++)
            {
                var actual = rnd.GetEvenInt(0, 10);
                Assert.IsTrue(actual % 2 == 0);
            }
        }

        [Test]
        [Category("Randomizations")]
        public void GetOddInt_MinMax_OnlyOdd()
        {
            var rnd = new BasicRandomization();

            for (int i = 0; i < 10; i++)
            {
                var actual = rnd.GetOddInt(0, 10);
                Assert.IsTrue(actual % 2 != 0);
            }
        }
    }
}
