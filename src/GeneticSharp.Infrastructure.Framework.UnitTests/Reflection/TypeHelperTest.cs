using System;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Reflection;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Reflection
{
    [TestFixture()]
    [Category("Infrastructure")]
    public class TypeHelperTest
    {
        [Test()]
        public void GetDisplayNamesByInterface_ThereIsTypeWithoutDisplayNameAttribute_Exception()
        {
            ExceptionAssert.IsThrowing(new InvalidOperationException("The member 'BasicRandomization' has no DisplayNameAttribute."), () =>
            {
                TypeHelper.GetDisplayNamesByInterface<IRandomization>();
            });
        }
    }
}

