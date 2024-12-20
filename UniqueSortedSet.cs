using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SharePortfolio
{
    public class UniqueSortedSet<T>
        : IEnumerable<T>
        , IReadOnlySet<T>
        , ISet<T>
        where T : IComparable<T>
    {
        private readonly SortedSet<T> sortedSet;

        public UniqueSortedSet()
        {
            this.sortedSet = new SortedSet<T>();
        }

        public UniqueSortedSet(IEnumerable<T> values)
            : this()
        {
            this.AddRange(values);
        }

        public int Count => this.sortedSet.Count;

        public bool IsReadOnly => ((ICollection<T>)sortedSet).IsReadOnly;

        public void Add(T item)
        {
            if (Contains(item))
            {
                throw new ArgumentException("Item already exists in the collection.");
            }

            this.sortedSet.Add(item);
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items) this.Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)sortedSet).Clear();
        }

        public bool Contains(T item)
        {
            return this.sortedSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)sortedSet).CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>)sortedSet).ExceptWith(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.sortedSet.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ((ISet<T>)sortedSet).IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return this.sortedSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return this.sortedSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return this.sortedSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return this.sortedSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return this.sortedSet.Overlaps(other);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)sortedSet).Remove(item);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return this.sortedSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>)sortedSet).SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ((ISet<T>)sortedSet).UnionWith(other);
        }

        bool ISet<T>.Add(T item)
        {
            return ((ISet<T>)sortedSet).Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}