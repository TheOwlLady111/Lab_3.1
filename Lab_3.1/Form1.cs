using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab_3._1
{
    public partial class Form1 : Form
    {
        const int MIN = 0;
        const int MAX = 20;
        int _arrayLenght = 100;
        int _complexity = 10000;

        public Form1()
        {
            InitializeComponent();
        }


        public int[] CreateArray(int size)
        {
            Random random = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = random.Next(MIN, MAX);
            }

            return array;
        }

        public void GetThreadsWorkTime(int threadCount, int arrayLeangth, int key, int complexity)
        {
            Stopwatch time = new Stopwatch();
            Barrier barrier = new Barrier(threadCount, b =>
            {
                if (key != -1)
                {
                    time.Stop();
                    Action action = () => { chart1.Series[1].Points.AddXY(threadCount, time.Elapsed.TotalSeconds); };
                    chart1.Invoke(action);
                }
                    
            });

            int[] arrayA = CreateArray(arrayLeangth);
            double[] arrayB = new double[arrayLeangth];
            time.Start();
            for (int j = 0; j < threadCount; j++)
            {
                int x = j;
                int start = arrayA.Length * x / threadCount;
                int end = arrayA.Length * (x + 1) / threadCount;
                Thread thread = new Thread(() =>
                {
                    StartThreadsMathPow(arrayLeangth, arrayA, arrayB, complexity, start, end, ref key, barrier);
                });
                thread.Start();

            }
            Thread.Sleep(100);
        }

        public void GetThreadsWorkTime(int threadCount, int arrayLeangth, int key, int complexity, int num)
        {
            Stopwatch time = new Stopwatch();
            Barrier barrier = new Barrier(threadCount, b =>
            {
                if (key != -1)
                {
                    time.Stop();
                    Console.WriteLine(time.Elapsed.TotalSeconds.ToString());
                    Action action = () => { chart2.Series[num].Points.AddXY(complexity, time.Elapsed.TotalSeconds); };
                    chart2.Invoke(action);
                }
            });

            int[] arrayA = CreateArray(arrayLeangth);
            double[] arrayB = new double[arrayLeangth];
            time.Start();
            for (int j = 0; j < threadCount; j++)
            {
                int x = j;
                int start = arrayA.Length * x / threadCount;
                int end = arrayA.Length * (x + 1) / threadCount;
                Thread thread = new Thread(() =>
                {
                    StartThreadsMathPow(arrayLeangth, arrayA, arrayB, complexity, start, end, ref key, barrier);
                });
                thread.Start();
            }
            Thread.Sleep(100);
        }

        public void MathPow(int start, int end, int[] arrayA, double[] arrayB, int k)
        {
            for (int i = start; i < end; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    arrayB[i] += Math.Pow(arrayA[i], 1.789);
                }
            }
            
        }

        public void StartThreadsMathPow(int arrayLength, int[] arrayA, double[] arrayB, int k, int start, int end, ref int key, Barrier barrier)
        {
            barrier.SignalAndWait();
            Interlocked.Increment(ref key);
            MathPow(start, end, arrayA, arrayB, k);
            barrier.SignalAndWait();
        }

        private void btnGenerate1_Click(object sender, EventArgs e)
        {
            _arrayLenght = Convert.ToInt32(textBox1.Text);
            _complexity = Convert.ToInt32(textBox2.Text);
            int step = Convert.ToInt32(textBox6.Text);
            int amountOfThreads = Convert.ToInt32(textBox5.Text);

            var objChart = chart1.ChartAreas[0];
            objChart.AxisX.IntervalType = DateTimeIntervalType.Number;
            objChart.AxisX.Minimum = 0;
            objChart.AxisX.Maximum = amountOfThreads;

            objChart.AxisY.IntervalType = DateTimeIntervalType.Number;
            objChart.AxisY.Minimum = 0;
            objChart.AxisY.Maximum = 1;

            chart1.Series.Clear();
            chart1.Series.Add("Without threads").Color = Color.DarkBlue;
            chart1.Series.Add("With threads").Color = Color.DarkMagenta;
            chart1.Series[0].ChartType = SeriesChartType.Line;
            chart1.Series[1].ChartType = SeriesChartType.Line;

            int[] a = CreateArray(_arrayLenght);
            double[] b = new double[_arrayLenght];
            Stopwatch sw = new Stopwatch();
            sw.Start();
                MathPow(0, _arrayLenght, a, b, _complexity);
            sw.Stop();
            //Thread.Sleep(10000);
            chart1.Series[0].Points.AddXY(0, sw.Elapsed.TotalSeconds);
            chart1.Series[0].Points.AddXY(30, sw.Elapsed.TotalSeconds);
            for (int j = 2; j <= amountOfThreads; j += step)
            {
                GetThreadsWorkTime(j, _arrayLenght, -1, _complexity);
                Thread.Sleep(1000);
            }
        }

        private void btnGenerate2_Click(object sender, EventArgs e)
        {
            _arrayLenght = Convert.ToInt32(textBox3.Text);
            _complexity = Convert.ToInt32(textBox4.Text);
            int step = Convert.ToInt32(textBox7.Text);
            var objChart = chart2.ChartAreas[0];
            objChart.AxisX.IntervalType = DateTimeIntervalType.Number;
            objChart.AxisX.Minimum = 0;
            objChart.AxisX.Maximum = _complexity;

            objChart.AxisY.IntervalType = DateTimeIntervalType.Number;
            objChart.AxisY.Minimum = 0;
            objChart.AxisY.Maximum = 1;

            chart2.Series.Clear();
            chart2.Series.Add("1 threads").Color = Color.BlueViolet;
            chart2.Series[0].ChartType = SeriesChartType.Line;
            chart2.Series.Add("5 threads").Color = Color.Blue;
            chart2.Series[1].ChartType = SeriesChartType.Line;
            chart2.Series.Add("15 threads").Color = Color.DarkCyan;
            chart2.Series[2].ChartType = SeriesChartType.Line;
            chart2.Series.Add("25 threads").Color = Color.Green;
            chart2.Series[3].ChartType = SeriesChartType.Line;
            chart2.Series.Add("30 threads").Color = Color.Orange;
            chart2.Series[4].ChartType = SeriesChartType.Line;

            for (int j = 0; j <= _complexity; j+=step)
            {
                GetThreadsWorkTime(1, _arrayLenght, -1, j, 0);
                Thread.Sleep(500);
                GetThreadsWorkTime(5, _arrayLenght, -1, j, 1);
                Thread.Sleep(500);
                GetThreadsWorkTime(15, _arrayLenght, -1, j, 2);
                Thread.Sleep(500);
                GetThreadsWorkTime(25, _arrayLenght, -1, j, 3);
                Thread.Sleep(500);
                GetThreadsWorkTime(30, _arrayLenght, -1, j, 4);
                Thread.Sleep(500);
            }
        }
    }
}
