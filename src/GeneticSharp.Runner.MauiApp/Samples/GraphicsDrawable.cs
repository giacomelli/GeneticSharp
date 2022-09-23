namespace GeneticSharp.Runner.MauiApp.Samples
{
    public class GraphicsDrawable : IDrawable
    {
        public static ISampleController Sample { get; set; }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            Sample.Draw(canvas);
        }
    }
}
