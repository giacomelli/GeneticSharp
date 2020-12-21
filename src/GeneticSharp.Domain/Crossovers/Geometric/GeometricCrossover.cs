using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Crossovers.Geometric
{

    //[DisplayName("Geometric")]
    //public class GeometricCrossover : GeometricConverter<object>
    //{

    //    public GeometricCrossover() : base() { }

    //    public GeometricCrossover(bool ordered) : base(ordered){}


    //    public GeometricCrossover(bool ordered, int parentNb, bool generateTwin) : base(ordered, parentNb, generateTwin)
    //    {
    //    }

    //    public GeometricCrossover(bool ordered, int parentNb, Func<int, IList<TValue>, TValue> linearGeometricOperator, bool generateTwin = false) : this(ordered, parentNb, generateTwin)
    //    {
    //        LinearGeometricOperator = linearGeometricOperator;
    //    }


    //}


    /// <summary>
    /// The Geometric crossover yields new genes by applying geometric operators on the parent genes values. Default operator convert genes to doubles computes the middle between gene values and converts back to the target type.
    /// Geometric operator can be gene based (Linear, same for all genes), or multidimensional (General)
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [DisplayName("Geometric")]
    public class GeometricCrossover<TValue> : CrossoverBase
    {

        private static readonly Func<int, IList<TValue>, TValue> _defaultGeometricOperator = (geneIndex, geneValues) => (geneValues.Sum(val => val.To<double>()) / geneValues.Count).To<TValue>();


        public GeometricCrossover(): this(false){}

        public GeometricCrossover(bool ordered) : this(ordered, 2, false) { }


        public GeometricCrossover(bool ordered, int parentNb, bool generateTwin) : base(parentNb, generateTwin ? 2 : 1)
        {
            IsOrdered = ordered;
            LinearGeometricOperator = _defaultGeometricOperator;
        }

        public GeometricCrossover(bool ordered, int parentNb, Func<int, IList<TValue>, TValue> linearGeometricOperator, bool generateTwin = false) : this(ordered, parentNb, generateTwin)
        {
            LinearGeometricOperator = linearGeometricOperator;
        }

        /// <summary>
        /// A function to compute child gene value from same index parent gene values
        /// </summary>
        public Func<int, IList<TValue>, TValue> LinearGeometricOperator { get; set; }

        /// <summary>
        /// A function to compute child gene values from all parent gene values
        /// </summary>
        public Func<IList<IList<TValue>>, IList<TValue>> GeneralGeometricOperator { get; set; }

        /// <summary>
        /// A geometry embedding can introduce transformation from chromosome values into and from metric space before and after geometric operator is computed
        /// </summary>
        public IGeometryEmbedding<TValue> GeometryEmbedding { get; set; } 



        /// <summary>
        /// The overriden PerformCross method applies the geometric operator to parents for a single offspring, and optionally generate the symmetrical twin if children nb is set to 2
        /// </summary>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
           var toReturn = new List<IChromosome>(ChildrenNumber);
           var firstChild = CreateOffspring(parents);
           toReturn.Add(firstChild);
           if (ChildrenNumber==2)
           {
               parents = parents.Reverse().ToList();
               var twinChild = CreateOffspring(parents);
               toReturn.Add(twinChild);
           }
           return toReturn;

        }

        /// <summary>
        /// The CreateOffspring method allows creating a single offspring by applying a geometric operator to parent individuals. It deals with optionally applying an embedding interface between metric space and genome, and applying either a general operator or a linear gene-wise operator.
        /// </summary>
        public IChromosome CreateOffspring(IList<IChromosome> parents)
        {
            if (GeometryEmbedding == null)
            {
                GeometryEmbedding = new OrderedEmbedding<TValue> { IsOrdered = IsOrdered };
            }
            var geometricParents = parents.Select(p => GeometryEmbedding.MapToGeometry(p)).ToList();
            IList<TValue> geometricChild;

            if (GeneralGeometricOperator != null)
            {
                geometricChild = GeneralGeometricOperator(geometricParents);
                return GeometryEmbedding.MapFromGeometry(parents, geometricChild);
            }

            if (LinearGeometricOperator==null)
            {
                throw new InvalidOperationException("GeometricCrossover has not geometric operator defined");
            }

            var nbGenes = parents[0].Length;
            geometricChild = new List<TValue>(nbGenes);
            for (int i = 0; i < nbGenes; i++)
            {
                var inputs = geometricParents.Select(p => p[i]).ToList();
                var newGeneValue = LinearGeometricOperator(i, inputs);
                geometricChild.Add(newGeneValue);
            }

            return GeometryEmbedding.MapFromGeometry(parents, geometricChild);
           
        }


      

    }
}