using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DXTest.Services
{
    public class CsvReadService
    {
        private readonly string _path;
        private readonly Action<int> _onValueChanged;
        public CsvReadService(string path, Action<int> onValueChanged)
        {
            _path = path;
            _onValueChanged = onValueChanged;
        }
        public async Task<DataTable> ReadCsvAsync(CancellationToken token)
        {
            return await Task.Run(() =>
            {
                DataTable csvData = new DataTable();
                using (FileStream fs = new FileStream(_path, FileMode.Open))
                {
                    var reader = new Reader(fs, _onValueChanged);
                    using (TextFieldParser csvReader = new TextFieldParser(reader))
                    {
                        csvReader.SetDelimiters(new string[] { ";" });
                        csvReader.HasFieldsEnclosedInQuotes = false;
                        string[] colFields = csvReader.ReadFields();
                        List<string[]> rowFields = new List<string[]>();
                        List<Type> types = new List<Type>();
                        while (!csvReader.EndOfData)
                        {
                            token.ThrowIfCancellationRequested();
                            rowFields.Add(csvReader.ReadFields());
                        }

                        for (int i = 0; i < rowFields[0].Length; i++)
                        {
                            List<string> col = new List<string>();
                            for (int j = 0; j < rowFields.Count; j++)
                            {
                                col.Add(rowFields[j][i]);

                            }
                            types.Add(GetColumnType(col));
                        }
                        for (int i = 0; i < colFields.Length; i++)
                        {
                            DataColumn datecolumn = new DataColumn(colFields[i], types[i]);
                            datecolumn.AllowDBNull = true;
                            csvData.Columns.Add(datecolumn);
                        }
                        foreach (var str in rowFields)
                        {
                            csvData.Rows.Add(str);
                        }
                        while (!csvReader.EndOfData)
                        {
                            token.ThrowIfCancellationRequested();
                            List<string> fieldData = csvReader.ReadFields().ToList();
                            for (int i = 0; i < fieldData.Count; i++)
                            {
                                if (fieldData[i] == "")
                                {
                                    fieldData[i] = null;
                                }
                            }
                            csvData.Rows.Add(fieldData.ToArray());
                        }
                    }
                }
                return csvData;
            }, token);
        }

        private Type GetColumnType(List<string> columns)
        {
            bool areAllDateTime = columns.All(x => DateTime.TryParse(x, out _));
            if (areAllDateTime)
                return typeof(DateTime);
            bool areAllInt = columns.All(x => int.TryParse(x, out _));
            if (areAllInt)
                return typeof(int);
            bool areAllDouble = columns.All(x => double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out _));
            if (areAllDouble)
                return typeof(double);
            return typeof(string);
        }
    }
}
