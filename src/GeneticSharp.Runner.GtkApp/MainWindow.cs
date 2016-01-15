using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gdk;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Reflection;
using GeneticSharp.Runner.GtkApp;
using GeneticSharp.Runner.GtkApp.Samples;
using Gtk;

/// <summary>
/// Main window.
/// </summary>
public partial class MainWindow : Gtk.Window
{
    #region Fields
    private GeneticAlgorithm m_ga;
    private IFitness m_fitness;
    private ISelection m_selection;
    private ICrossover m_crossover;
    private IMutation m_mutation;
    private IReinsertion m_reinsertion;
    private ITermination m_termination;
    private IGenerationStrategy m_generationStrategy;

    private ISampleController m_sampleController;
    private SampleContext m_sampleContext;
    private Thread m_evolvingThread;
    #endregion

    #region Constructors
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        HeightRequest = 200;

        DeleteEvent += delegate { Application.Quit(); };
        drawingArea.ConfigureEvent += delegate
        {
            ResetBuffer();
            UpdateSample();
        };

        sbtPopulationMinSize.SetRange(2, int.MaxValue);
        sbtPopulationMaxSize.SetRange(2, int.MaxValue);

        drawingArea.ExposeEvent += delegate
        {
            DrawBuffer();
        };

        ShowAll();

        btnResume.Visible = false;
        btnStop.Visible = false;
        btnNew.Visible = false;
        PrepareButtons();
        PrepareComboBoxes();
        PrepareSamples();

        cmbSample.Active = 0;
        hslCrossoverProbability.Value = GeneticAlgorithm.DefaultCrossoverProbability;
        hslMutationProbability.Value = GeneticAlgorithm.DefaultMutationProbability;

        ResetBuffer();
        ResetSample();

        GLib.Timeout.Add(
            100,
            new GLib.TimeoutHandler(delegate
        {
            UpdateSample();
            return true;
        }));
    }
    #endregion

    #region Protected methods
    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
    #endregion

    #region GA methods
    private void RunGAThread(bool isResuming)
    {
        vbxSample.Sensitive = false;
        vbxGA.Sensitive = false;
        m_evolvingThread = isResuming ? new Thread(ResumeGA) : new Thread(StartGA);
        m_evolvingThread.Start();
    }

    private void StartGA()
    {
        RunGA(() =>
        {
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
            m_ga.Reinsertion = m_reinsertion;
            m_ga.Termination = m_termination;

            m_sampleContext.GA = m_ga;
            m_ga.GenerationRan += delegate
            {
                Application.Invoke(delegate
                {
                    m_sampleController.Update();
                });
            };

            m_ga.Start();
        });
    }

    private void ResumeGA()
    {
        RunGA(() =>
        {
            m_ga.Population.MinSize = Convert.ToInt32(sbtPopulationMinSize.Value);
            m_ga.Population.MaxSize = Convert.ToInt32(sbtPopulationMaxSize.Value);
            m_ga.Selection = m_selection;
            m_ga.Crossover = m_crossover;
            m_ga.Mutation = m_mutation;
            m_ga.CrossoverProbability = Convert.ToSingle(hslCrossoverProbability.Value);
            m_ga.MutationProbability = Convert.ToSingle(hslMutationProbability.Value);
            m_ga.Reinsertion = m_reinsertion;
            m_ga.Termination = m_termination;

            m_ga.Resume();
        });
    }

    private void RunGA(System.Action runAction)
    {
        try
        {
            runAction();
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

        Application.Invoke(delegate
        {
            btnNew.Visible = true;
            btnResume.Visible = true;
            btnStop.Visible = false;
            vbxGA.Sensitive = true;
        });
    }
    #endregion

    #region Sample methods
    private void PrepareSamples()
    {
        LoadComboBox(cmbSample, TypeHelper.GetDisplayNamesByInterface<ISampleController>());
        m_sampleController = TypeHelper.CreateInstanceByName<ISampleController>(cmbSample.ActiveText);

        // Sample context.
        var layout = new Pango.Layout(this.PangoContext);
        layout.Alignment = Pango.Alignment.Center;
        layout.FontDescription = Pango.FontDescription.FromString("Arial 16");

        m_sampleContext = new SampleContext(drawingArea.GdkWindow, this)
        {
            Layout = layout
        };

        m_sampleContext.GC = m_sampleContext.CreateGC(new Gdk.Color(255, 50, 50));

        m_sampleController.Context = m_sampleContext;
        m_sampleController.Reconfigured += delegate
        {
            ResetSample();
        };

        problemConfigWidgetContainer.Add(m_sampleController.CreateConfigWidget());
        problemConfigWidgetContainer.ShowAll();

        SetSampleOperatorsToComboxes();
        cmbSample.Changed += delegate
        {
            m_sampleController = TypeHelper.CreateInstanceByName<ISampleController>(cmbSample.ActiveText);
            SetSampleOperatorsToComboxes();

            m_sampleController.Context = m_sampleContext;
            m_sampleController.Reconfigured += delegate
            {
                ResetSample();
            };

            if (problemConfigWidgetContainer.Children.Length > 0)
            {
                problemConfigWidgetContainer.Children[0].Destroy();
            }

            problemConfigWidgetContainer.Add(m_sampleController.CreateConfigWidget());
            problemConfigWidgetContainer.ShowAll();

            ResetBuffer();
            ResetSample();
        };
    }

    private void SetSampleOperatorsToComboxes()
    {
        SetSampleOperatorToCombobox(CrossoverService.GetCrossoverTypes, m_sampleController.CreateCrossover, (c) => m_crossover = c, cmbCrossover);
        SetSampleOperatorToCombobox(MutationService.GetMutationTypes, m_sampleController.CreateMutation, (c) => m_mutation = c, cmbMutation);
        SetSampleOperatorToCombobox(SelectionService.GetSelectionTypes, m_sampleController.CreateSelection, (c) => m_selection = c, cmbSelection);
        SetSampleOperatorToCombobox(TerminationService.GetTerminationTypes, m_sampleController.CreateTermination, (c) => m_termination = c, cmbTermination);
    }

    private void SetSampleOperatorToCombobox<TOperator>(Func<IList<Type>> getOperatorTypes, Func<TOperator> getOperator, System.Action<TOperator> setOperator, ComboBox combobox)
    {
        var @operator = getOperator();
        var operatorType = @operator.GetType();

        var opeartorIndex = getOperatorTypes().Select((type, index) => new { type, index }).First(c => c.type.Equals(operatorType)).index;
        combobox.Active = opeartorIndex;
        setOperator(@operator);
    }

    private void UpdateSample()
    {
        DrawSample();
    }

    private void ResetSample()
    {
        m_sampleContext.GC = m_sampleContext.CreateGC(new Gdk.Color(255, 50, 50));
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
            m_sampleContext.WriteText("Fitness: {0:n2}", m_sampleContext.Population.BestChromosome != null ? m_sampleContext.Population.BestChromosome.Fitness : 0.0);
            m_sampleContext.WriteText("Time: {0}", m_ga.TimeEvolving);
        }

        DrawBuffer();
    }
    #endregion

    #region Form methods

    private void PrepareButtons()
    {
        btnStart.Clicked += delegate
        {
            btnStop.Visible = true;
            btnStart.Visible = false;
            RunGAThread(false);
        };

        btnResume.Clicked += delegate
        {
            btnStop.Visible = true;
            btnNew.Visible = false;
            btnResume.Visible = false;
            RunGAThread(true);
        };

        btnStop.Clicked += delegate
        {
            btnStop.Label = "Stopping...";
            btnStop.Sensitive = false;
            m_ga.Stop();
            btnStop.Label = "_Stop";
            btnResume.Visible = true;
            btnStop.Visible = false;
            btnStop.Sensitive = true;
            btnNew.Visible = true;
        };

        btnNew.Clicked += delegate
        {
            btnResume.Visible = false;
            btnStart.Visible = true;
            btnNew.Visible = false;
            vbxSample.Sensitive = true;
        };
    }

    private void PrepareComboBoxes()
    {
        PrepareEditComboBox(
           cmbSelection,
           btnEditSelection,
           SelectionService.GetSelectionNames,
           SelectionService.GetSelectionTypeByName,
           SelectionService.CreateSelectionByName,
           () => m_selection,
           (i) => m_selection = i);

        PrepareEditComboBox(
            cmbCrossover,
            btnEditCrossover,
            CrossoverService.GetCrossoverNames,
            CrossoverService.GetCrossoverTypeByName,
            CrossoverService.CreateCrossoverByName,
            () => m_crossover,
            (i) => m_crossover = i);

        PrepareEditComboBox(
            cmbMutation,
            btnEditMutation,
            MutationService.GetMutationNames,
            MutationService.GetMutationTypeByName,
            MutationService.CreateMutationByName,
            () => m_mutation,
            (i) => m_mutation = i);

        PrepareEditComboBox(
            cmbTermination,
            btnEditTermination,
            () =>
            {
                return TerminationService.GetTerminationNames();
            },
            TerminationService.GetTerminationTypeByName,
            TerminationService.CreateTerminationByName,
            () => m_termination,
            (i) => m_termination = i);

        PrepareEditComboBox(
            cmbReinsertion,
            btnEditReinsertion,
            ReinsertionService.GetReinsertionNames,
            ReinsertionService.GetReinsertionTypeByName,
            ReinsertionService.CreateReinsertionByName,
            () => m_reinsertion,
            (i) => m_reinsertion = i);

        PrepareEditComboBox(
            cmbGenerationStrategy,
            btnEditGenerationStrategy,
            PopulationService.GetGenerationStrategyNames,
            PopulationService.GetGenerationStrategyTypeByName,
            PopulationService.CreateGenerationStrategyByName,
            () => m_generationStrategy,
            (i) => m_generationStrategy = i);
    }

    private void PrepareEditComboBox<TItem>(ComboBox comboBox, Button editButton, Func<IList<string>> getNames, Func<string, Type> getTypeByName, Func<string, object[], TItem> createItem, Func<TItem> getItem, Action<TItem> setItem)
    {
        // ComboBox.
        LoadComboBox(comboBox, getNames());

        comboBox.Changed += delegate
        {
            var item = createItem(comboBox.ActiveText, new object[0]);
            setItem(item);
            ShowButtonByEditableProperties(editButton, item);
        };

        setItem(createItem(comboBox.ActiveText, new object[0]));

        comboBox.ExposeEvent += delegate
        {
            ShowButtonByEditableProperties(editButton, getItem());
        };

        // Edit button.
        editButton.Clicked += delegate
        {
            var editor = new PropertyEditor(getTypeByName(comboBox.ActiveText), getItem());
            editor.Run();
            setItem((TItem)editor.ObjectInstance);
        };
    }

    private void LoadComboBox(ComboBox cmb, IList<string> names)
    {
        foreach (var c in names)
        {
            cmb.AppendText(c);
        }

        cmb.Active = 0;
    }

    private void ShowButtonByEditableProperties(Button editButton, object item)
    {
        if (PropertyEditor.HasEditableProperties(item.GetType()))
        {
            editButton.Show();
        }
        else
        {
            editButton.Hide();
        }
    }

    private void ResetBuffer()
    {
        m_sampleContext.Buffer = new Pixmap(drawingArea.GdkWindow, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
    }

    private void DrawBuffer()
    {
        drawingArea.GdkWindow.DrawDrawable(m_sampleContext.GC, m_sampleContext.Buffer, 0, 0, 0, 0, drawingArea.Allocation.Width, drawingArea.Allocation.Height);
    }
    #endregion
}
