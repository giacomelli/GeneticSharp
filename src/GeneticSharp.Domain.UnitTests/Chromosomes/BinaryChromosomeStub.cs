using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    public class BinaryChromosomeStub : BinaryChromosomeBase
    {
        public BinaryChromosomeStub(int length)
            : base(length)
        {
        }

        #region implemented abstract members of ChromosomeBase

        public override IChromosome CreateNew ()
        {
            return new BinaryChromosomeStub (Length);
        }

        #endregion
    }
}

