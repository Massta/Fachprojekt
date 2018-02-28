using System.Linq;

namespace FachprojektLibrary
{
    public class Number
    {
        public double Label { get; set; }
        public double Guess { get; set; }
        public double LabelGuess { get { return Guess > 0.5 ? 1 : 0; } }
        public double[] Data { get; set; }
        public double[] NormalizedData
        {
            get
            {
                if (Data == null)
                {
                    return null;
                }
                return Data.Select(d => d / 255).ToArray();
            }
        }
    }
}