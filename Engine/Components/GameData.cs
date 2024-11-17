using System;
using System.Collections.Generic;
using System.Linq;

namespace DestinyTrail.Engine
{
    public abstract class GameData<T> : List<T>
    {
        public static implicit operator T[](GameData<T> data) => data?.ToArray() ?? throw new NullReferenceException();

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

        public T GetByPropertyValue(string propertyName, string value)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found");

            return this.FirstOrDefault(item =>
                string.Equals(
                    propertyInfo.GetValue(item, null)?.ToString(),
                    value,
                    StringComparison.OrdinalIgnoreCase)) 
                ?? throw new NullReferenceException();
        }


        public T FirstOrDefaultSafe()
        {
            return this.FirstOrDefault() ?? throw new NullReferenceException();
        }

        public T LastOrDefaultSafe()
        {
            return this.LastOrDefault() ?? throw new NullReferenceException();
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
    
    }
}
