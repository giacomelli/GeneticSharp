using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.Mutations
{
    [TestFixture()]
    [Category("Mutations")]
    public class MinChromosomeLengthTest
    {
        [Test()]
        public void MinChromosomeLength_SequenceMutations_Three()
        {
            Assert.AreEqual(3, new DisplacementMutation().MinChromosomeLength);
            Assert.AreEqual(3, new InsertionMutation().MinChromosomeLength);
            Assert.AreEqual(3, new PartialShuffleMutation().MinChromosomeLength);
            Assert.AreEqual(3, new ReverseSequenceMutation().MinChromosomeLength);
        }

        [Test()]
        public void MinChromosomeLength_NonSequenceMutations_Zero()
        {
            Assert.AreEqual(0, new TworsMutation().MinChromosomeLength);
            Assert.AreEqual(0, new UniformMutation().MinChromosomeLength);
        }
    }
}
