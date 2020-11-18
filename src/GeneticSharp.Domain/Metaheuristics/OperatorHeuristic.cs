namespace GeneticSharp.Domain.Metaheuristics
{
    public abstract class OperatorHeuristic<TOperator> : ContainerMetaHeuristic
    {

        public OperatorHeuristic()
        {
            
        }

        public OperatorHeuristic(ParameterGenerator<TOperator> dynamicOperator)
        {
            DynamicOperator = dynamicOperator;
        }

        public OperatorHeuristic(TOperator staticOperator)
        {
            StaticOperator = staticOperator;
        }


        public ParameterGenerator<TOperator> DynamicOperator { get; set; }

        public TOperator StaticOperator { get; set; }

        protected TOperator GetOperator(IMetaHeuristicContext ctx)
        {
            if (StaticOperator != null)
            {
                return StaticOperator;
            }

            return DynamicOperator(this, ctx);
        }

    }
}