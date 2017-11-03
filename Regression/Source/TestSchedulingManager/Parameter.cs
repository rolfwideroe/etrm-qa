
namespace TestSchedulingManager
{
    internal class Parameter
    {
        public Parameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Value { get; private set; }
        public string Name { get; private set; }
    }

    
}
