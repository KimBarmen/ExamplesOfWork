

namespace Leitner.MatrixComponents
{
    public class MatrixLayer
    {
        public int Input_Size;
        public int Ouput_Size;
        private int LastSampleLenght;

        public MatrixLayer(int input_Size, int ouput_Size)
        {
            Input_Size = input_Size;
            Ouput_Size = ouput_Size;
            W = new Matrix(Input_Size, Ouput_Size);
        }

        public Matrix Input { get; set; }
        public Matrix Output { get; set; }
        public Matrix Output_Error { get; private set; }

        private Matrix Output_Delta { get; set; }
        private Matrix W { get; set; }
        private Matrix W_Delta { get; set; }

        public void Forward()
        {
            Output = Input.Dot(W).Bias(1.0).Relu();
            LastSampleLenght = Input.Lenght;
        }

        public void Backward(MatrixLayer prevousLayer, MatrixLayer nextLayer)
        {
            Output_Error = nextLayer.Backward_PrevousLayerOutputError();
            Output_Delta = Matrix.Multiply(Output_Error, Output.Relu_deriv());
            W_Delta = Matrix.Dot(prevousLayer.Output.T(), Output_Delta);
        }
        public void Backward_LastLayer(MatrixLayer prevousLayer, Matrix target)
        {
            Output_Error = target.Add(Output.Scale(-1.0));
            Output_Delta = Matrix.Multiply(Output_Error, prevousLayer.Output.Relu_deriv());
            W_Delta = Matrix.Dot(prevousLayer.Output.T(), Output_Delta);
        }
        public void Backward_FirstLayer(MatrixLayer nextLayer)
        {
            Output_Error = nextLayer.Backward_PrevousLayerOutputError();
            Output_Delta = Matrix.Multiply(Output_Error, Output.Relu_deriv());
            W_Delta = Matrix.Dot(Input.T(), Output_Delta);

        }

        public Matrix Backward_PrevousLayerOutputError()
        {
            var prevousLayerOutputError = Matrix.Dot( Output_Delta, W.T() ).Scale( -1.0 );
            return prevousLayerOutputError;
        }


        public void UpdateW(double learningRate)
        {
            var scaleFactor = learningRate / (LastSampleLenght * Input_Size * 1.0);
            W = W.Add(W_Delta.Scale(scaleFactor));
        }

    }
}
