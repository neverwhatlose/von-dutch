namespace von_dutch.Attributes
{
    /// <summary>
    /// Атрибут, используемый для указания имени файла словаря, связанного с свойством.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DictFileAttribute(string fileName) : Attribute
    {
        /// <summary>
        /// Имя файла словаря.
        /// </summary>
        public string FileName { get; } = fileName;
    }
}