using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Gdk;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Runner.GtkApp;
using Gtk;
using HelperSharp;

/// <summary>
/// Main window.
/// </summary>
public partial class MainWindow: Gtk.Window
{	
	#region Fields
	private GeneticAlgorithm m_ga;
	private Population m_population;
	private TspFitness m_fitness;
	private ISelection m_selection;
	private ICrossover m_crossover;
	private IMutation m_mutation;
	private ITermination m_termination;
    private IGenerationStrategy m_generationStrategy;

	private Gdk.GC m_gc;
	private Pango.Layout m_layout;
	private Pixmap m_buffer;
    #endregion

	#region Constructors
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		DeleteEvent+=delegate {Application.Quit(); };
		btnGenerateCities.Clicked += delegate { GenerateCities(); };
		btnEvolve.Clicked += delegate { Run(); };

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

		m_gc = new Gdk.GC(drawingArea.GdkWindow);
		m_gc.RgbFgColor = new Gdk.Color(255,50,50);
		m_gc.RgbBgColor = new Gdk.Color(255, 255, 255);
		m_gc.Background =  new Gdk.Color(255, 255, 255);
		m_gc.SetLineAttributes(1, LineStyle.OnOffDash, CapStyle.Projecting, JoinStyle.Round);
	
		m_layout = new Pango.Layout(this.PangoContext);
		m_layout.Alignment = Pango.Alignment.Center;
		m_layout.FontDescription = Pango.FontDescription.FromString("Arial 16");	

		spbCitiesNumber.ValueChanged += delegate {
			GenerateCities();
		};

		drawingArea.ConfigureEvent += delegate {
			ResetBuffer ();
			UpdateMap();
		};

		drawingArea.ExposeEvent += delegate {
			DrawBuffer();
		};

		LoadComboBox (cmbSelection, SelectionService.GetSelectionNames ());
		LoadComboBox (cmbCrossover, CrossoverService.GetCrossoverNames ());
		LoadComboBox (cmbMutation, MutationService.GetMutationNames ());
		LoadComboBox (cmbTermination, TerminationService.GetTerminationNames ());
		LoadComboBox (cmbGenerationStrategy, PopulationService.GetGenerationStrategyNames ());

		m_selection = SelectionService.CreateSelectionByName (cmbSelection.ActiveText);
		m_crossover = CrossoverService.CreateCrossoverByName (cmbCrossover.ActiveText);
		m_mutation = MutationService.CreateMutationByName(cmbMutation.ActiveText);
		m_termination = TerminationService.CreateTerminationByName (cmbTermination.ActiveText);
		m_generationStrategy = PopulationService.CreateGenerationStrategyByName (cmbGenerationStrategy.ActiveText);

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

		hslCrossoverProbability.Value = GeneticAlgorithm.DefaultCrossoverProbability;
		hslMutationProbability.Value = GeneticAlgorithm.DefaultMutationProbability;

		cmbCrossover.Active = 1;
		cmbTermination.Active = 1;

		ShowAll();
		ShowButtonByEditableProperties(btnEditSelection, m_selection);
		ShowButtonByEditableProperties(btnEditCrossover, m_crossover);
		ShowButtonByEditableProperties(btnEditMutation, m_mutation);
		ShowButtonByEditableProperties(btnEditTermination, m_termination);

		ResetBuffer ();
		UpdateMap ();	
	}
	#endregion

	#region Methods
    private void Run()
    {
		try {
			if (m_ga != null) {
				m_ga.GenerationRan -= HandleGAUpdated;
				m_ga.TerminationReached -= HandleGAUpdated;
			}        

			m_population = new Population(
				Convert.ToInt32(sbtPopulationMinSize.Value),
				Convert.ToInt32(sbtPopulationMaxSize.Value),
				new TspChromosome(m_fitness.Cities.Count));

			m_population.GenerationStrategy = m_generationStrategy;

			m_ga = new GeneticAlgorithm(
				m_population,
				m_fitness,
				m_selection, 
				m_crossover,
				m_mutation);

			m_ga.CrossoverProbability = Convert.ToSingle(hslCrossoverProbability.Value);
			m_ga.MutationProbability = Convert.ToSingle(hslMutationProbability.Value);
			m_ga.GenerationRan += HandleGAUpdated;
			m_ga.TerminationReached -= HandleGAUpdated;
	    
			m_ga.Termination = m_termination;
			m_ga.Evolve(); 
		}
		catch(Exception ex) {
			var msg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.YesNo, "{0}\n\nDo you want to see more details about this error?", ex.Message);
          
            if (msg.Run() == (int)ResponseType.Yes)
            {
                var details = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, ex.StackTrace);
                details.Run();
                details.Destroy();
            }

			msg.Destroy ();
		}
    }

	private void GenerateCities()
	{
		int numberOfCities = Convert.ToInt32(spbCitiesNumber.Value - (spbCitiesNumber.Value % 2));
		spbCitiesNumber.Value = numberOfCities;
		m_fitness = new TspFitness (numberOfCities, 100, drawingArea.Allocation.Width -100, 100, drawingArea.Allocation.Height - 100);

		DrawCities ();
		DrawBuffer ();
	}

	void LoadComboBox (ComboBox cmb, IList<string> names)
	{
		foreach (var c in names) {
			cmb.AppendText (c);
		}

		cmb.Active = 0;
	}

	void ShowButtonByEditableProperties(Button btn, object objectInstance)
	{
		if(PropertyEditor.HasEditableProperties(objectInstance.GetType()))
		{
			btn.Show(); 
		}
		else {
			btn.Hide();
		}
	}

	void HandleGAUpdated (object sender, EventArgs e)
	{
		UpdateMap ();
	}

	private void UpdateMap()
	{
		DrawCities ();

		if (m_population != null && m_population.CurrentGeneration != null) {
			var genes = m_ga.Population.BestChromosome.GetGenes ();

			for (int i = 0; i < genes.Length; i += 2) {
				var cityOneIndex = Convert.ToInt32 (genes [i].Value);
				var cityTwoIndex = Convert.ToInt32 (genes [i + 1].Value);
				var cityOne = m_fitness.Cities [cityOneIndex];
				var cityTwo = m_fitness.Cities [cityTwoIndex];

				if (i > 0) {
					var previousCity = m_fitness.Cities [Convert.ToInt32 (genes [i - 1].Value)];
					m_buffer.DrawLine (m_gc, previousCity.X, previousCity.Y, cityOne.X, cityOne.Y);
				}

				m_buffer.DrawLine (m_gc, cityOne.X, cityOne.Y, cityTwo.X, cityTwo.Y);


				m_layout.SetMarkup ("<span color='black'>{0}</span>".With (i));
				m_buffer.DrawLayout (m_gc, cityOne.X, cityOne.Y, m_layout);

				m_layout.SetMarkup ("<span color='black'>{0}</span>".With (i + 1));
				m_buffer.DrawLayout (m_gc, cityTwo.X, cityTwo.Y, m_layout);
			}

			var lastCity = m_fitness.Cities [Convert.ToInt32 (genes [genes.Length - 1].Value)];
			var firstCity = m_fitness.Cities [Convert.ToInt32 (genes [0].Value)];
			m_buffer.DrawLine (m_gc, lastCity.X, lastCity.Y, firstCity.X, firstCity.Y);
		
			var bestChromosome = (TspChromosome)m_ga.Population.BestChromosome;
			WriteText(0, 0, "Generation: {0}", m_population.GenerationsNumber);
			WriteText(0, 20, "Fitness: {0:n2}", bestChromosome.Fitness);
			WriteText(0, 40, "Distance: {0:n2}", bestChromosome.Distance);
			WriteText(0, 60, "Time: {0}", m_ga.TimeEvolving);
		}

		DrawBuffer ();
	}

    private void WriteText(int x, int y, string text, params object[] args)
    {
        m_layout.SetMarkup("<span color='gray'>{0}</span>".With(String.Format(CultureInfo.InvariantCulture, text, args)));
        m_buffer.DrawLayout(m_gc, x, y, m_layout);
    }

	private void ResetBuffer()
	{
		m_buffer = new Pixmap (drawingArea.GdkWindow, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
	}

	private void DrawCities()
	{
		if (m_fitness == null) {
			GenerateCities ();
		}

		m_buffer.DrawRectangle(drawingArea.Style.WhiteGC, true, 0, 0, drawingArea.Allocation.Width, drawingArea.Allocation.Height);

		foreach (var c in m_fitness.Cities)
		{
			m_buffer.DrawRectangle(m_gc, true, c.X - 2, c.Y - 2, 4, 4);
		}
	}

	private void DrawBuffer()
	{
		drawingArea.GdkWindow.DrawDrawable (m_gc, m_buffer, 0, 0, 0, 0, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
	}

	private TInterface ShowEditorProperty<TInterface>(Type objectType, object objectInstance)
	{
		var editor = new PropertyEditor(objectType, objectInstance);
		editor.Run();
		return (TInterface) editor.ObjectInstance;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}	
	#endregion
}
