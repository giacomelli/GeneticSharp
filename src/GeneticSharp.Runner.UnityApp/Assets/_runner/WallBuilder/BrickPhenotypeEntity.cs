using UnityEngine;
using GeneticSharp.Runner.UnityApp.Commons;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class BrickPhenotypeEntity : PhenotypeEntityBase
    {
        public BrickPhenotypeEntity(Vector3 minPosition, Vector3 maxPosition)
        {
            Phenotypes = new IPhenotype[]
            {
                new Phenotype("x", 5) { MinValue = minPosition.x, MaxValue = maxPosition.x },
                new Phenotype("y", 5) { MinValue = minPosition.y, MaxValue = maxPosition.y },
                new Phenotype("z", 5) { MinValue = minPosition.z, MaxValue = maxPosition.z },
            };
        }

        public Vector3 Position
        {
            get
            {
                return new Vector3(
                    (float)Phenotypes[0].Value,
                    (float)Phenotypes[1].Value,
                    (float)Phenotypes[2].Value);
            }
        }
    }
}