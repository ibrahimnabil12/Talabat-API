﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications.Products_Specs
{
    public class ProductSpecsParams
    {

        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value.ToLower(); }
        }

        private int pageSize = 5;

        public int PageSize
        {
            get { return pageSize; }
            set {
                if (value > 10)
                    pageSize = 10;
                else
                    pageSize = value;
            }
        }

        public int PageIndex { get; set; } = 1;

        public string? Sort { get; set; }

        public int? BrandId { get; set; }

        public int? TypeId { get; set;}
    }
}
