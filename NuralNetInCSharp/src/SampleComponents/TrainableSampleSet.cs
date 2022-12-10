using System;
using System.Collections.Generic;

namespace Leitner3.src.SampleComponents
{
    public class TrainableSampleSet
    {
        public TrainableSampleSet()
        {
            Samples = new List<TrainableSample>();
        }

        public List<TrainableSample> Samples { get; }
        public Dictionary<int, string> TargetLabels { get; set; }

        public void AddSample(TrainableSample newSample)
        {
            Samples.Add(newSample);
        }
    }
}
