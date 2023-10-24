using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.Params
{
    public record PagedParams
    {
        private const int MinPageSize = 5;
        private const int MaxPageSize = 50;

        private int _pageNumber = 1;
        private int _pageSize = 10;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (value < 1)
                {
                    _pageNumber = 1;
                }
                else
                {
                    _pageNumber = value;
                }
            }
        }
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value > MaxPageSize)
                {
                    _pageSize = MaxPageSize;
                }
                else if (value < MinPageSize)
                {
                    _pageSize = MinPageSize;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }
    }
}