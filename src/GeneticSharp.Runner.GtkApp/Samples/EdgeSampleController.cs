using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Accord.Diagnostics;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using Gtk;

namespace GeneticSharp.Runner.GtkApp.Samples
{

  /// <summary>
  /// Cette classe rajoute notre algorithme génétique à la liste de ceux disponibles dans la liste déroulante.
  /// </summary>
  [DisplayName("Edge")]
   class EdgeSampleController : SampleControllerBase
   {

      //La matrice doit être de taille impaire, on définit ici la moitié de la taille moins un: demitaille 1 -> taille 3, 2 -> 5 etc.
      public const int DemiTailleNoyau = 2;
      //Les coef de la matrice sont entier et évoluent entre - et + la valeur suivante
      public const int ValeursMaxCoefficients = 4;
      //Nombre de gènes dont les matrices sont additionnées pour donner la matrice complète
      public const int TailleChromosome = 20;



      protected EdgeFitness m_fitness;
      protected double m_resolution = 0.4;
      private Label m_resolutionLabel;
      // Filtre classique de détection de bords par convolution avec la matrice ( 0  -1   0, -1   4  -1, 0  -1   0)
      private Edges edgeFilter = new Edges();
      

      
      /// <summary>
      /// On fournit ici le chromose typé qui représente un filtre par convolution
      /// </summary>
      /// <returns></returns>
      public override IChromosome CreateChromosome()
      {
         return new EdgeChromosome(TailleChromosome);
      }

      /// <summary>
      /// Cette méthode créé la partie de l'interface graphique spécifique à notre algorithme: définition de la résolution cible pour simplifier l'image, et chargement de l'image d'entrainement
      /// </summary>
      /// <returns></returns>
      public override Widget CreateConfigWidget()
      {
         var container = new VBox();


         // Resolution de l'image.
         m_resolutionLabel = new Label();
         m_resolutionLabel.Text = "Resolution";
         container.Add(m_resolutionLabel);

         var resolutionButton = new SpinButton(0.01, 1, 0.01);
         resolutionButton.Value = m_resolution;
         resolutionButton.ValueChanged += delegate
         {
            m_resolution = resolutionButton.Value;

      //OnReconfigured();
   };
         container.Add(resolutionButton);


         // Sélecteur d'image
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
               var imageOriginale = Bitmap.FromFile(filechooser.Filename) as Bitmap;
               InitializeFitness(imageOriginale);
            }

            filechooser.Destroy();

            OnReconfigured();
         };
         container.Add(selectImageButton);

       
         return container;
      }


      /// <summary>
      /// On garde ici la méthode de croisement par défaut de l'agorithme dont on s'est inspiré, il est possible de la redéfinir par sélection  dans l'interface
      /// </summary>
      /// <returns></returns>
      public override ICrossover CreateCrossover()
      {
         return new UniformCrossover();
      }

      /// <summary>
      /// La méthode d'évaluation est créée au chargement de l'image et conservée dans un membre de la classe, on se contente de renvoyer ce membre
      /// </summary>
      /// <returns></returns>
      public override IFitness CreateFitness()
      {
         return m_fitness;
      }

      /// <summary>
      /// On garde ici la méthode de mutation par défaut de l'agorithme dont on s'est inspiré, il est possible de la redéfinir par sélection dans l'interface
      /// </summary>
      /// <returns></returns>
      public override IMutation CreateMutation()
      {
         return new TworsMutation();
      }

      /// <summary>
      /// On garde ici la méthode de sélection par défaut de l'agorithme dont on s'est inspiré, il est possible de la redéfinir par sélection dans l'interface
      /// </summary>
      /// <returns></returns>
      public override ISelection CreateSelection()
      {
         return new EliteSelection();
      }

      /// <summary>
      /// Cette méthode est appelée à chaque génération pour rafraichir l'image affichée.
      /// </summary>
      public override void Draw()
      {
         var ga = Context.GA;
         if (m_fitness != null)
         {
            if (ga != null)
            {
               var generationsNumber = ga.GenerationsNumber;
               var bestChromosome = ga.BestChromosome;

               
               if (bestChromosome != null)
               {
                  // Si l'algorithme est en cours, on se base sur le meilleur chromosome
                  var best = bestChromosome as EdgeChromosome;

                  var layout = Context.Layout;

                  //  Générer le filtre issu du chromosome
                  var monFiltre = best.GetFilter();
                  lock (this.m_fitness)
                  {
                     // Appliquer le filtre à l'image originale
                     using (var imageFiltreeParMonChromosome = monFiltre.Apply(m_fitness.ImageOriginale))
                     {
                        Bitmap imageAAfficher = null;
                        //On affiche alternativement plusieurs images pour pouvoir comparer le résultat de diverses manières. L'utilisation du modulo vis à vis des générations permet de gérer l'alternance
                        switch (ga.GenerationsNumber / 20 % 10)
                        {
                          
                           case 6:
                              // On affiche le filtre idéal que l'on souhaite approximer
                              imageAAfficher = m_fitness.ImageBienFiltree;
                              Context.WriteText($"Image issue du filtre idéal");
                              break;
                           case 7:
                              // On affiche le résultat d'un filtre connu
                              imageAAfficher = edgeFilter.Apply(m_fitness.ImageOriginale);
                              Context.WriteText($"Image du filtre par convolution connu");

                              break;
                           case 8:
                              // On affiche la différence entre le filtre à atteindre et notre meilleur filtre
                              imageAAfficher = m_fitness.Differenciateur.Apply(imageFiltreeParMonChromosome);
                              Context.WriteText($"Différence entre le meilleur et la cible");
                              break;

                           default:
                              // On affiche l'image résultant de notre meilleur filtre
                              imageAAfficher = imageFiltreeParMonChromosome;
                              Context.WriteText($"Matrice: {best.GetDescription()}...");
                              break;

                        }
                        DrawBitmap(imageAAfficher);
                     }
                  }
               }
            }
            else
            {
               // L'algorithme n'est pas encore lancée, on affiche l'image bien filtrée que l'on souhaite approximer
               DrawBitmap(m_fitness.ImageBienFiltree);
            }
         }
      }





      public override void Reset()
      {
      }

      public override void Update()
      {
       
      }


      /// <summary>
      /// Initialisation de la fonction d'évaluation à partir de l'image chargée
      /// </summary>
      /// <param name="imageOriginale"></param>
      private void InitializeFitness(Bitmap imageOriginale)
      {
         // On applique la résolution choisie
         var resolutionSize = new Size(Convert.ToInt32(imageOriginale.Width * m_resolution), Convert.ToInt32(imageOriginale.Height * m_resolution));
         var resizedBitmap = new Bitmap(imageOriginale, resolutionSize);
         //On passe l'image en nuances de gris
         resizedBitmap = Grayscale.CommonAlgorithms.RMY.Apply(resizedBitmap);
         // On créée la fonction d'évaluation correspondante
         m_fitness = new EdgeFitness(resizedBitmap);

         //On affiche la taille de l'image résultante
         Application.Invoke(delegate { m_resolutionLabel.Text = $"Resolution {resizedBitmap.Width}x{resizedBitmap.Height}"; });
      }

      /// <summary>
      /// Méthode qui dessine l'image dans l'UI
      /// </summary>
      /// <param name="bitmap"></param>
      private void DrawBitmap(Bitmap bitmap)
      {
         using (var ms = new MemoryStream())
         {
            var buffer = Context.Buffer;
            var gc = Context.GC;
            var converter = new ImageConverter();

            var imageBytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            var pb = new Gdk.Pixbuf(imageBytes);
            var width = Context.DrawingArea.Width;
            var height = Context.DrawingArea.Height;

            pb = pb.ScaleSimple(width, height, Gdk.InterpType.Nearest);
            buffer.DrawPixbuf(gc, pb, 0, 0, 0, 100, width, height, Gdk.RgbDither.None, 0, 0);
         }
      }

   }

   /// <summary>
   /// Classe chargée d'effectuer l'évaluation de nos chromosomes à partir de l'image cible
   /// </summary>
   public class EdgeFitness : IFitness
   {

      /// <summary>
      /// Il s'agit d'une classe fournie par le framework Accord.Net pour la comparison d'images
      /// </summary>
      private ExhaustiveTemplateMatching comparateurDimage = new ExhaustiveTemplateMatching(0.5F);

      /// <summary>
      /// Ce filtre calcule la soustraction de 2 images et fournit une alternative pour leur comparaison
      /// </summary>
      private Difference _differenciateur;

      /// <summary>
      /// ON stocke l'image originale dans ce membre
      /// </summary>
      public Bitmap ImageOriginale;

      /// <summary>
      /// On stocke l'image issue d'un filtre idéal à approxime ici pour pouvoir les comparer par la suite
      /// </summary>
      private Bitmap _imageBienFiltree;

      /// <summary>
      /// Cette propriété fournit l'image bien filtrée de comparaison en "lazylaoding". Elle la créée la première fois qu'elle est appelée
      /// </summary>
      public Bitmap ImageBienFiltree
      {
         get
         {
            if (_imageBienFiltree == null)
            {
               lock (this)
               {
                  if (_imageBienFiltree == null)
                  {
                     var leBonDetecteur = new SobelEdgeDetector();
                     _imageBienFiltree = leBonDetecteur.Apply(ImageOriginale);
                  }
               }
            }
            return _imageBienFiltree;
         }
      }

      /// <summary>
      /// Cette propriété fournit le filtre capable de calculer la soustraction de l'image bien filtrée avec une image cible. Il est céée également en lazyloading.
      /// </summary>
      public Difference Differenciateur
      {
         get
         {
            if (_differenciateur == null)
            {
               _differenciateur = new Difference(ImageBienFiltree);
            }
            return _differenciateur;
         }
      }


      /// <summary>
      /// Constructeur qui prend en paramètre l'image chargée sur laquelle nous allons travailler
      /// </summary>
      /// <param name="imageOriginale">L'image redimensionnée qui servira à nos différents cibles</param>
      public EdgeFitness(Bitmap imageOriginale)
      {
         this.ImageOriginale = imageOriginale;
      }

      /// <summary>
      /// Fonction principale du GA permettant d'évaluer un individu, c'est à dire un chromosome
      /// </summary>
      /// <param name="chromosome"></param>
      /// <returns></returns>
      public double Evaluate(IChromosome chromosome)
      {
         return Evaluate((EdgeChromosome)chromosome);
      }

      /// <summary>
      /// Fonction permettant d'évaluer un chromosome du type qui nous intéresse
      /// </summary>
      /// <param name="chromosome"></param>
      /// <returns></returns>
      public double Evaluate(EdgeChromosome chromosome)
      {
         //  Générer le filtre issu du chromosome
         var monFiltre = chromosome.GetFilter();
         // Appliquer le filtre à l'image originale
         Bitmap imageFiltreeParMonChromosome;
         lock (this)
         {
            imageFiltreeParMonChromosome = monFiltre.Apply(ImageOriginale);
         }

         // Comparer avec le résultat d'un bon détecteur de bords
         // En utilisant un comparateur dédié
         var similarites = comparateurDimage.ProcessImage(
           imageFiltreeParMonChromosome, ImageBienFiltree);
         // retourner le nombre de similarites
         if (similarites.Length > 0)
         {
            if (similarites.Length > 1)
            {
               Debugger.Break();
            }
            return similarites[0].Similarity *1000;
         }

         return 0;

         // Version alternative: on prend les différence entre les deux images, et la moyenne des pixels qui restent
         //var imageDif = Differenciateur.Apply(imageFiltreeParMonChromosome);
         //return (-imageDif.Mean())*1000;

      }



   }

   /// <summary>
   /// Classe de chromosome représentant un filtre par convolution
   /// </summary>
   public class EdgeChromosome : ChromosomeBase
   {

     
      //private const int convolutionDivisor = 10;

         /// <summary>
         /// Le noyau de la convolution est une matrice dont la taille doit être impaire. Cette taille est définie ici à partir d'une constante.
         /// </summary>
      private const int kernelSize = 2* EdgeSampleController.DemiTailleNoyau +1;

      /// <summary>
      /// Constructeur du chromosome
      /// </summary>
      /// <param name="length">Nombre de gènes du chromosome</param>
      public EdgeChromosome(int length) : base(length)
      {
         for (int i = 0; i < Length; i++)
         {
            ReplaceGene(i, GenerateGene(i));
         }
      }

      /// <summary>
      /// Création d'un nouveau chromosome à partir d'un chromosome existant: on conserve le nombre de gènes
      /// </summary>
      /// <returns>Un chronomose au même nombre de gènes</returns>
      public override IChromosome CreateNew()
      {
         return new EdgeChromosome(Length);
      }

      /// <summary>
      /// Initialise un gène contenant une matrice de convolution 
      /// </summary>
      /// <param name="geneIndex"></param>
      /// <returns>le gène créé</returns>
      public override Gene GenerateGene(int geneIndex)
      {
         var rnd = RandomizationProvider.Current;
         ///On initialise une matrice de la bonne taille de façon aléatoire.
         var matrix = new int[kernelSize, kernelSize];
         for (int i = 0; i < (kernelSize/2)+1; i++)
         {
            for (int j = 0; j < (kernelSize / 2) + 1; j++)
            {
               //On veut une détection symmétrique et donc une matrice également symétrique
               matrix[i, j] = rnd.GetInt(-EdgeSampleController.ValeursMaxCoefficients, EdgeSampleController.ValeursMaxCoefficients);
               matrix[kernelSize - i - 1, j] = matrix[i, j];
               matrix[i, kernelSize - 1 - j] = matrix[i, j];
               matrix[kernelSize - i - 1, kernelSize - 1 - j] = matrix[i, j];
               matrix[j, i] = matrix[i, j];
               matrix[kernelSize - 1 - j, kernelSize - 1 - i] = matrix[i, j];

            }
         }
         ///Le gène retourné contient la matrice de convolution
         return new Gene(matrix);
      }

      /// <summary>
      /// Transforme le chromosome en un filtre de traitement d'image
      /// </summary>
      /// <returns></returns>
      public IFilter GetFilter()
      {
         var matriceComplete = GetMatriceComplete();
         //Le filtre de convolution est créé directement à partir de la matrice, sans autre paramètre
         return new Convolution(matriceComplete);
         //return new Convolution(matriceComplete, convolutionDivisor);
      }

      /// <summary>
      /// La matrice de convolution de l'individu est calculée en sommant toutes celles des gènes qu'il contient.
      /// </summary>
      /// <returns></returns>
      public int[,] GetMatriceComplete()
      {
         var matriceComplete = Matrix.Create<int>(kernelSize,
            kernelSize, 0);
         foreach (var gene in GetGenes())
         {
            int[,] matriceDuGene = (int[,])gene.Value;
            matriceComplete = matriceComplete.Add(matriceDuGene);
         }

         return matriceComplete;
      }

      /// <summary>
      /// Cette méthode fournit une description du filtre en affichant la matrice complète résultante
      /// </summary>
      /// <returns></returns>
      public string GetDescription()
      {
         var matriceComplete = GetMatriceComplete();
         return matriceComplete.ToString(false, new OctaveMatrixFormatProvider(CultureInfo.InvariantCulture));
      }
   }


}
