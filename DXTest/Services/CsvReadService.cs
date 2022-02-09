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
                        List<Type> types = new List<Type>(new Type[colFields.Count]);
                        var parser = new ParserService();
                        while (!csvReader.EndOfData)
                        {
                            token.ThrowIfCancellationRequested();
                            var fields = csvReader.ReadFields().ToList();
                            if (isLastElementEmpty)
                                fields.RemoveAt(fields.Count - 1);
                            rowFields.Add(fields);

                            for (int i = 0; i < fields.Count; i++)
                            {
                                var type = types[i];
                                parser.FindFieldType(ref type, fields[i]);
                                types[i] = type;
                            }
                        }
                        for (int i = 0; i < colFields.Count; i++)
                        {
                            DataColumn dataColumn = new DataColumn(colFields[i], types[i]);
                            dataColumn.AllowDBNull = true;
                            csvData.Columns.Add(dataColumn);
                        }
                        foreach (var t in rowFields)
                            csvData.Rows.Add(t.ToArray());
                    }
                }
                return csvData;
            }, token);
        }
    }
}
