using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StockApp.BaseClasses
{
    public static class ExtensionMethods
    {

        public static void Sort<T>(this ObservableCollection<T> collection, bool asc)
            where T : IComparable<T>, IEquatable<T>
        {
            List<T> sorted = asc ? collection.OrderBy(x => x).ToList() : collection.OrderByDescending(x => x).ToList();

            int ptr = 0;
            while (ptr < sorted.Count - 1)
            {
                if (!collection[ptr].Equals(sorted[ptr]))
                {
                    int idx = Search(collection, ptr + 1, sorted[ptr]);
                    collection.Move(idx, ptr);
                }

                ptr++;
            }
        }

        public static int Search<T>(ObservableCollection<T> collection, int startIndex, T other)
        {
            for (int i = startIndex; i < collection.Count; i++)
            {
                if (other.Equals(collection[i]))
                    return i;
            }

            return -1; // decide how to handle error case
        }
    }
}
