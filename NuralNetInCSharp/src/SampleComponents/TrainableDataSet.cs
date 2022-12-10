using System;
using System.Collections.Generic;
using System.Linq;

namespace Leitner3.src.SampleComponents
{
    public class TrainableDataSet
    {
        public TrainableDataSet( string[] partitionNames, TrainableSample[] samples)
        {
            var totalSamples = samples.Length;
            var samplesPrPartition = samples.Length / partitionNames.Length;

            Random r = new();

            foreach (string partitionName in partitionNames)
            {
                AllPartitions.Add(partitionName, new TrainableDataSetPartition(partitionName));
            }

            var partionsList = AllPartitions.Select(o => o.Value).ToArray();
            var sampleList = new List<TrainableSample>(samples);
            var partitionIdx = 0;

            while (sampleList.Count > 0)
            {
                var selectedIdx = r.Next(0, sampleList.Count);
                partionsList[partitionIdx].samples.Add( sampleList[selectedIdx] );
                sampleList.RemoveAt(selectedIdx);
                partitionIdx = (partitionIdx + 1) % partionsList.Length;
            }
        }

        public Dictionary<string, TrainableDataSetPartition> AllPartitions;
        public int TotalSamples { get { return AllPartitions.Select(o => o.Value.Length).Sum(); ; } }


        public string GetStats()
        {
            var str = "";
            foreach (var partition in AllPartitions.Select(o => o.Value))
            {
                str = str + partition.Name + ": " + partition.Length + ", ";

            }
            return str.Trim();
        }
    }



    public class TrainableDataSetPartition
    {
        public TrainableDataSetPartition(string name)
        {
            Name = name;
        }

        public List<TrainableSample> samples;
        public string Name { get; }
        public int Length { get { return samples.Count; }}
    }
}
