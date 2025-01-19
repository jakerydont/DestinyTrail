using System;
using System.Collections.Generic;
using System.Linq;

namespace DestinyTrail.Engine
{
    public abstract class GameData<T> : List<T>, IGameData<T> where T : GameComponent, new()
    {

        private IDisplay Display { get; }

        public static T Default { get; } = new();

        public static implicit operator T[](GameData<T> data) => data?.ToArray() 
            ?? throw new NullReferenceException($"{typeof(T).Name} data is null");

        public GameData() : this(new Display()) { }

        public GameData(IDisplay display) : base()
        {
            Display = display;
        }

        protected List<T> _items
        {
            get => this; // Return the current list instance
            set
            {
                this.Clear();
                this.AddRange(value);
            }
        }



        public T GetByName(string value)
        {
            return GetByPropertyValue("Name", value);
        }

        public bool TryGetByName(string name, out T outItem)
        {
            outItem = TryGetByPropertyValue("Name", name, out T item) ? item : Default;
            return outItem != Default;
        }

        public bool TryGetByPropertyValue(string propertyName, string value, out T item)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                Display.WriteError($"Property '{propertyName}' not found");
                item = Default;
                return false;
            }
            item = this.FirstOrDefault(item =>
                string.Equals(
                    propertyInfo.GetValue(item, null)?.ToString(),
                    value,
                    StringComparison.OrdinalIgnoreCase))!;

            return item != null;
        }

        public T GetByPropertyValue(string propertyName, string value)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found");

            return this.FirstOrDefault(item =>
                string.Equals(
                    propertyInfo.GetValue(item, null)?.ToString(),
                    value,
                    StringComparison.OrdinalIgnoreCase))!;
             
        }


        public T FirstOrDefaultSafe()
        {
            return this.FirstOrDefault() ?? Default;
        }

        public T LastOrDefaultSafe()
        {
            return this.LastOrDefault() ?? Default;
        }

        public T MinBy<TKey>(Func<T, TKey> keySelector)
        {
            return this.OrderBy(keySelector).First();
        }

        public T MaxBy<TKey>(Func<T, TKey> keySelector)
        {
            return this.OrderByDescending(keySelector).First();
        }

        public new T[] ToArray()
        {
            return base.ToArray();
        }

        void IGameData<T>.Remove(T item)
        {
            Remove(item);
        }

        T IGameData<T>.First()
        {
            return this.First();
        }
    }
}
