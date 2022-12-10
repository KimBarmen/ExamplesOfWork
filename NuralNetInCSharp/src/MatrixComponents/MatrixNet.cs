using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitner.MatrixComponents
{
    public class MatrixNet
    {
        readonly int Input_Size;
        readonly int Ouput_Size;
        readonly int[] HiddenLayer_Sizes;

        MatrixLayer[] AllLayers;

        public MatrixNet(int Input_Size, int Ouput_Size, int[] HiddenLayer_Sizes)
        {
            this.Input_Size = Input_Size;
            this.Ouput_Size = Ouput_Size;
            this.HiddenLayer_Sizes = HiddenLayer_Sizes;
        }

        public void Setup()
        {
            AllLayers = new MatrixLayer[this.HiddenLayer_Sizes.Length + 1];
            AllLayers[0] = new MatrixLayer(Input_Size, HiddenLayer_Sizes[0]);
            for (int i = 0; i < HiddenLayer_Sizes.Length - 1; i++)
            {
                AllLayers[i + 1] = new MatrixLayer(HiddenLayer_Sizes[i], HiddenLayer_Sizes[i + 1]);
            }
            AllLayers[AllLayers.Length - 1] = new MatrixLayer(HiddenLayer_Sizes[HiddenLayer_Sizes.Length-1], Ouput_Size);
        }

        public void Train(Matrix x, Matrix y, TraningParameters tp)
        {
            Random random = new();

            tp.startTime = DateTime.Now;
            tp.lastPrintedStatusTime = DateTime.Now.AddDays(-1);
            tp.maxIterations ??= int.MaxValue;
            tp.batchSize ??= x.Lenght;
            int batchIdx = random.Next(0,x.Lenght-2);

            string runaway = "";

            while ((tp.iterations < tp.maxIterations) && (tp.ElapsedTime < tp.maxTrainTime))
            {
                Matrix bx = x.Cut((int) batchIdx, (int) tp.batchSize);
                Matrix by = y.Cut((int) batchIdx, (int) tp.batchSize);

                batchIdx = tp.GetUpdatedBatchIndex(batchIdx, x.Lenght);

                TrainOnce(bx, by, tp.ln);

                var timeToBeVerbose = tp.lastPrintedStatusTime.AddSeconds(tp.verboseAtIntervall.GetValueOrDefault() );
                var now = DateTime.Now;
                if (tp.verboseAtIntervall.HasValue && (now > timeToBeVerbose))
                {
                    var percentDone = tp.PercentDone;
                    var barLength = 20;
                    Tools.ProgressBar( ((int) percentDone)/ (100/barLength), barLength,
                        prefix: tp.prePrint + "\t" + string.Format("{0:0.0}", tp.ElapsedTime) + "s ",
                        postfix: ((int)Math.Min(percentDone,100)) + "   " 
                            + Tools.LPrint("I:" + tp.iterations, 10,true)
                            + Tools.LPrint("E:" + GetLayerErrorsForPrint(),35,true)
                            + Tools.LPrint(" Batch: {BS:" + tp.batchSize + " -> BI:" + batchIdx + "} RN:" + runaway  + " ",40,true),
                        space: '-'
                    );
                    tp.lastPrintedStatusTime = DateTime.Now;

                    if (tp.adaptiveBatchSize)
                    {
                        if ((now - timeToBeVerbose).TotalSeconds > tp.verboseAtIntervall.Value) tp.DecreaseBatchSize();
                        else tp.IncreaseBatchSize();
                    }
                }

                runaway = AllLayers[0].Output_Error.T().GetErrorForPrint();



                tp.UpdateIterations();
                
            }
            Console.WriteLine();
        }


        public void TrainOnce(Matrix x, Matrix y, double learningRate)
        {
            // Forward
            AllLayers[0].Input = x;
            AllLayers[0].Forward();

            for (int i = 1; i < AllLayers.Length; i++)
            {
                AllLayers[i].Input = AllLayers[i - 1].Output;
                AllLayers[i].Forward();
            }

            // Backward
            var lastLayer = AllLayers[ AllLayers.Length -1 ];
            var secondToLastLayer = AllLayers[AllLayers.Length - 2];

            lastLayer.Backward_LastLayer(secondToLastLayer, y);


            for (int i = AllLayers.Length - 2; i >= 1; i--)
            {
                var nextLayer = AllLayers[i + 1];
                var currentLayer = AllLayers[i];
                var prevousLayer = AllLayers[i - 1];

                currentLayer.Backward(prevousLayer, nextLayer);
            }

            AllLayers[0].Backward_FirstLayer(AllLayers[1]);


            // Forward - Update
            for (int i = 0; i < AllLayers.Length; i++)
            {
                AllLayers[i].UpdateW(learningRate);
            }
        }

        public double Validate(Matrix x, Matrix y, bool printConfusion = true, bool printPerformance = true, string[] labels = null)
        {
            AllLayers[0].Input = x;
            AllLayers[0].Forward();

            for (int i = 1; i < AllLayers.Length; i++)
            {
                AllLayers[i].Input = AllLayers[i - 1].Output;
                AllLayers[i].Forward();
            }

            var result_correct = 0;
            var result_failed = 0;
            var lastLayerOutput = AllLayers[ AllLayers.Length-1 ].Output;
            var confusionMatrix = new Matrix(y.Height, y.Height, 0);

            for (int i = 0; i < lastLayerOutput.Value.GetLength(0); i++)
            {
                double outputValueAtMax = -10000;
                int outputIndexAtMax = 0;

                for (int j = 0; j < lastLayerOutput.Value.GetLength(1); j++)
                {
                    if (lastLayerOutput.Value[i, j] > outputValueAtMax)
                    {
                        outputIndexAtMax = j;
                        outputValueAtMax = lastLayerOutput.Value[i, j];
                    }
                }
                double targetValueAtMax = -100000;
                int targetIndexAtMax = 0;

                for (int j = 0; j < lastLayerOutput.Value.GetLength(1); j++)
                {
                    if (y.Value[i, j] > targetValueAtMax)
                    {
                        targetIndexAtMax = j;
                        targetValueAtMax = y.Value[i, j];
                    }
                }
                confusionMatrix.Value[outputIndexAtMax, targetIndexAtMax] += 1;

                if (outputIndexAtMax == targetIndexAtMax)
                {
                    result_correct++;
                }
                else
                {
                    result_failed++;
                }
            }

            if (printConfusion)
            {
                if (labels != null)
                {
                    Console.Write("\t");
                    Tools.Print(labels,"\n");
                }
                confusionMatrix.Print(decimals: 1, prefix: "\t");
            }

            double performance = (result_correct + 0.0) / (result_failed + result_correct + 0.0);

            if (printPerformance)
            {
                Console.WriteLine("\tPerformance: " + " " +  string.Format("{0:0.0}", performance*100));
            }

            return performance;
        }

        public void PrintLayerError(Matrix targets)
        {
            Console.WriteLine("Forward");
            AllLayers[ AllLayers.Length-1 ].Output.T().Print();
            Console.WriteLine("Targets: ");
            targets.T().Print();
            Console.WriteLine("Error Output:");
            for (int i = 0; i < AllLayers.Length; i++)
            {
                Console.WriteLine(AllLayers[i].Output_Error.GetErrorForPrint());
            }
        }

        public void PrintArchitecture(string pre="")
        {
            Console.Write(pre + "Architecture: [ ");
            for (int i = 0; i < AllLayers.Length; i++)
            {
                Console.Write(AllLayers[i].Input_Size + " --> ");
            }
            Console.WriteLine(AllLayers[ AllLayers.Length -1 ].Ouput_Size + " ]");

        }

        public string GetLayerErrorsForPrint()
        {
            string err = "[ ";

            for (int i = 0; i < AllLayers.Length; i++)
            {
                err += AllLayers[i].Output_Error.T().GetErrorForPrint() + "   ";
            }

            return err + "]";
        }

    }
}
