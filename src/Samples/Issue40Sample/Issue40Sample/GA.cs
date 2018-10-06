using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace Issue40Sample
{
    public class GeneticOptimizerVariable
    {
        public string VariableName { get; set; }
        public int NumberDigitsPrecision { get; set; }
        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }

        public GeneticOptimizerVariable()
        { }

        public GeneticOptimizerVariable(string variableName, int numberDigitsPrecision, double minimumValue, double maximumValue)
        {
            VariableName = variableName;
            NumberDigitsPrecision = numberDigitsPrecision;
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }
    }

    public class GeneticOptimizerConfiguration
    {
        public int NumberThreadsToUse { get; set; }
        public string OptimizeVariableName { get; set; }
        public List<GeneticOptimizerVariable> Variables { get; set; }

        public GeneticOptimizerConfiguration()
        { }

        public GeneticOptimizerConfiguration(string optimizeVariableName, List<GeneticOptimizerVariable> variables, int numberThreadsToUse)
        {
            OptimizeVariableName = optimizeVariableName;
            Variables = variables;
            NumberThreadsToUse = numberThreadsToUse;
        }
    }

    public class GeneticOptimizerResult
    {
        public string OutputVariableName { get; set; }
        public List<string> InputVariableNames { get; set; }
        public List<double> BestFitInputs { get; set; }
        public double BestFitOutput { get; set; }
        public List<string> IterationArray { get; set; } //comma delimited -> values 1,...,n-1 = input values, n = output value

        public GeneticOptimizerResult()
        {
            InputVariableNames = new List<string>();
            BestFitInputs = new List<double>();
            IterationArray = new List<string>();
        }

        public GeneticOptimizerResult(string optimizationVariableName, List<string> variableNames)
        {
            OutputVariableName = optimizationVariableName;
            InputVariableNames = variableNames;
            BestFitInputs = new List<double>();
            IterationArray = new List<string>();
        }
    }

    public class GeneticOptimizer
    {
        private const int MinimumNumberPopulation = 50;
        private const int MaximumNumberPopulation = 100;
        private readonly GeneticOptimizerConfiguration _configuration;
        private readonly Action<string> _generationRanCallback;
        private readonly GeneticAlgorithm _algorithm;
        private GeneticOptimizerResult _result;

        public GeneticOptimizer(GeneticOptimizerConfiguration configuration, Func<double[], double> objectiveFunction, Action<string> generationRanCallback = null)
        {
            //store configuration
            _configuration = configuration;
            _generationRanCallback = generationRanCallback;

            //set min/max/precision of input variables
            var minValues = new double[_configuration.Variables.Count];
            var maxValues = new double[_configuration.Variables.Count];
            var fractionDigits = new int[_configuration.Variables.Count];

            for (int index = 0; index < _configuration.Variables.Count; index++)
            {
                minValues[index] = _configuration.Variables[index].MinimumValue;
                maxValues[index] = _configuration.Variables[index].MaximumValue;
                fractionDigits[index] = _configuration.Variables[index].NumberDigitsPrecision;
            }

            //total bits
            var totalBits = new int[] { 64 };

            //chromosome
            var chromosome = new FloatingPointChromosome(minValues, maxValues, totalBits, fractionDigits);

            //population
            var population = new Population(MinimumNumberPopulation, MaximumNumberPopulation, chromosome);

            //set fitness function
            var fitnessFunction = new FuncFitness(c =>
            {
                var fc = c as FloatingPointChromosome;
                var inputs = fc.ToFloatingPoints();
                var result = objectiveFunction(inputs);

                //add to results
                if (!Double.IsNaN(result))
                {
                    var list = inputs.ToList();
                    list.Add(result);

                    _result.IterationArray.Add(string.Join(",", list));
                }

                return result;
            });

            //other variables
            var selection = new EliteSelection();
            var crossover = new UniformCrossover(0.5f);
            var mutation = new FlipBitMutation();
            var termination = new FitnessThresholdTermination();

            _algorithm = new GeneticAlgorithm(population, fitnessFunction, selection, crossover, mutation)
            {
                Termination = termination,
            };

            //task parallelism
            var taskExecutor = new ParallelTaskExecutor();
            taskExecutor.MinThreads = 1;
            taskExecutor.MaxThreads = _configuration.NumberThreadsToUse;
            _algorithm.TaskExecutor = taskExecutor;

            //if (_configuration.NumberThreadsToUse > 1)
            //{
            //    var taskExecutor = new ParallelTaskExecutor();
            //    taskExecutor.MinThreads = 1;
            //    taskExecutor.MaxThreads = _configuration.NumberThreadsToUse;
            //    _algorithm.TaskExecutor = taskExecutor;
            //}

            //register generation ran callback
            _algorithm.GenerationRan += AlgorithmOnGenerationRan;

        }

        public void Start()
        {
            //define result
            _result = new GeneticOptimizerResult(_configuration.OptimizeVariableName, _configuration.Variables.Select(x => x.VariableName).ToList());

            //start optimizer
            _algorithm.Start();
        }

        public void Stop()
        {
            _algorithm.Stop();
        }

        public GeneticOptimizerResult GetResults()
        {
            return _result;
        }

        private void AlgorithmOnGenerationRan(object sender, EventArgs e)
        {
            var bestChromosome = _algorithm.BestChromosome as FloatingPointChromosome;
            if (bestChromosome == null || bestChromosome.Fitness == null)
                return;

            var phenotype = bestChromosome.ToFloatingPoints();

            //update results with best fit
            _result.BestFitInputs = phenotype.ToList();
            _result.BestFitOutput = bestChromosome.Fitness.Value;

            //invoke callback to update
            if (_generationRanCallback != null)
            {
                var variables = string.Join(" - ", _configuration.Variables.Select((item, index) => $"{item.VariableName} = {phenotype[index]}"));
                var updateString = $"Optimizer Generation: {_algorithm.GenerationsNumber} - Fitness: {bestChromosome.Fitness.Value} - Variables: {variables}";
                _generationRanCallback(updateString);
            }
        }
    }
}