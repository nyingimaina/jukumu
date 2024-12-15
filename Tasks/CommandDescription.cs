using System.Collections.ObjectModel;

namespace jukumu.Tasks
{
    public class CommandDescription
    {
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string App { get; set; } = string.Empty;


        public override bool Equals(object? obj)
        {
            if (obj is CommandDescription other)
            {
                return Key == other.Key &&
                    Description == other.Description &&
                    App == other.App;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Description, App);
        }
    }

}