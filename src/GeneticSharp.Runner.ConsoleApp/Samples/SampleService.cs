using System;
using System.Collections.Generic;
using GeneticSharp.Infrastructure.Framework.Reflection;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    /// <summary>
    /// Sample service.
    /// </summary>
    public static class SampleService
    {
        #region Methods
        /// <summary>
        /// Gets available sample types.
        /// </summary>
        /// <returns>All available sample types.</returns>
        public static IList<Type> GetSampleControllerTypes()
        {
            return TypeHelper.GetTypesByInterface<ISampleController>();
        }

        /// <summary>
        /// Gets the available sample names.
        /// </summary>
        /// <returns>The sample names.</returns>
        public static IList<string> GetSampleControllerNames()
        {
            return TypeHelper.GetDisplayNamesByInterface<ISampleController>();
        }

        /// <summary>
        /// Creates the ISample's implementation with the specified name.
        /// </summary>
        /// <returns>The sample implementation instance.</returns>
        /// <param name="name">The sample name.</param>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public static ISampleController CreateSampleControllerByName(string name, params object[] constructorArgs)
        {
            return TypeHelper.CreateInstanceByName<ISampleController>(name, constructorArgs);
        }

        /// <summary>
        /// Gets the sample type by the name.
        /// </summary>
        /// <returns>The sample type.</returns>
        /// <param name="name">The name of sample.</param>
        public static Type GetSampleControllerTypeByName(string name)
        {
            return TypeHelper.GetTypeByName<ISampleController>(name);
        }
        #endregion
    }
}