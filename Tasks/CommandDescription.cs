namespace Jukumu.Tasks
{
    public class CommandDescription
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Executable { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is CommandDescription other)
            {
                return Name == other.Name &&
                    Description == other.Description &&
                    Executable == other.Executable;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, Executable);
        }
    }

}