using System;
using System.Collections.Generic;
using System.Globalization;
using Gdk;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Tsp;
using Gtk;
using HelperSharp;
using GeneticSharp.Runner.GtkApp;
using GeneticSharp.Domain.Terminations;
using System.Threading;

/// <summary>
/// Main window.
/// </summary>
public partial class MainWindow: Gtk.Window
{	
	#region Fields
	private Population m_population;
	private TspFitness m_fitness;
	private Gdk.GC m_gc;
	private Pango.Layout m_layout;
	private Pixmap m_buffer;
    private TimeSpan? m_currentGenerationsTimeSpend;
	private ISelection m_selection;
	private ICrossover m_crossover;
	private IMutation m_mutation;
	private ITermination m_termination;
	#endregion

	#region Constructors
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	
		DeleteEvent+=delegate {Application.Quit(); };
		btnGenerateCities.Clicked += delegate { GenerateCities(); };
		btnRunGenerations.Clicked += delegate { Run(); };

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

		m_selection = SelectionService.CreateSelectionByName (cmbSelection.ActiveText);
		m_crossover = CrossoverService.CreateCrossoverByName (cmbCrossover.ActiveText);
		m_mutation = MutationService.CreateMutationByName(cmbMutation.ActiveText);
		m_termination = TerminationService.CreateTerminationByName (cmbTermination.ActiveText);

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

		hslCrossoverProbability.Value = Population.DefaultCrossoverProbability;
		hslMutationProbability.Value = Population.DefaultMutationProbability;

		cmbCrossover.Active = 1;

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
			if (m_population != null) {
				m_population.GenerationRan -= HandlePopulationUpdated;
				m_population.TerminationReached -= HandlePopulationUpdated;
				m_currentGenerationsTimeSpend = null;
			}        

			var chromosome = new TspChromosome(m_fitness.Cities.Count);

			m_population = new Population(
				Convert.ToInt32(sbtPopulationMinSize.Value),
				Convert.ToInt32(sbtPopulationMaxSize.Value),
				chromosome,
				m_fitness,
				m_selection, m_crossover, m_mutation);

			m_population.CrossoverProbability = Convert.ToSingle(hslCrossoverProbability.Value);
			m_population.MutationProbability = Convert.ToSingle(hslMutationProbability.Value);
			m_population.GenerationRan += HandlePopulationUpdated;
			m_population.TerminationReached -= HandlePopulationUpdated;
	    
			m_population.Termination = m_termination;
	        m_population.RunGenerations(); 
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
		m_fitness = new TspFitness (numberOfCities, 50, drawingArea.Allocation.Width -50, 50, drawingArea.Allocation.Height - 50);
	
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

	void HandlePopulationUpdated (object sender, EventArgs e)
	{
        m_currentGenerationsTimeSpend = DateTime.Now - m_population.Generations[0].CreationDate;
		UpdateMap ();
	}

	private void UpdateMap()
	{
		DrawCities ();

		if (m_population != null && m_population.CurrentGeneration != null) {
			var genes = m_population.BestChromosome.GetGenes ();

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
		
			WriteText(0, 0, "Generation: {0}", m_population.Generations.Count);
			WriteText(0, 20, "Distance: {0:n2}", ((TspChromosome) m_population.BestChromosome).Distance);
			WriteText(0, 40, "Time: {0}", m_currentGenerationsTimeSpend);
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
