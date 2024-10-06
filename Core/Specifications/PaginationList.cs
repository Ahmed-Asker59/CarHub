using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class PaginationList
    {
        public PaginationList() { }
        public PaginationList(int pageIndex,int pageSize,int count , IReadOnlyList<object> data) {
        PageIndex= pageIndex;
        PageSize= pageSize;
        Count=count;
        Data= data;
         
        }
        public int PageIndex {  get;  set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<object> Data { get; set; }

    }
}
