using GeneticSharp.Runner.MauiApp.Samples;
using GeneticSharp.Runner.MauiApp.ViewModels;

namespace GeneticSharp.Runner.MauiApp
{
    public partial class MainPage : ContentPage
    {
        private GeneticAlgorithm _ga;
        

        private ISampleController _sampleController;
        private SampleContext _sampleContext;
        private Thread _evolvingThread;

        MainViewModel _vm;

        public MainPage(MainViewModel vm)
        {            
            InitializeComponent();
            BindingContext = vm;
            _vm = vm;

            _sampleController = TypeHelper.CreateInstanceByName<ISampleController>("TSP");

            _sampleContext = new SampleContext();
            _sampleController.Context = _sampleContext;
            ResetSampleContext();
          

            sampleView.Add(_sampleController.CreateConfigView());

            GraphicsDrawable.Sample = _sampleController;

            _sampleController.Reconfigured += (e, args) =>
            {
                ResetSample();
                canvas.Invalidate();
            };

            ResetSample();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ResetSampleContext();
        }
      
        void ResetSample()
        {
            _sampleController.Reset();
            _vm.Fitness = _sampleController.CreateFitness();
        }

        void ResetSampleContext()
        {
            _sampleContext.DrawingArea = new System.Drawing.Rectangle((int)canvas.X, (int)canvas.Y, (int)canvas.Width, (int)canvas.Height);
            ResetSample();
        }

        public void StartGA()
        {
            RunGA(() =>
            {
                _sampleController.Reset();

                _sampleContext.Population = new Population(
                    Convert.ToInt32(populationMinSize.Value),
                    Convert.ToInt32(populationMaxSize.Value),
                    _sampleController.CreateChromosome())
                { 
                    GenerationStrategy =  _vm.GenerationStrategy 
                };


                _ga = new GeneticAlgorithm(
                    _sampleContext.Population,
                    _vm.Fitness,
                    _vm.Selection,
                    _vm.Crossover,
                    _vm.Mutation)
                {
                    //CrossoverProbability = Convert.ToSingle(hslCrossoverProbability.Value),
                    //MutationProbability = Convert.ToSingle(hslMutationProbability.Value),
                    Reinsertion = _vm.Reinsertion,
                    Termination = _vm.Termination
                };

                _sampleContext.GA = _ga;
                _ga.GenerationRan += delegate
                {
                    _sampleController.Update();
                };

                _sampleController.ConfigGA(_ga);
                _ga.Start();
            });
        }

        void RunGA(System.Action runAction)
        {
            try
            {
                runAction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Application.Invoke(delegate
                //{
                //    var msg = new MessageDialog(
                //        this,
                //        DialogFlags.Modal,
                //        MessageType.Error,
                //        ButtonsType.YesNo,
                //        "{0}\n\nDo you want to see more details about this error?",
                //        ex.Message);

                //    if (msg.Run() == (int)ResponseType.Yes)
                //    {
                //        var details = new MessageDialog(
                //            this,
                //            DialogFlags.Modal,
                //            MessageType.Info,
                //            ButtonsType.Ok,
                //            "StackTrace")
                //        { SecondaryText = ex.StackTrace };

                //        details.Run();
                //        details.Destroy();
                //    }

                //    msg.Destroy();
                //});
            }

            //btnNew.Visible = true;
            //btnResume.Visible = true;
            //btnStop.Visible = false;
            //vbxGA.Sensitive = true;
        }

        private void RunGAButton_Clicked(object sender, EventArgs e)
        {
            StartGA();
        }
    }
}