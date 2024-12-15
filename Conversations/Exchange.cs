namespace jukumu.Conversations
{
    public class Exchange
    {
        public string Question { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public HashSet<string> Options { get; set; } = new HashSet<string>();


        public override bool Equals(object? obj)
        {
            if (obj is Exchange other)
            {
                return Question == other.Question &&
                    Key == other.Key &&
                    Answer == other.Answer &&
                    Options.SetEquals(other.Options); // Ensures both sets contain the same elements
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Combine(Question, Key, Answer);

            // Incorporate Options into the hash
            foreach (var option in Options)
            {
                hash = HashCode.Combine(hash, option.GetHashCode());
            }

            return hash;
        }
    }
}