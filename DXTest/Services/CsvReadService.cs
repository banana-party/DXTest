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
                        bool isLastElementEmpty = false;
                        csvReader.SetDelimiters(new string[] { ";" });
                        csvReader.HasFieldsEnclosedInQuotes = false;
                        var colFields = csvReader.ReadFields().ToList();
                        if (string.IsNullOrEmpty(colFields.Last()))
                        {
                            isLastElementEmpty = true;
                            colFields.RemoveAt(colFields.Count - 1);
                        }
                        List<List<string>> rowFields = new List<List<string>>();
                        List<Type> types = new List<Type>();
                        while (!csvReader.EndOfData)
                        {
                            token.ThrowIfCancellationRequested();
                            rowFields.Add(csvReader.ReadFields().ToList());
                        }
                        for (int i = 0; i < rowFields[0].Count; i++)
                        {
                            token.ThrowIfCancellationRequested();
                            List<string> col = new List<string>();
                            for (int j = 0; j < rowFields.Count; j++)
                                col.Add(rowFields[j][i]);
                            types.Add(GetColumnType(col));
                        }
                        if (isLastElementEmpty)
                            types.RemoveAt(types.Count - 1);
                        for (int i = 0; i < colFields.Count; i++)
                        {
                            token.ThrowIfCancellationRequested();
                            DataColumn datacolumn = new DataColumn(colFields[i], types[i]);
                            datacolumn.AllowDBNull = true;
                            csvData.Columns.Add(datacolumn);
                            datacolumn.DataType = typeof(string);
                        }
                        for (int i = 0; i < rowFields.Count; i++)
                        {
                            if (isLastElementEmpty)
                                rowFields[i].RemoveAt(rowFields[i].Count - 1);
                            token.ThrowIfCancellationRequested();
                            csvData.Rows.Add(rowFields[i].ToArray());
                        }
                        reader.CurrentProgress = 100;
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
