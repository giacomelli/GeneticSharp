using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Drawing;
using GeneticSharp.Runner.ConsoleApp.Samples;
using HelperSharp;
using ImageMagick;

namespace GeneticSharp.Runner.ConsoleApp
{
    [DisplayName("Bitmap equality")]
    public class BitmapEqualitySampleController : SampleControllerBase
    {
        private BitmapEqualityFitness m_fitness;
        private IChromosome m_lastBest;
        private string m_destFolder;
        private int m_minutesToEvolve;

        public BitmapEqualitySampleController()
        {
        }

        #region implemented abstract members of SampleControllerBase

        public override IChromosome CreateChromosome()
        {
            return new BitmapChromosome(m_fitness.BitmapWidth, m_fitness.BitmapHeight);
        }

        public override IFitness CreateFitness()
        {
            return m_fitness;
        }

        public override ITermination CreateTermination()
        {
            return new TimeEvolvingTermination(TimeSpan.FromMinutes(m_minutesToEvolve));
        }

        public override IMutation CreateMutation()
        {            
            return new TworsMutation();
        }

        public override GeneticSharp.Domain.Crossovers.ICrossover CreateCrossover()
        {            
            return new UniformCrossover();
        }

        public override void Initialize()
        {
            base.Initialize();

            Console.WriteLine("Input image file:");
            var inputImageFile = Console.ReadLine();

            var targetBitmap = Bitmap.FromFile(inputImageFile) as Bitmap;
            m_fitness = new BitmapEqualityFitness(targetBitmap);

            var folder = Path.Combine(Path.GetDirectoryName(inputImageFile), "results");
            m_destFolder = "{0}_{1:yyyyMMdd_HHmmss}".With(folder, DateTime.Now);
            Directory.CreateDirectory(m_destFolder);
            Console.WriteLine("Results images will be written to '{0}'.", m_destFolder);

            Console.WriteLine("Minutes to evolve:");
            m_minutesToEvolve = Convert.ToInt32(Console.ReadLine());
        }

        public override void ConfigGA(GeneticAlgorithm ga)
        {
            base.ConfigGA(ga);
            ga.MutationProbability = 0.4f;
            ga.TerminationReached += (sender, args) =>
            {
                using (var collection = new MagickImageCollection())
                {
                    var files = Directory.GetFiles(m_destFolder, "*.png");

                    foreach (var image in files)
                    {
                        collection.Add(image);
                        collection[0].AnimationDelay = 100;
                    }

                    var settings = new QuantizeSettings();
                    settings.Colors = 256;
                    collection.Quantize(settings);

                    collection.Optimize();
                    collection.Write(Path.Combine(m_destFolder, "result.gif"));
                }
            };
        }

        public override void Draw(IChromosome bestChromosome)
        {
            if (GA.GenerationsNumber == 1 || (GA.GenerationsNumber % 200 == 0 && m_lastBest.Fitness != bestChromosome.Fitness))
            {
                var best = bestChromosome as BitmapChromosome;

                using (var bitmap = best.BuildBitmap())
                {
                    bitmap.Save("{0}/{1}_{2}.png".With(m_destFolder, GA.GenerationsNumber.ToString("D10"), best.Fitness));
                }

                m_lastBest = best;
            }
        }

        #endregion
    }
}