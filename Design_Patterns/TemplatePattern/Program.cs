using System;

namespace TemplatePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourdough = new Sourdough();
            var twelveGrain = new TwelveGrain();
            var wholeWheat = new WholeWheat();

            sourdough.Make();
            twelveGrain.Make();
            wholeWheat.Make();
        }
    }
}
