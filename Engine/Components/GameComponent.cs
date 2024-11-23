namespace DestinyTrail.Engine
{
    public abstract class GameComponent
    {
        public virtual string Name { get; set; } = ""; // The name of the status

        public override string ToString() => Name;

        // FIXME: This method shouldn't be necessary. Tokenized strings think we're trying to call MemoryExtensions.ToLower() if we don't spell this out (even without override keyword). I don't know why.
        public string ToLower() => Name.ToLower();

        public static implicit operator string(GameComponent gc) => gc.Name;

        public override int GetHashCode() => HashCode.Combine(Name);
        
    }
}
