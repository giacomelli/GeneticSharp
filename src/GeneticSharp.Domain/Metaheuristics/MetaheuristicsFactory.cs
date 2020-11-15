using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Metaheuristics
{

    /// <summary>
    /// Provides a factory to generate common well known Metaheuristics
    /// </summary>
    public static class MetaheuristicsFactory
    {

        public static IMetaHeuristic WhaleOptimisationAlgorithm()
        {
            return new IndividualPhaseBasedMetaHeuristic((population, count, index) => RandomizationProvider.Current.GetInt(0,2))
                .WithPhaseHeuristic(1, new IndividualPhaseBasedMetaHeuristic((population, count, index) => RandomizationProvider.Current.GetInt(0, 2))
                    .WithPhaseHeuristic(1, null)
                    .WithPhaseHeuristic(1, null))
                .WithPhaseHeuristic(1, null);


        }

        //public static IMetaHeuristic FederalBureauInvestigation()
        //{



        //}

        //public static IMetaHeuristic EquilibriumOptimizer()
        //{



        //}

        //public static IMetaHeuristic SailfishOptimizer()
        //{



        //}

        //public static IMetaHeuristic SocialSkiDriverOptimization()
        //{



        //}

        //public static IMetaHeuristic SparrowSearchAlgorithm()
        //{



        //}

    }
}