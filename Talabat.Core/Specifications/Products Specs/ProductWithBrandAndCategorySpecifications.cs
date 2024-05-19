using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Products_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(ProductSpecsParams specsParams)
            : base(p =>
            (string.IsNullOrEmpty(specsParams.Search)||p.Name.ToLower().Contains(specsParams.Search))&&
            (!specsParams.BrandId.HasValue || p.ProductBrandId ==specsParams.BrandId.Value)&&
            (!specsParams.TypeId.HasValue || p.ProductTypeId == specsParams.TypeId.Value)

            )
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);

            if (!string.IsNullOrEmpty(specsParams.Sort))
            {
                switch (specsParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                AddOrderBy(p => p.Name);
            }
            ApplyPagination((specsParams.PageIndex - 1) * specsParams.PageSize, specsParams.PageSize);
        }
        public ProductWithBrandAndCategorySpecifications(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);
            //Criteria = p => p.Id == id;
        }
    }
}
