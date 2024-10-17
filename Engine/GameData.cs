using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DestinyTrail.Engine
{
    public abstract class GameData<T> : IGameData<T>, IEnumerable<T>
    {
        protected List<T> _items = new List<T>();

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public void Add(T item) => _items.Add(item);

        public void Remove(T item) => _items.Remove(item);

        public int Count => _items.Count;

        public bool IsReadOnly => false; // Indicates that the collection is not read-only

        public int IndexOf(T item) => _items.IndexOf(item);

        public void Insert(int index, T item) => _items.Insert(index, item);

        public void RemoveAt(int index) => _items.RemoveAt(index);

        public T First() => _items.First();

        public T MinBy<TKey>(Func<T, TKey> keySelector) => _items.OrderBy(keySelector).First();

        public T MaxBy<TKey>(Func<T, TKey> keySelector) => _items.OrderByDescending(keySelector).First();

        public void Clear() => _items.Clear();

        public bool Contains(T item) => _items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public T FirstOrDefault() => _items.FirstOrDefault();

        public T LastOrDefault() => _items.LastOrDefault();

        public bool Any() => _items.Any();

        public void Reverse() => _items.Reverse();

        public void Sort(Comparison<T> comparison) => _items.Sort(comparison);

        public void Sort(IComparer<T> comparer) => _items.Sort(comparer);

        public IEnumerable<T> AsQueryable() => _items.AsQueryable();

        public T Find(Predicate<T> match) => _items.Find(match);

        public List<T> FindAll(Predicate<T> match) => _items.FindAll(match);

        public int RemoveAll(Predicate<T> match) => _items.RemoveAll(match);

        public void ForEach(Action<T> action) => _items.ForEach(action);

        public void AddRange(IEnumerable<T> collection) => _items.AddRange(collection);

        public List<T> GetRange(int index, int count) => _items.GetRange(index, count);

        public void TrimExcess() => _items.TrimExcess();

        public T[] ToArray() => _items.ToArray();

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        

        public static implicit operator T[](GameData<T> data) => data?.ToArray();

    }
}
