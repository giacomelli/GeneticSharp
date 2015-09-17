using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using HelperSharp;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Tournament selection involves running several "tournaments" among a few individuals chosen at random from the population.
    /// The winner of each tournament (the one with the best fitness) is selected for crossover. 
    /// <remarks>    
    /// Selection pressure is easily adjusted by changing the tournament size. 
    /// If the tournament size is larger, weak individuals have a smaller chance to be selected.
    /// </remarks>
    /// </summary>
    [DisplayName("Tournament")]
    public class TournamentSelection : SelectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSelection"/> class.
        /// <remarks>
        /// The default Size is 2.
        /// The default AllowWinnerCompeteNextTournament is true.
        /// </remarks>
        /// </summary>
        public TournamentSelection() : this(2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSelection"/> class.
        /// <remarks>
        /// The default AllowWinnerCompeteNextTournament is true.
        /// </remarks>
        /// </summary>
        /// <param name="size">The size of the tournament, in other words, the number of chromosomes that will participate of each tournament until all need chromosomes be selected.</param>
        public TournamentSelection(int size) : this(size, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSelection"/> class.
        /// </summary>
        /// <param name="size">The size of the tournament, in other words, the number of chromosomes that will participate of each tournament until all need chromosomes be selected.</param>      
        /// <param name="allowWinnerCompeteNextTournament">If allow any winner in a tournament participate in the next tournament, in other words, if you want to allow a chromosome be selected more the one time.</param>
        public TournamentSelection(int size, bool allowWinnerCompeteNextTournament) : base(2)
        {
            Size = size;
            AllowWinnerCompeteNextTournament = allowWinnerCompeteNextTournament;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the tournament.
        /// <remarks>
        /// In other words, the number of chromosomes that will participate of each tournament until all need chromosomes be selected.
        /// </remarks>
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow any winner in a tournament to participate in the next tournament.
        /// <remarks>
        /// In other words, if you want to allow a chromosome be selected more the one time.
        /// </remarks>
        /// </summary>
        public bool AllowWinnerCompeteNextTournament { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>
        /// The selected chromosomes.
        /// </returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            if (Size > generation.Chromosomes.Count)
            {
                throw new SelectionException(
                    this,
                    "The tournament size is greater than available chromosomes. Tournament size is {0} and generation {1} available chromosomes are {2}.".With(Size, generation.Number, generation.Chromosomes.Count));
            }

            var candidates = generation.Chromosomes.ToList();
            var selected = new List<IChromosome>();

            while (selected.Count < number)
            {
                var randomIndexes = RandomizationProvider.Current.GetUniqueInts(Size, 0, candidates.Count);
                var tournamentWinner = candidates.Where((c, i) => randomIndexes.Contains(i)).OrderByDescending(c => c.Fitness).First();

                selected.Add(tournamentWinner);

                if (!AllowWinnerCompeteNextTournament)
                {
                    candidates.Remove(tournamentWinner);
                }
            }

            return selected;
        }
        #endregion
    }
}
