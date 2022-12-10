using System;
namespace Leitner3.src.SampleComponents
{
    public class TrainableSample
    {
        public TrainableSample(double[] value, double[] target)
        {
            this.Value = value;
            this.Taraget = target;
        }

       public double[] Value { get; }
       public double[] Taraget { get; }
    }
}
