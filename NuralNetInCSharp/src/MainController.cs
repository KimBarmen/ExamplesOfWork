using System;
using Leitner.MatrixComponents;
using Leitner.DataSources;
using System.IO;

namespace Leitner
{
    public class MainController
    {
        public void Run()
        {
            char osFileSepperator = System.IO.Path.DirectorySeparatorChar;
            string runPath = Directory.GetCurrentDirectory();

            Console.WriteLine("> Setup");


            var datasetPath = ".."+ osFileSepperator + ".."+ osFileSepperator + "Datasets" + osFileSepperator + "JsonSetsSmall" + osFileSepperator + "ColorsTransformed.json";
            var jsonName = runPath + osFileSepperator + datasetPath;

            Console.WriteLine("\tReading dataset..");
            var reReadSamples = StandardDataSet.Load(jsonName);
            var dataSet = new StandardDataSet(reReadSamples, new string[] { "Training", "Validation" });

            Console.WriteLine("");
            Console.WriteLine("\tBuilding..");
            int input_Size = dataSet.Input_Size;
            int ouput_Size = dataSet.Output_Size;

            Console.Write("\tLables: [");
            foreach (var label in dataSet.TargetLabels)
            {
                Console.Write(label + " ");
            }
            Console.WriteLine("]");
            Console.WriteLine("\tSet-size: " + dataSet.DataSets["Training"].Inputs.Lenght);

            var net = new MatrixNet(
                Input_Size: input_Size,
                Ouput_Size: ouput_Size,
                HiddenLayer_Sizes: new int[] { 50, 10 }
            );

            net.Setup();
            net.PrintArchitecture(pre: "\t");

            Console.WriteLine("> Validate");
            Random r = new Random();
            int validateSize = 200;
            int vIdx = r.Next(0, dataSet.DataSets["Validation"].Inputs.Lenght - validateSize);

            net.Validate(
                dataSet.DataSets["Validation"].Inputs.Cut(vIdx, validateSize),
                        dataSet.DataSets["Validation"].Targets.Cut(vIdx, validateSize)
                    );



            var trainingPrameters = new TraningParameters()
            {
                verboseAtIntervall = 0.5,
            };
            
            while (true)
            {
                trainingPrameters.prePrint = "Training: [" + trainingPrameters.epoch++ + "]"
                    + " BP: " + (int)(trainingPrameters.bestPerformance * 100) + "%"
                    + " LP: " + (int)(trainingPrameters.performance * 100) + "%"
                    + " LN: " + Tools.LPrint( trainingPrameters.ln + "",15,true);

                net.Train(
                    x: dataSet.DataSets["Training"].Inputs,
                    y: dataSet.DataSets["Training"].Targets,
                    tp: trainingPrameters
                );

                if (trainingPrameters.epoch % 10 == 0)
                {
                    Console.WriteLine("> Validate");
                }


                vIdx = r.Next(0, dataSet.DataSets["Validation"].Inputs.Lenght - validateSize);
                trainingPrameters.performance = net.Validate(
                    dataSet.DataSets["Validation"].Inputs.Cut(vIdx, validateSize),
                    dataSet.DataSets["Validation"].Targets.Cut(vIdx, validateSize),
                    labels: dataSet.TargetLabels,
                    printConfusion: trainingPrameters.epoch % 10 == 0,
                    printPerformance: trainingPrameters.epoch % 10 == 0
                );
                trainingPrameters.bestPerformance = Math.Max(trainingPrameters.performance, trainingPrameters.bestPerformance);
                trainingPrameters.ln *= trainingPrameters.lnDecay;


            }

        }
    }
}
