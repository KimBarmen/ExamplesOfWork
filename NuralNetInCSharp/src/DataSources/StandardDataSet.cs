using Leitner.MatrixComponents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leitner.DataSources
{

    public class StandardSample
    {
        public string TargetAsString;
        public double[] Input;
        public double[] TargetAsArray;
    }

    public class StandardDataSet
    {
        int MAX_JSON_SAMPLE_COUNT = 100;

        public int Input_Size;
        public int Output_Size;
        public int TotalNumberOfSamples;
        public int NumberOfSubsets;
        public string[] TargetLabels;

        public StandardSample[] AllSamples;
        public Dictionary<string, MatrixSubSet> DataSets;

        public class MatrixSubSet
        {
            public string Name;
            public Matrix Inputs;
            public Matrix Targets;
        }

        public StandardDataSet(StandardSample[] samples, string[] subsetNames)
        {
            AllSamples = samples;
            TotalNumberOfSamples = samples.Length;

            DataSets = SplitSamplesIntoMatrixSubSets(samples, subsetNames);
            Input_Size = samples[0].Input.Length;
            Output_Size = samples[0].TargetAsArray.Length;
            TargetLabels = samples.Select(o => o.TargetAsString).Distinct().ToArray();
        }

        public void Save(string path)
        {
            var tmpList = new List<StandardSample>();

            int idx = 0;
            do
            {
                Tools.ProgressBar((int)((idx + 0.0) / (AllSamples.Length + 0.0) * 50.0), 50,
                    postfix: " " + idx + " / " + AllSamples.Length,
                    prefix: "\t\t");

                var countToTake = Math.Min(MAX_JSON_SAMPLE_COUNT, AllSamples.Length - idx);
                tmpList = AllSamples.Skip(idx).Take(countToTake).ToList();
                using (StreamWriter file = File.CreateText(path + "." + idx + "-" + (idx + countToTake)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, tmpList.ToArray());
                }
                idx += countToTake;

            } while (idx < AllSamples.Length);
        }

        public static StandardSample[] Load(string path)
        {
            var sepperator = Path.DirectorySeparatorChar;
            var dirname = String.Join(sepperator + "", path.Split(sepperator).Reverse().Skip(1).Reverse());
            var filename = path.Split(Path.DirectorySeparatorChar).Reverse().Take(1).First().Split('.').First();
            var filesInDir = Directory.GetFiles(dirname, filename + "*");
            var allSamplesArrays = new List<StandardSample>();

            for (int i = 0; i < filesInDir.Length; i++)
            {
                var jsonFile = filesInDir[i];
                Tools.ProgressBar((int)((i + 0.0) / (filesInDir.Length + 0.0) * 50.0), 50,
                    postfix: " " + i + " / " + filesInDir.Length,
                    prefix: "\t\t");

                using (var file = File.OpenText(jsonFile))
                {
                    string jsonString = file.ReadToEnd();
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<StandardSample[]>(jsonString);
                    allSamplesArrays.AddRange(obj);
                }

            }

            return allSamplesArrays.ToArray();
        }

        private static Dictionary<string, MatrixSubSet> SplitSamplesIntoMatrixSubSets(StandardSample[] samples, string[] names)
        {
            int sizeOfSubsets = (samples.Length / names.Length) - 1;
            var tmpAllInputs = new double[samples.Length, samples[0].Input.Length];
            var tmpAllOutputs = new double[samples.Length, samples[0].TargetAsArray.Length];
            var resultingCollettionOfDatasets = new Dictionary<string, MatrixSubSet>();

            for (int i = 0; i < tmpAllInputs.GetLength(0); i++)
            {
                for (int j = 0; j < tmpAllInputs.GetLength(1); j++)
                {
                    tmpAllInputs[i, j] = samples[i].Input[j];
                }
                for (int j = 0; j < tmpAllOutputs.GetLength(1); j++)
                {
                    tmpAllOutputs[i, j] = samples[i].TargetAsArray[j];
                }
            }

            for (int k = 0; k < names.Length; k++)
            {
                var tmpSubsetInput = new double[sizeOfSubsets, samples[0].Input.Length];
                var tmpSubsetTarget = new double[sizeOfSubsets, samples[0].TargetAsArray.Length];
                int startingIndexOfSubsetInMainSet = k * sizeOfSubsets;

                for (int i = 0; i < sizeOfSubsets; i++)
                {
                    for (int j = 0; j < tmpAllInputs.GetLength(1); j++)
                    {
                        tmpSubsetInput[i, j] = tmpAllInputs[i + startingIndexOfSubsetInMainSet, j];
                    }
                    for (int j = 0; j < tmpAllOutputs.GetLength(1); j++)
                    {
                        tmpSubsetTarget[i, j] = tmpAllOutputs[i + startingIndexOfSubsetInMainSet, j];
                    }
                }

                resultingCollettionOfDatasets.Add(names[k], new MatrixSubSet
                {
                    Name = names[k],
                    Inputs = new Matrix(tmpSubsetInput),
                    Targets = new Matrix(tmpSubsetTarget)
                });

            }


            return resultingCollettionOfDatasets;
        }
    }
}
