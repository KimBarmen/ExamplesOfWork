using Newtonsoft.Json;
using System;


namespace Leitner.MatrixComponents
{
    public class Matrix
    {
        [JsonConstructor]
        public Matrix(double[,] value)
        {
            Lenght = value.GetLength(0);
            Height = value.GetLength(1);

            Setup(Lenght, Height, 0);
            Value = value;
        }

        public Matrix(int lx, int ly, double? value = null)
        {
            Lenght = lx;
            Height = ly;
            Setup(lx, ly, value);
        }

        private void Setup(int lx, int ly, double? value = null)
        {
            if ((lx < 1) || (ly < 1))
            {
                var e = new ArgumentOutOfRangeException("Matrix lenght must be positive. Was [x= " + lx + ", y=" + ly + "]");
                throw e;
            }
            Value = new double[lx, ly];
            var random = new Random((int)(DateTime.Now.Ticks << 32));
            for (int i = 0; i < lx; i++)
            {
                for (int j = 0; j < ly; j++)
                {
                    if (value.HasValue)
                    {
                        Value[i, j] = value.Value + 0.0;
                    }
                    else
                    {
                        Value[i, j] = (random.NextDouble() * 2) - 1;
                    }
                }
            }
        }

        public double[,] Value { get; set; }
        public int Lenght { get; }
        public int Height { get; }

        public void Print(int decimals = 4, string prefix ="")
        {
            string str = "";
            for (int i = 0; i < Height; i++)
            {
                str += prefix + " [";
                for (int j = 0; j < Lenght; j++)
                {
                    double v = Value[j, i];
                    var beforeDot = ((int)v + "").PadLeft(4);
                    var afterDot = string.Format("{0:0.000}",v).Split('.')[1].Substring(0, decimals + 1);
                    str += " " + ( v  > 0 ? " " : "-") + beforeDot + "." + afterDot;
                    
                }
                str += " ]\n";
            }
            Console.WriteLine(prefix + Lenght + " x " + Height);
            Console.WriteLine(str);
        }

        public Matrix Cut(int Idx, int batchSize)
        {
            int endIdx = 0;
            int batchEnd = 0;

            try
            {
                endIdx = Math.Min(Idx + batchSize, this.Lenght - 1);
                batchEnd = Math.Max(endIdx - Idx, 1);

                Matrix result = new Matrix(batchEnd, Height);

                for (int x = 0; x < result.Lenght; x++)
                {
                    for (int y = 0; y < result.Height; y++)
                    {
                        result.Value[x, y] = this.Value[Idx + x, y];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;  
            }
        }

        public string GetErrorForPrint()
        {
            return string.Format("{0:0.000}", Mean(abs: true));
        }

        public Matrix Multiply(Matrix m2)
        {
            return Multiply(this, m2);
        }

        public Matrix Dot(Matrix m2)
        {
            return Dot(this, m2);
        }
        public Matrix Scale(double factor)
        {
            return Scale(this, factor);
        }
        public Matrix Add(Matrix m2)
        {
            return Add(this, m2);
        }
        public Matrix Invert()
        {
            return Invert(this);
        }
        public double Mean(bool abs = false)
        {
            return Mean(this, abs: abs);
        }
        public Matrix Relu(double leak = 0.1, double maxValue = 2.0)
        {
            return Relu(this, leak: leak, maxValue: maxValue);
        }
        public Matrix Relu_deriv()
        {
            return Relu_deriv(this);
        }
        public Matrix T()
        {
            return Transpose(this);
        }
        public Matrix Bias(double value = 1)
        {
            return Bias(this, value: value);
        }
        public Matrix Tanh()
        {
            return Tanh(this);
        }
        public Matrix Tanh_deriv()
        {
            return Tanh_deriv(this);
        }
        public static Matrix Dot(Matrix m1, Matrix m2)
        {
            var result = new Matrix(m1.Lenght, m2.Height, 0);
            if (m1.Height != m2.Lenght)
            {
                throw new ArgumentOutOfRangeException("Matrix DOT() got invalid dimentions: " + m1.Height + " != " + m2.Lenght + "( [" + m1.Lenght + "," + m1.Height + "] * [" + m2.Lenght + "," + m2.Height + "] )");
            }
            else
            {
                for (int i = 0; i < m1.Lenght; i++)
                {
                    for (int j = 0; j < m2.Height; j++)
                    {
                        double tmpSum = 0;
                        for (int k = 0; k < m1.Height; k++)
                        {
                            tmpSum += m1.Value[i, k] * m2.Value[k, j];
                        }
                        result.Value[i, j] = tmpSum;
                    }
                }
                return result;
            }
        }

        public static Matrix Scale(Matrix m1, double factor)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = m1.Value[i, j] * factor;

                    //if (!double.IsNormal(result.Value[i, j]) && result.Value[i, j] != 0.0)
                    //{
                    //    throw new InvalidOperationException("Scale() got NaN: " + m1.Value[i, j]);
                    //}
                }
            }
            return result;
        }

        public static Matrix Add(Matrix m1, Matrix m2)
        {
            if ((m1.Height != m2.Height) || m1.Lenght != m2.Lenght)
            {
                throw new ArgumentOutOfRangeException("Matrix ADD()  got invalid dimentions: [" + m1.Lenght + "," + m1.Height + "] + [" + m2.Lenght + "," + m2.Height + "]");
            }
            else
            {
                var result = new Matrix(m1.Lenght, m1.Height);
                for (int i = 0; i < m1.Lenght; i++)
                {
                    for (int j = 0; j < m1.Height; j++)
                    {
                        result.Value[i, j] = m1.Value[i, j] + m2.Value[i, j];
                    }
                }
                return result;
            }
        }

        public static Matrix Invert(Matrix m1)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = 1.0 / m1.Value[i, j];

                    //if (!double.IsNormal(result.Value[i, j]) && result.Value[i, j] != 0.0)
                    //{
                    //    throw new InvalidOperationException("Inverse() got NaN: " + m1.Value[i, j]);
                    //}
                }
            }
            return result;
        }

        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = m1.Value[i, j] * m2.Value[i, j];
                }
            }
            return result;
        }

        public static Matrix Tanh(Matrix m1)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = Math.Tanh(m1.Value[i, j]);
                }
            }
            return result;
        }

        public static Matrix Tanh_deriv(Matrix m1)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = 1 - (Math.Tanh(m1.Value[i, j]) * Math.Tanh(m1.Value[i, j]));
                }
            }
            return result;
        }

        public static Matrix Relu(Matrix m1, double leak = 0.1, double maxValue = 5.0)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = Math.Max(
                        Math.Min(
                            Math.Max(
                                m1.Value[i, j],
                                m1.Value[i, j] * leak),
                            maxValue),
                        (-1.0 * maxValue));
                }
            }
            return result;
        }

        public static Matrix Relu_deriv(Matrix m1, double leak = 0.1)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = m1.Value[i, j] > 0 ? 1 : leak;
                }
            }
            return result;
        }

        public static double Mean(Matrix m1, bool abs = false)
        {
            var result = 0.0;
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result += abs ? Math.Abs(m1.Value[i, j]) : m1.Value[i, j];
                }
            }
            return result / (m1.Lenght * m1.Height);
        }

        public static Matrix Transpose(Matrix m1)
        {
            var result = new Matrix(m1.Height, m1.Lenght);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[j, i] = m1.Value[i, j];
                }
            }
            return result;
        }

        public static Matrix Bias(Matrix m1, double value = 1)
        {
            var result = new Matrix(m1.Lenght, m1.Height);
            for (int i = 0; i < m1.Lenght; i++)
            {
                for (int j = 0; j < m1.Height; j++)
                {
                    result.Value[i, j] = m1.Value[i, j] + value;
                }
            }
            return result;
        }
    }
}
