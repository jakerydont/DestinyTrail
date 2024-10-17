using System;

namespace DestinyTrail.Engine
{
    public interface IGameData<T> 
    {
        T this[int index] { get; set; }
        void Add(T item);
        void Remove(T item);
        int Count { get; }
        T[] ToArray();

        T First();

        T MinBy<TKey>(Func<T, TKey> keySelector);
    }
}
