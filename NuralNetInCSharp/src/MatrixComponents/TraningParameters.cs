using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitner.MatrixComponents
{
    public class TraningParameters
    {
        // Setup values
        public int? batchSize = 80;
        public bool adaptiveBatchSize = false;

        public double maxTrainTime = 30; // secounds, pr train iteration
        public int? maxIterations = 20000;
        public double? verboseAtIntervall = 2.0;

        public string prePrint;
        public double ln = 0.005;
        public double lnDecay = 0.99;

        public double adaptiveBatchSizeChangeDecreaseMultiplier = 0.95; 



        // Self defined values
        public double performance = 0.0;
        public double bestPerformance = 0.0;
        public int epoch = 0;

        public DateTime startTime;
        public double ElapsedTime
        {
            get { return (DateTime.Now - startTime).TotalSeconds; }
        }
        public int PercentDone
        {
            get { return Math.Min((int)(100.0 * ElapsedTime / maxTrainTime), 100); } 
        }
        public int iterations = 0;
        public DateTime lastPrintedStatusTime;


        

        public int GetUpdatedBatchIndex(int lastBatchIndex, int dataSetLengtht)
        {
            return (lastBatchIndex + batchSize.Value) > (dataSetLengtht - 2) ?
                0 :
                (lastBatchIndex + batchSize.Value);
        }

        public void IncreaseBatchSize()
        {
            batchSize = (int)((batchSize + 1) * (2.0 - adaptiveBatchSizeChangeDecreaseMultiplier )  );
        }

        public void DecreaseBatchSize()
        {
            batchSize = Math.Max((int)((batchSize - 1) * adaptiveBatchSizeChangeDecreaseMultiplier), 3);
        }

        internal void UpdateIterations()
        {
            iterations++;
        }
    }
}
