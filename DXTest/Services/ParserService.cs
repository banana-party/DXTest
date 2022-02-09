using System;
using System.Collections.Generic;
using DXTest.Interfaces;
using DXTest.Services.Parsers;

namespace DXTest.Services
{
    public class ParserService
    {
        private readonly List<IParser> _parsers;
        public ParserService()
        {
            _parsers = new List<IParser>()
            {
                new DoubleParser(),
                new DateParser(),
                new IntParser(),
            };
        }

        public Type FindFieldType(Type previousType, string field)
        {
            if (previousType == typeof(string))
                return typeof(string);

            foreach (var parser in _parsers)
                if (parser.TryParse(field))
                {
                    return parser.FieldType;
                }
            return typeof(string);
        }



    }
}
