using System;
using System.Collections.Generic;
using System.Threading;
using Gdk;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions;
using Gtk;
using GeneticSharp.Runner.GtkApp;
using GeneticSharp.Runner.GtkApp.Samples;

/// <summary>
/// Main window.
/// </summary>
public partial class MainWindow: Gtk.Window
{	
	#region Fields
	private GeneticAlgorithm m_ga;
	private IFitness m_fitness;
	private ISelection m_selection;
	private ICrossover m_crossover;
	private IMutation m_mutation;
	private ITermination m_termination;
    private IGenerationStrategy m_generationStrategy;
    
	private ISampleController m_sampleController;
    private SampleContext m_sampleContext;
	private Thread m_evolvingThread;
    #endregion

	#region Constructors
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		DeleteEvent+=delegate {Application.Quit(); };
	   
		btnEvolve.Clicked += delegate 
        {
			if(vbxButtonBar.Sensitive)
			{
				m_evolvingThread = new Thread(Run);                        
				m_evolvingThread.Start();
			}
			else {
				btnEvolve.Label = "Aborting...";
				btnEvolve.Sensitive = false;
				m_evolvingThread.Abort();
			}
        };

		btnEditSelection.Clicked += delegate {
			m_selection = ShowEditorProperty<ISelection>(SelectionService.GetSelectionTypeByName(cmbSelection.ActiveText), m_selection);
		};

		btnEditCrossover.Clicked += delegate {
			m_crossover = ShowEditorProperty<ICrossover>(CrossoverService.GetCrossoverTypeByName(cmbCrossover.ActiveText), m_crossover);
		};

		btnEditMutation.Clicked += delegate {
			m_mutation = ShowEditorProperty<IMutation>(MutationService.GetMutationTypeByName(cmbMutation.ActiveText), m_mutation);
		};

		btnEditTermination.Clicked += delegate {
			m_termination = ShowEditorProperty<ITermination>(TerminationService.GetTerminationTypeByName(cmbTermination.ActiveText), m_termination);
		};

		btnEditGenerationStrategy.Clicked += delegate {
			m_generationStrategy = ShowEditorProperty<IGenerationStrategy>(PopulationService.GetGenerationStrategyTypeByName(cmbGenerationStrategy.ActiveText), m_generationStrategy);
		};		
		
		drawingArea.ConfigureEvent += delegate {
			ResetBuffer ();
            UpdateSample();
		};

		drawingArea.ExposeEvent += delegate {
			DrawBuffer();
		};

		LoadComboBox (cmbSelection, SelectionService.GetSelectionNames ());
		LoadComboBox (cmbCrossover, CrossoverService.GetCrossoverNames ());
		LoadComboBox (cmbMutation, MutationService.GetMutationNames ());
		LoadComboBox (cmbTermination, TerminationService.GetTerminationNames ());
		LoadComboBox (cmbGenerationStrategy, PopulationService.GetGenerationStrategyNames ());
		LoadComboBox (cmbSample, SampleService.GetSampleControllerNames ());

		m_selection = SelectionService.CreateSelectionByName (cmbSelection.ActiveText);
		m_crossover = CrossoverService.CreateCrossoverByName (cmbCrossover.ActiveText);
		m_mutation = MutationService.CreateMutationByName(cmbMutation.ActiveText);
		m_termination = TerminationService.CreateTerminationByName (cmbTermination.ActiveText);
		m_generationStrategy = PopulationService.CreateGenerationStrategyByName (cmbGenerationStrategy.ActiveText);
		m_sampleController = SampleService.CreateSampleControllerByName (cmbSample.ActiveText);

        // Sample context.
        var layout = new Pango.Layout(this.PangoContext);
        layout.Alignment = Pango.Alignment.Center;
        layout.FontDescription = Pango.FontDescription.FromString("Arial 16");	

        m_sampleContext = new SampleContext(drawingArea.GdkWindow, this)
        {
            Layout = layout
        };

		m_sampleContext.GC = m_sampleContext.CreateGC (new Gdk.Color (255, 50, 50));

        m_sampleController.Context = m_sampleContext;
        m_sampleController.Reconfigured += delegate
        {
            ResetSample();
        };

		problemConfigWidgetContainer.Add(m_sampleController.CreateConfigWidget());

		cmbSelection.Changed += delegate {
			m_selection = SelectionService.CreateSelectionByName (cmbSelection.ActiveText);
			ShowButtonByEditableProperties(btnEditSelection, m_selection);
		};

		cmbCrossover.Changed += delegate {
			m_crossover = CrossoverService.CreateCrossoverByName (cmbCrossover.ActiveText);
			ShowButtonByEditableProperties(btnEditCrossover, m_crossover);
		};

		cmbMutation.Changed += delegate {
			m_mutation = MutationService.CreateMutationByName(cmbMutation.ActiveText);
			ShowButtonByEditableProperties(btnEditMutation, m_mutation);
		};

		cmbTermination.Changed += delegate {
			m_termination = TerminationService.CreateTerminationByName(cmbTermination.ActiveText);
			ShowButtonByEditableProperties(btnEditTermination, m_termination);
		};

		cmbGenerationStrategy.Changed += delegate {
			m_generationStrategy = PopulationService.CreateGenerationStrategyByName(cmbGenerationStrategy.ActiveText);
			ShowButtonByEditableProperties(btnEditGenerationStrategy, m_generationStrategy);
		};

		cmbSample.Changed += delegate {
			m_sampleController = SampleService.CreateSampleControllerByName(cmbSample.ActiveText);
			m_sampleController.Context = m_sampleContext;
			m_sampleController.Reconfigured += delegate
			{
				ResetSample();
			};
		
			if(problemConfigWidgetContainer.Children.Length > 0)
			{
				problemConfigWidgetContainer.Children[0].Destroy();
			}

			problemConfigWidgetContainer.Add(m_sampleController.CreateConfigWidget());
			problemConfigWidgetContainer.ShowAll();

			ResetBuffer ();
			ResetSample();
		};

		hslCrossoverProbability.Value = GeneticAlgorithm.DefaultCrossoverProbability;
		hslMutationProbability.Value = GeneticAlgorithm.DefaultMutationProbability;

		cmbCrossover.Active = 1;
		cmbTermination.Active = 2;

		ShowAll();
		ShowButtonByEditableProperties(btnEditSelection, m_selection);
		ShowButtonByEditableProperties(btnEditCrossover, m_crossover);
		ShowButtonByEditableProperties(btnEditMutation, m_mutation);
		ShowButtonByEditableProperties(btnEditTermination, m_termination);

		ResetBuffer ();
        ResetSample();
	}
	#endregion

	#region Methods
    private void Run()
    {
        try
        {
			Application.Invoke(delegate
			{
        		vbxButtonBar.Sensitive = false;
				btnEvolve.Label = "_Stop";
			});

            if (m_ga != null)
            {
                m_ga.GenerationRan -= HandleGAUpdated;
                m_ga.TerminationReached -= HandleGAUpdated;
            }

            m_sampleController.Reset();
            m_sampleContext.Population = new Population(
                Convert.ToInt32(sbtPopulationMinSize.Value),
                Convert.ToInt32(sbtPopulationMaxSize.Value),
                m_sampleController.CreateChromosome());

            m_sampleContext.Population.GenerationStrategy = m_generationStrategy;

            m_ga = new GeneticAlgorithm(
                m_sampleContext.Population,
                m_fitness,
                m_selection,
                m_crossover,
                m_mutation);

            m_ga.CrossoverProbability = Convert.ToSingle(hslCrossoverProbability.Value);
            m_ga.MutationProbability = Convert.ToSingle(hslMutationProbability.Value);
            m_ga.GenerationRan += HandleGAUpdated;
            m_ga.TerminationReached -= HandleGAUpdated;

            m_ga.Termination = m_termination;

            m_ga.Start();
	    }
		catch(ThreadAbortException) {
			Console.WriteLine ("Thread aborted.");
		}
        catch (Exception ex)
        {
			Application.Invoke(delegate
			{
				var msg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.YesNo, "{0}\n\nDo you want to see more details about this error?", ex.Message);

	            if (msg.Run() == (int)ResponseType.Yes)
	            {
	                var details = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, ex.StackTrace);
	                details.Run();
	                details.Destroy();
	            }

	            msg.Destroy();
			});
        }
        finally
        {           
			Application.Invoke(delegate
			{
				vbxButtonBar.Sensitive = true;
				btnEvolve.Label = "_Evolve";
				btnEvolve.Sensitive = true;
			});
        }
    }

    private void LoadComboBox(ComboBox cmb, IList<string> names)
	{
		foreach (var c in names) {
			cmb.AppendText (c);
		}

		cmb.Active = 0;
	}

    private void ShowButtonByEditableProperties(Button btn, object objectInstance)
	{
		if(PropertyEditor.HasEditableProperties(objectInstance.GetType()))
		{
			btn.Show(); 
		}
		else {
			btn.Hide();
		}
	}

    private TInterface ShowEditorProperty<TInterface>(Type objectType, object objectInstance)
    {
        var editor = new PropertyEditor(objectType, objectInstance);
        editor.Run();
        return (TInterface)editor.ObjectInstance;
    }

    private void HandleGAUpdated(object sender, EventArgs e)
	{
        UpdateSample();
	}

    private void UpdateSample()
    {
        if (m_sampleContext.Population == null)
        {
            Application.Invoke(delegate
            {
                DrawSample();
            });
        }
        else {
            // Avoid to update the map so quickly that makes the UI freeze.
            if (m_sampleContext.Population.GenerationsNumber % 10 == 0)
            {                
                Application.Invoke(delegate
                {
                    m_sampleController.Update();
                    DrawSample();
                });
            }
        }
    }

    private void ResetSample()
    {
        m_sampleContext.Population = null;
        var r = drawingArea.Allocation;
        m_sampleContext.DrawingArea = new Rectangle(0, 100, r.Width, r.Height - 100);
        m_sampleController.Reset();
        m_fitness = m_sampleController.CreateFitness();
        UpdateSample();
    }

    private void DrawSample()
    {
        m_sampleContext.Reset();
        m_sampleContext.Buffer.DrawRectangle(drawingArea.Style.WhiteGC, true, 0, 0, drawingArea.Allocation.Width, drawingArea.Allocation.Height);

        m_sampleController.Draw();

        if (m_sampleContext.Population != null)
        {
            m_sampleContext.WriteText("Generation: {0}", m_sampleContext.Population.GenerationsNumber);
            m_sampleContext.WriteText("Fitness: {0:n2}", m_sampleContext.Population.BestChromosome.Fitness);
            m_sampleContext.WriteText("Time: {0}", m_ga.TimeEvolving);
        }

        DrawBuffer();
    }	
   
	private void ResetBuffer()
	{
        m_sampleContext.Buffer = new Pixmap(drawingArea.GdkWindow, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
	}
	
	private void DrawBuffer()
	{
        drawingArea.GdkWindow.DrawDrawable(m_sampleContext.GC, m_sampleContext.Buffer, 0, 0, 0, 0, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}	
	#endregion
}
