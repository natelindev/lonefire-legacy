using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace lonefire.Data
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageIndexCap { get; private set; }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndexCap = Constants.PageIndexCap;
            this.AddRange(items);
        }

        public PaginatedList()
        {
        }

        public List<int> getIndexes()
        {
            List<int> indexes = new List<int>();
            indexes.Add(PageIndex);
            int leftRef = PageIndex, rightRef = PageIndex;
            while (PageIndexCap > 1 && (leftRef > 1 || rightRef < TotalPages))
            {
                if(leftRef > 1)
                {
                    --leftRef;
                    --PageIndexCap;
                    indexes.Add(leftRef);
                }
                if(PageIndexCap > 1 && rightRef < TotalPages)
                {
                    ++rightRef;
                    --PageIndexCap;
                    indexes.Add(rightRef);
                }
            }
            indexes.Sort((a, b) => a - b);
            //Left dots
            if(indexes[0] > 1)
            {
                indexes.Insert(0,-1);
            }
            //Right dots
            if(indexes.Last() < TotalPages)
            {
                indexes.Add(-1);
            }
            return indexes;
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
