using SciChart.Charting.Model.DataSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.IntegralTransforms;

namespace GraphExample
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XyDataSeries<double,double> lineData = new XyDataSeries<double, double>();

        private double[] signalYValues;
        private double[] spectrumYValues;

        private DataReader dataReader = new DataReader();
        private const string pathToData = @"data\data.csv";

        //signal formula: Math.Sin(i * 0.1) + Math.Sin(i * 0.3) + Math.Sin(i * 0.6)

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            LineSeries.DataSeries = lineData;
            XyDataSeries<double, double> values = TakeValuesFromSource();

            UpdateDataSeries(values.YValues.ToArray());
        }


        private void OnButtonSpectrumClick(object sender, RoutedEventArgs e)
        {
            ButtonSpectrum.IsEnabled = false;
            ButtonSignal.IsEnabled = true;

            if (lineData.Count == 0)
                return;

            signalYValues = lineData.YValues.ToArray();

            double[] data;

            if (spectrumYValues == null)
            {
                data = Fft(lineData.YValues.ToArray());
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = Math.Abs(data[i]);
                }
            }
            else
            {
                data = new double[spectrumYValues.Length];
                Array.Copy(spectrumYValues, data, spectrumYValues.Length);
            }

            UpdateDataSeries(data);
        }


        private void OnButtonSignalClick(object sender, RoutedEventArgs e)
        {
            ButtonSpectrum.IsEnabled = true;
            ButtonSignal.IsEnabled = false;

            spectrumYValues = lineData.YValues.ToArray();

            if (signalYValues != null)
            {
                UpdateDataSeries(signalYValues);
            }
        }


        private void UpdateDataSeries(double[] newData)
        {
            using (lineData.SuspendUpdates())
            {
                lineData.Clear();
                for (int i = 0; i < newData.Length; i++)
                {
                    lineData.Append(i, newData[i]);
                }
            }

        }

        private XyDataSeries<double, double> TakeValuesFromSource()
        { 
            XyDataSeries<double, double> data = new XyDataSeries<double, double>();

            try
            {
                data = dataReader.ReadXYFromCsv(pathToData);
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }

            return data;
        }

        private double[] Fft(double[] values)
        {
            int length;
            if(values.Length % 2 == 0)
            {
                length = values.Length + 2;
            }
            else
            {
                length = values.Length + 1;
            }

            double[] data = new double[length];
            Array.Copy(values, data, values.Length);

            Fourier.ForwardReal(data, values.Length);

            return data;

        }
    }
}
