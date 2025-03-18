namespace von_dutch
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DictFileAttribute(string fileName) : Attribute
    {
        public string FileName { get; } = fileName;
    }
}