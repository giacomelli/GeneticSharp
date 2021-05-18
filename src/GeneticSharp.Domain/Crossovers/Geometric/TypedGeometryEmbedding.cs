using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Commons;

namespace GeneticSharp.Domain.Crossovers.Geometric
{
    /// <summary>
    /// This is a helper class to build an untyped geomtry embedding from a generic type definition one
    /// </summary>
    public class TypedGeometryEmbedding : IGeometryEmbedding
    {

        public object TypedEmbedding { get; set; }

        public void SetTypedEmbedding<TValue>(IGeometryEmbedding<TValue> embedding)
        {
            ExceptionHelper.ThrowIfNull(nameof(embedding), embedding);
            TypedEmbedding = embedding;
            MapFromGeometryFunction = (parents, offSpringValues) => embedding.MapFromGeometry(parents, offSpringValues.Cast<TValue>().ToArray());
            MapToGeometryFunction = parents => embedding.MapToGeometry(parents).Select(c=>(IList<Object>)c.Cast<object>().ToArray()).ToArray();

        }

        public Func<IList<IChromosome>, IList<object>, IChromosome> MapFromGeometryFunction { get; set; }

        public Func<IList<IChromosome>, IList<IList<object>>> MapToGeometryFunction { get; set; }

       

        public IChromosome MapFromGeometry(IList<IChromosome> parents, IList<object> offSpringValues)
        {
            return MapFromGeometryFunction(parents, offSpringValues);
        }

        public IList<IList<object>> MapToGeometry(IList<IChromosome> parents)
        {
            return MapToGeometryFunction(parents);
        }

      
    }
}