using System;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

public class TspGA
{
    GeneticAlgorithm _ga;
    Timer _timer;
    public event Action GenerationRan;
    public TspFitness Fitness { get; private set; }
    public TspChromosome BestChromosome =>  _ga != null ? _ga.BestChromosome as TspChromosome : null;
    public int GenerationsNumber => _ga != null ? _ga.GenerationsNumber : 0;
    public bool IsRunning => _timer != null;

    public void Initialize(int numberOfCities, int areaWidth, int areaHeight)
    {
        Stop();
        Fitness = new TspFitness(numberOfCities, areaWidth, areaHeight);
        var chromosome = new TspChromosome(numberOfCities);

        // This operators are classic genetic algorithm operators that lead to a good solution on TSP,
        // but you can try others combinations and see what result you get.
        var crossover = new OrderedCrossover();
        var mutation = new ReverseSequenceMutation();
        var selection = new RouletteWheelSelection();
        var population = new Population(50, 100, chromosome);
       
        _ga = new GeneticAlgorithm(population, Fitness, selection, crossover, mutation);
    }

    public void Run()
    {
        if (!IsRunning)
        {
            // As there no way to use a new thread on WebAssembly right now, we wil use a timer
            // to start a new generation each 1 microsecond.
            _timer = new Timer(new TimerCallback(_ =>
            {
                _ga.Termination = new GenerationNumberTermination(_ga.GenerationsNumber + 1);

                if (_ga.GenerationsNumber > 0)
                    _ga.Resume();
                else
                    _ga.Start();

                GenerationRan?.Invoke();
            }), null, 0, 1);
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}