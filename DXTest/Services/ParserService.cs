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

        public void FindFieldType(ref Type type, string field)
        {
            if (type == typeof(string))
                return;

            foreach (var parser in _parsers)
                if (parser.TryParse(field))
                {
                    type = parser.FieldType;
                    return;
                }
            type = typeof(string);
        }



    }
}
