using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public T GetByName(string Value)
        {
            return GetByPropertyValue("Name", Value);
        }

        public T GetByPropertyValue(string PropertyName, string Value)
        {
            var propertyInfo = typeof(T).GetProperty(PropertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Property '{PropertyName}' not found");

            return _items.FirstOrDefault(item =>
                propertyInfo.GetValue(item, null)?.ToString() == Value) ?? throw new NullReferenceException();
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public T First()
        {
            return _items.First();
        }

        public T MinBy<TKey>(Func<T, TKey> keySelector)
        {
            return _items.OrderBy(keySelector).First();
        }

        public T MaxBy<TKey>(Func<T, TKey> keySelector)
        {
            return _items.OrderByDescending(keySelector).First();
        }




        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public T FirstOrDefault()
        {
            return _items.FirstOrDefault() ?? throw new NullReferenceException();
        }

        public T LastOrDefault()
        {
            return _items.LastOrDefault() ?? throw new NullReferenceException();
        }

        public bool Any()
        {
            return _items.Any();
        }

        public void Reverse()
        {
            _items.Reverse();
        }

        public void Sort(Comparison<T> comparison)
        {
            _items.Sort(comparison);
        }

        public void Sort(IComparer<T> comparer)
        {
            _items.Sort(comparer);
        }

        public IEnumerable<T> AsQueryable()
        {
            return _items.AsQueryable();
        }

        public T Find(Predicate<T> match)
        {
            return _items.Find(match) ?? throw new NullReferenceException();
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return _items.FindAll(match);
        }

        public int RemoveAll(Predicate<T> match)
        {
            return _items.RemoveAll(match);
        }

        public void ForEach(Action<T> action)
        {
            _items.ForEach(action);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _items.AddRange(collection);
        }

        public List<T> GetRange(int index, int count)
        {
            return _items.GetRange(index, count);
        }

        public void TrimExcess()
        {
            _items.TrimExcess();
        }

        public T[] ToArray()
        {
            return _items.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator T[](GameData<T> data) => data?.ToArray() ?? throw new NullReferenceException();

    }
}
