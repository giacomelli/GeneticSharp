GeneticSharp
===========

GeneticSharp is a fast, extensible, multi-platform and multithreading C# Genetic Algorithm library that simplifies the development of applications using Genetic Algorithms (GAs).

Can be used in ASP .NET MVC, Web Forms, Windows Forms, GTK# and Unity3D applications.

--------

Features
===
 - Chromosomes
   - Add your own chromosome representation implementing IChromosome interface or extending ChromosomeBase.  
 - Crossovers
   - One-Point
   - Ordered OX1
   - Partially Mapped (PMX)
   - Two-Point
   - Uniform
   - Others crossovers can be added implementing ICrossover interface or extending CrossoverBase.    
 - Fitness
   - Add your own fitness evaluation, implementing IFitness interface.
 - Mutations
   - Reverse Sequence (RSM)
   - Twors
   - Uniform	
   - Others mutations can be added implementing IMutation interface or extending MutationBase.
 - Populations
   - Generations
   - Generation strategy
     - Performance strategy
     - Tracking strategy  
 - Randomizations
   - Basic randomization (using System.Random)
   - Fast random
   - Troschuetz
   - If you need a special kind of randomization for your GA, just implement the IRandomization interface.
 - Selections
   - Elite (also know as Truncate or Truncation)
   - Roulette Wheel
   - Tournament  
   - Others selections can be added implementing ISelection interface or extending SelectionBase. 
 - Terminations
   - Generation number
   - Time evolving
   - Fitness threshold  
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

ga.Evolve();

```

--------

Roadmap
--------
 - Improve Runner.GtkApp
 - Create and publish NuGet package
 - Add new crossovers
 - Add new mutations
 - Add new selections
 - Add new terminations
   - Population convergence
   - Chromosome convergence   
 - Unity3d game sample
 - MonoTouch Runner app (sample)
 - New classic samples
   - Time series   
 - Parallel populations (islands)
 
--------

How to improve it?
======

Create a fork of [GeneticSharp](https://github.com/giacomelli/GeneticSharp/fork). 

Did you change it? [Submit a pull request](https://github.com/giacomelli/GeneticSharp/pull/new/master).


License
======

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at [apache.org/licenses/LICENSE-2.0](http://apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.


Change Log
======
1.0.0 First version.
