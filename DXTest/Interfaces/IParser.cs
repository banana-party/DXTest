using System;

namespace DXTest.Interfaces
{
    public interface IParser
    {
        Type FieldType { get; }
        bool TryParse(string field);
    }
}
