using System;
using System.Linq;
using System.Collections.Generic;

namespace MvcClient
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize, int count)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(source);
        }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious
        {
            get { return PageIndex > 1; }
        }
        public bool HasNext
        {
            get { return PageIndex < TotalPages; }
        }
        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            int count = source.Count<T>();
            var items = source.Skip((pageIndex - 1) * pageSize).Take<T>(pageSize);

            return new PaginatedList<T>(items, pageIndex, pageSize, count);
        }
    }
}