using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Runner.UnityApp.Commons;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarChromosome : BitStringChromosome<CarVectorPhenotypeEntity>
    {
        private CarSampleConfig m_config;
        public CarChromosome(CarSampleConfig config)
        {
            m_config = config;

            var phenotypeEntities = new CarVectorPhenotypeEntity[config.VectorsCount];

            for (int i = 0; i < phenotypeEntities.Length; i ++)
            {
                phenotypeEntities[i] = new CarVectorPhenotypeEntity(config, i);
            }

            SetPhenotypes(phenotypeEntities);
            CreateGenes();
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public float MaxDistance { get; set; }
        public float MaxDistanceTime { get; set; }
        public float MaxVelocity 
        { 
            get 
            {
                return MaxDistanceTime > 0 ? MaxDistance / MaxDistanceTime : 0; 
                            
            } 
        }
      
        public override IChromosome CreateNew()
        {
            return new CarChromosome(m_config);
        }
    }
}