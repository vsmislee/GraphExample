using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SciChart.Charting.Model.DataSeries;
using System.IO;

namespace GraphExample
{
    internal class DataReader
    {
        public XyDataSeries<double, double> ReadXYFromCsv(string path)
        {
            XyDataSeries<double, double> data = new XyDataSeries<double, double>();
            

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using var streamReader = File.OpenText(path);
            using var csvReader = new CsvReader(streamReader, configuration);

            while (csvReader.Read())
            {
                data.XValues.Add(csvReader.GetField<double>(0));
                data.YValues.Add(csvReader.GetField<double>(1));
            }

            return data;

        }
    }
}
