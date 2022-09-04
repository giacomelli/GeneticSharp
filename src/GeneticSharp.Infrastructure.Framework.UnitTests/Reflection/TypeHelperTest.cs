using System;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Reflection
{
    [TestFixture()]
    [Category("Infrastructure")]
    public class TypeHelperTest
    {
        [Test()]
        public void GetDisplayNamesByInterface_ThereIsTypeWithoutDisplayNameAttribute_Exception()
        {
            Assert.Catch<InvalidOperationException>(() =>
            {
                TypeHelper.GetDisplayNamesByInterface<IRandomization>();
            }, "The member 'BasicRandomization' has no DisplayNameAttribute.");
        }
    }
}

