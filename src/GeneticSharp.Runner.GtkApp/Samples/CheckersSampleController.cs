using System;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Extensions.Checkers;
using Gtk;

namespace GeneticSharp.Runner.GtkApp.Samples
{
	/// <summary>
	/// Checkers sample controller.
	/// </summary>
	[DisplayName("Checkers")]
	public class CheckersSampleController : SampleControllerBase
	{
		#region Fields
		private int m_boardSize = 10;
		private int m_movesAhead = 10;
		private CheckersFitness m_fitness;
		private Gdk.GC m_playerOneGC;
		private Gdk.GC m_playerTwoGC;
		#endregion

		#region implemented abstract members of SampleControllerBase
		/// <summary>
		/// Creates the config widget.
		/// </summary>
		/// <returns>The config widget.</returns>
		public override Gtk.Widget CreateConfigWidget ()
		{
			m_playerOneGC = Context.CreateGC (new Gdk.Color (0, 0, 0));
			m_playerTwoGC = Context.CreateGC (new Gdk.Color (100, 100, 100));

			var container = new VBox();
			var boardSize = new SpinButton(2, 10000, 2);
			boardSize.Text = "Board size";
			boardSize.Value = m_boardSize;
			boardSize.ValueChanged += delegate
			{
				m_boardSize = boardSize.ValueAsInt - (boardSize.ValueAsInt  % 2);
				boardSize.Value = m_boardSize;
				OnReconfigured();
			};
			container.Add(boardSize);

			var movesAhead = new SpinButton(1, 1000, 1);
			movesAhead.Text = "Moves ahead";
			movesAhead.Value = m_movesAhead;
			movesAhead.ValueChanged += delegate
			{
				m_boardSize = movesAhead.ValueAsInt - (movesAhead.ValueAsInt  % 2);
				movesAhead.Value = m_movesAhead;
				OnReconfigured();
			};
			container.Add(movesAhead);

			var generateButton = new Button();
			generateButton.Label = "Generate board";
			generateButton.Clicked += delegate
			{
				m_boardSize = boardSize.ValueAsInt;
				OnReconfigured();
			};

			container.Add(generateButton);

			return container;
		}

		/// <summary>
		/// Creates the fitness.
		/// </summary>
		/// <returns>The fitness.</returns>
		public override IFitness CreateFitness ()
		{
			m_fitness = new CheckersFitness (m_boardSize);

			return m_fitness;
		}

		/// <summary>
		/// Creates the chromosome.
		/// </summary>
		/// <returns>The chromosome.</returns>
		public override IChromosome CreateChromosome ()
		{
			return new CheckersChromosome (m_movesAhead, m_boardSize);
		}

		/// <summary>
		/// Draws the sample.
		/// </summary>
		public override void Draw ()
		{
			var buffer = Context.Buffer;
			var gc = Context.GC;
			var layout = Context.Layout;
			var population = Context.Population;
			var squareSize = Math.Min (Context.DrawingArea.Width, Context.DrawingArea.Height) / m_boardSize;
			var halfSquareSize = squareSize / 2;
			var quarterSquareSize = halfSquareSize / 2;
		
			// Draw board.
			for (int c = 0; c < m_boardSize; c++) {
				for (int r = 0; r < m_boardSize; r++) {
					var s = m_fitness.Board [c, r];
					var x = c * squareSize;
					var y = r * squareSize;
				
					// Draws the square.
					buffer.DrawRectangle(gc, false, x, y, squareSize, squareSize);
				
					if (s.State != CheckersSquareState.NotPlayable) {
						buffer.DrawRectangle(gc, true, x, y, squareSize, squareSize);

						if (s.State == CheckersSquareState.OccupiedByPlayerOne) {
							buffer.DrawRectangle (m_playerOneGC, true, x + quarterSquareSize, y + quarterSquareSize, halfSquareSize, halfSquareSize);
						} else if (s.State == CheckersSquareState.OccupiedByPlayerTwo) {
							buffer.DrawRectangle(m_playerTwoGC, true, x + quarterSquareSize, y + quarterSquareSize, halfSquareSize, halfSquareSize);
						}
					}
				}
			}

		}
		#endregion
	}
}

