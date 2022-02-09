using System;
using DXTest.Interfaces;

namespace DXTest.Services.Parsers
{
    public class DateParser : IParser
    {
        public Type FieldType { get; } = typeof(DateTime);
        public bool TryParse(string field)
        {
            return DateTime.TryParse(field, out _);
        }
    }
}
