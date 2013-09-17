GeneticSharp
===========

GeneticSharp is a fast, extensible, multi-platform and multithreading C# Genetic Algorithm library that simplifies the development of applications using Genetic Algorithms (GAs).

Can be used in ASP .NET MVC, Web Forms, Windows Forms, GTK# and Unity3D applications.

--------

Features
===
 - Chromosomes
   - Add your own chromosome representation implementing IChromosome interface or extending ChromosomeBase.   
 - Fitness
   - Add your own fitness evaluation, implementing IFitness interface.
 - Populations
   - Generations
   - Generation strategy
     - Performance strategy
     - Tracking strategy  
 - Selections
   - Elite (also know as Truncate or Truncation)
   - Roulette Wheel
   - Stochastic Universal Sampling 
   - Tournament  
   - Others selections can be added implementing ISelection interface or extending SelectionBase. 
 - Crossovers
   - Cut and Splice 
   - Cycle (CX)   
   - One-Point
   - Ordered OX1
   - Partially Mapped (PMX)
   - Three parent
   - Two-Point
   - Uniform
   - Others crossovers can be added implementing ICrossover interface or extending CrossoverBase.   
 - Mutations
   - Reverse Sequence (RSM)
   - Twors
   - Uniform	
   - Others mutations can be added implementing IMutation interface or extending MutationBase.
 - Reinsertions
   - Elitist
   - Fitness Based
   - Pure
   - Uniform
 - Terminations
   - Generation number
   - Time evolving
   - Fitness stagnation
   - Fitness threshold  
   - And e Or (allows combine others terminations)
 - Randomizations
   - Basic randomization (using System.Random)
   - Fast random   
   - If you need a special kind of randomization for your GA, just implement the IRandomization interface.
 - Runner app (GTK#) showing the library solving TSP (Travelling Salesman Problem). 
 - Mono support
 - Fully tested on Windows and MacOSX
 - 100% Unit Tests coveraged 
 - 100% code documentation
 - FxCop validated
 - Good (and good used) design patterns  

--------


Usage
===

Creating your own fitness evaluation 
---
```csharp

public class YourIFitnessImplementation : IFitness
{  
	public double Evaluate (IChromosome chromosome)
	{
		// Avaliate the fitness of chromosome.
	}
}

```

Creating your own chromosome 
---
```csharp

public class YourIChrosomeImplementation : ChromosomeBase
{
	public override Gene GenerateGene (int geneIndex)
	{
		// Generate a gene base on your chromosome representation.
	}

	public override IChromosome CreateNew ()
	{
		return new YourIChrosomeImplementation();
	}
}

```

Running your GA 
---
```csharp

var selection = new EliteSelection();
var crossover = new OrderedCrossover();
var mutation = new ReverseSequenceMutation();
var fitness = new YourIFitnessImplementation();
var chromosome = new YourIChrosomeImplementation(); // please, don't use names like that ;)
var population = new Population (50, 70, chromosome);

var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);

ga.Start();

```

--------

Roadmap
--------
 - Improve Runner.GtkApp
   - Add new problems/classic samples
      - Checkers 
	  - Time series   
 - Create and publish NuGet package
 - Create the wiki
 - Add new selections   
   - Reward-based
 - Add new crossovers   
   - Order-based (OX2)
   - Position-based (POS)
   - Voting recombination
   - Alternating-position (AP)
   - Sequential Constructive (SCX)    
   - Shuffle crossover
   - Precedence Preservative Crossover (PPX)
 - Add new mutations
   - Non-Uniform
   - Flip Bit
   - Boundary
   - Gaussian 
 - Add new terminations
   - Fitness convergence 
   - Population convergence
   - Chromosome convergence   
 - Unity3d game sample
 - MonoTouch Runner app (sample)
 - Parallel populations (islands)
 
--------

How to improve it?
======

Create a fork of [GeneticSharp](https://github.com/giacomelli/GeneticSharp/fork). 

Did you change it? [Submit a pull request](https://github.com/giacomelli/GeneticSharp/pull/new/master).


License
======

Licensed under the The MIT License (MIT).
In others words, you can use this library for developement any kind of software: open source, commercial, proprietary and alien.


Change Log
======
0.5.0 First version.
