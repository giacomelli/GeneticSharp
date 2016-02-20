using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Extensions.Drawing;
using Gtk;
using HelperSharp;

namespace GeneticSharp.Runner.GtkApp.Samples
{
    [DisplayName("Bitmap equality")]
    public class BitmapEqualitySampleController : SampleControllerBase
    {
        #region Fields
        private BitmapEqualityFitness m_fitness = new BitmapEqualityFitness();
        //// private IChromosome m_lastBest;
        private string m_destFolder;
        private double m_resolution = 1;
        private Bitmap m_targetBitmap;
        private Label m_resolutionLabel;
        #endregion

        #region Methods
        public override IChromosome CreateChromosome()
        {
            return new BitmapChromosome(m_fitness.BitmapWidth, m_fitness.BitmapHeight);
        }

        public override Widget CreateConfigWidget()
        {
            var container = new VBox();
            var selectImageButton = new Button();
            selectImageButton.Label = "Select the image";
            selectImageButton.Clicked += delegate
            {
                Gtk.FileChooserDialog filechooser =
                new Gtk.FileChooserDialog(
                    "Select the image to use",
                Context.GtkWindow,
                FileChooserAction.Open,
                "Cancel",
                ResponseType.Cancel,
                "Open",
                ResponseType.Accept);

                if (filechooser.Run() == (int)ResponseType.Accept)
                {
                    m_targetBitmap = Bitmap.FromFile(filechooser.Filename) as Bitmap;
                    InitializeFitness();

                    var folder = Path.Combine(Path.GetDirectoryName(filechooser.Filename), "results");
                    m_destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
                    Directory.CreateDirectory(m_destFolder);
                }

                filechooser.Destroy();

                OnReconfigured();
            };
            container.Add(selectImageButton);

            // Resolution.
            m_resolutionLabel = new Label();
            m_resolutionLabel.Text = "Resolution";
            container.Add(m_resolutionLabel);

            var resolutionButton = new SpinButton(0.01, 1, 0.01);
            resolutionButton.Value = 100;
            resolutionButton.ValueChanged += delegate
            {
                m_resolution = resolutionButton.Value;
                InitializeFitness();

                OnReconfigured();
            };
            container.Add(resolutionButton);

            return container;
        }

        public override ICrossover CreateCrossover()
        {
            return new UniformCrossover();
        }

        public override IFitness CreateFitness()
        {
            return m_fitness;
        }

        public override IMutation CreateMutation()
        {
            return new TworsMutation();
        }

        public override ISelection CreateSelection()
        {
            return new EliteSelection();
        }

        public override void Draw()
        {
            var ga = Context.GA;

            if (ga != null)
            {
                var generationsNumber = ga.GenerationsNumber;
                var bestChromosome = ga.BestChromosome;

                //// if (generationsNumber == 1 || (generationsNumber % 200 == 0 && m_lastBest.Fitness != bestChromosome.Fitness))
                if (bestChromosome != null)
                {
                    var best = bestChromosome as BitmapChromosome;
                    var buffer = Context.Buffer;
                    var gc = Context.GC;
                    var layout = Context.Layout;

                    using (var bitmap = best.BuildBitmap())
                    {
                        //// bitmap.Save("{0}/{1}_{2}.png".With(m_destFolder, generationsNumber.ToString("D10"), best.Fitness));

                        using (var ms = new MemoryStream())
                        {
                            var converter = new ImageConverter();

                            var imageBytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                            var pb = new Gdk.Pixbuf(imageBytes);
                            var width = Context.DrawingArea.Width;
                            var height = Context.DrawingArea.Height;

                            pb = pb.ScaleSimple(width, height, Gdk.InterpType.Nearest);
                            buffer.DrawPixbuf(gc, pb, 0, 0, 0, 100, width, height, Gdk.RgbDither.None, 0, 0);
                        }
                    }

                    //// m_lastBest = best;
                }
            }
        }

        public override void Reset()
        {
            ////var targetBitmap = Bitmap.FromFile(inputImageFile) as Bitmap;
            ////m_fitness = new BitmapEqualityFitness(targetBitmap);

            ////var folder = Path.Combine(Path.GetDirectoryName(inputImageFile), "results");
            ////m_destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
            ////Directory.CreateDirectory(m_destFolder);
            ////Console.WriteLine("Results images will be written to '{0}'.", m_destFolder);

            ////Console.WriteLine("Minutes to evolve:");
            ////m_minutesToEvolve = Convert.ToInt32(Console.ReadLine());
        }

        public override void Update()
        {
        }

        private void InitializeFitness()
        {
            if (m_targetBitmap != null)
            {
                var resolutionSize = new Size(Convert.ToInt32(m_targetBitmap.Width * m_resolution), Convert.ToInt32(m_targetBitmap.Height * m_resolution));
                var resizedBitmap = new Bitmap(m_targetBitmap, resolutionSize);
                m_fitness.Initialize(resizedBitmap);
            }

            Application.Invoke(delegate
            {
                m_resolutionLabel.Text = "Resolution {0}x{1}".With(m_fitness.BitmapWidth, m_fitness.BitmapHeight);
            });
        }
        #endregion
    }
}
