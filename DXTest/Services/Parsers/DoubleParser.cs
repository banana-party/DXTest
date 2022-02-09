using System;
using System.Globalization;
using DXTest.Interfaces;

namespace DXTest.Services.Parsers
{
    internal class DoubleParser : IParser
    {
        public Type FieldType { get; } = typeof(double);
        public bool TryParse(string field)
        {
            return double.TryParse(field, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }
    }
}
