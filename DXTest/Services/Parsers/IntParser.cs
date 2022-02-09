using System;
using DXTest.Interfaces;

namespace DXTest.Services.Parsers
{
    public class IntParser : IParser
    {
        public Type FieldType { get; } = typeof(int);
        public bool TryParse(string field)
        {
            return int.TryParse(field, out _);
        }
    }
}
