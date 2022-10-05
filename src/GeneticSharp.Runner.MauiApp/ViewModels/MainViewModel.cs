using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace GeneticSharp.Runner.MauiApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        readonly OperatorInfo _generationStrategy = new(nameof(IGenerationStrategy), nameof(PerformanceGenerationStrategy));
        readonly OperatorInfo _selection = new(nameof(ISelection), nameof(EliteSelection));
        readonly OperatorInfo _crossover = new(nameof(ICrossover), nameof(OrderedCrossover));
        readonly OperatorInfo _mutation = new(nameof(IMutation), nameof(ReverseSequenceMutation));
        readonly OperatorInfo _reinsertion = new(nameof(IReinsertion), nameof(ElitistReinsertion));
        readonly OperatorInfo _termination = new(nameof(ITermination), nameof(FitnessStagnationTermination));

        public MainViewModel()
        {
            Samples = new ObservableCollection<string>(TypeHelper.GetDisplayNamesByInterface<Samples.ISampleController>());
            SamplesSelectedIndex = 0;

            GenerationStrategies = new ObservableCollection<string>(TypeHelper.GetDisplayNamesByInterface<IGenerationStrategy>());
            generationStrategiesSelectedIndex = 0;
        }

        [ObservableProperty]
        ObservableCollection<string> samples;

        [ObservableProperty]
        int samplesSelectedIndex;

        [ObservableProperty]
        ObservableCollection<string> generationStrategies;

        [ObservableProperty]
        int generationStrategiesSelectedIndex;

        public IGenerationStrategy GenerationStrategy => (IGenerationStrategy)_generationStrategy.Instance;
        public IFitness Fitness { get; set; }
        public ISelection Selection => (ISelection)_generationStrategy.Instance;
        public ICrossover Crossover => (ICrossover)_generationStrategy.Instance;
        public IMutation Mutation => (IMutation)_generationStrategy.Instance;
        public IReinsertion Reinsertion => (IReinsertion)_generationStrategy.Instance;
        public ITermination Termination => (ITermination)_generationStrategy.Instance;

        [RelayCommand]
        async Task OpenGenerationStrategyEditor()
        {
            _generationStrategy.ImplementationName = generationStrategies[generationStrategiesSelectedIndex];
            await OpenEditor(_generationStrategy);
        }

        async Task<object> OpenEditor(OperatorInfo info)
        {                              
            await Shell.Current.GoToAsync(
                nameof(PropertyEditorPage),
                new Dictionary<string, object>
                {
                    { "OperatorInfo", info }
                }
            );

            return null;
        }
    }
}
