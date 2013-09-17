using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Extensions.Checkers;
using Checkers.Application;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using System.Linq;
using GeneticSharp.Infrastructure.Threading;

public class GAController : MonoBehaviour {
	
	#region Events
	public event System.EventHandler Ran;
	#endregion
	
	#region Fields
	private IList<CheckersMove> m_remainingMoves;
	private int m_currentGenerationNumber;
	#endregion
	
	#region Constructors
	public GAController ()
	{
		Instance = this;
		m_remainingMoves = new List<CheckersMove>();
	}
	#endregion
	
	#region Editor properties
	public int m_populationSize = 40;
	public int m_generationNumber = 100;
	public int m_movesAhead = 10;
	public int m_maxMoveAttempts = 3;
	public int m_boardSize = 10;
	#endregion
	
	#region Properties
	public static GAController Instance { get; private set; }
	public CheckersFitness Fitness { get; private set; }
	public GeneticAlgorithm GA { get; private set; }
	public int BoardSize { get { return m_boardSize; } }
	#endregion
	
	#region Methods
	private void Awake ()
	{
		var population = new Population (m_populationSize, m_populationSize, new CheckersChromosome (m_movesAhead, m_boardSize));
		Fitness = new CheckersFitness (new CheckersBoard (m_boardSize));
		GA = new GeneticAlgorithm (
			population, 
			Fitness, 
			new EliteSelection (),
			new UniformCrossover (),
			new UniformMutation ());
		
		GA.TaskExecutor = new SmartThreadPoolTaskExecutor();
		GA.TaskExecutor.Timeout = System.TimeSpan.FromSeconds (1);
	}
	
	public void MovePiece ()
	{
		StartCoroutine(MovePieceDelay());
	}
	
	private IEnumerator MovePieceDelay ()
	{
		HudController.IsThinking = true;
		yield return new WaitForSeconds(0.01f);
		
		
		CheckersMove move;
		
		do {	
			m_remainingMoves = GetNewMoves ();
			
			Debug.Log ("Using move...");
			move = m_remainingMoves.First ();
			
		} while(!Fitness.Board.MovePiece(move));
			
		Ran (this, System.EventArgs.Empty);
		HudController.IsThinking = false;
	}
	
	private IList<CheckersMove> GetNewMoves ()
	{
		int attempts = m_maxMoveAttempts;
		
		var termination = 
			new OrTermination (
				new FitnessStagnationTermination (10),
				new AndTermination (
					new FitnessThresholdTermination (0.1),
					new GenerationNumberTermination (GA.GenerationsNumber + 20)
				)
			);
			
		GA.Termination = termination;
		Debug.Log ("Running GA...");
		
		if (GA.State == GeneticAlgorithmState.NotStarted) {
			GA.Start ();
		} else {
			GA.Resume ();
		}
		
				
		Debug.Log ("Fitness: " + GA.BestChromosome.Fitness);
		Debug.Log ("Generations: " + GA.GenerationsNumber);
		
		if (GA.BestChromosome.Fitness <= 0) {
			Debug.LogError ("Lower than zero.");
		}
		
		attempts--;
		
		if (GA.BestChromosome.Fitness == 0) {
			HudController.IsGameOver = true;
		}
		
		return (GA.BestChromosome as CheckersChromosome).Moves;
	}
	#endregion
}

