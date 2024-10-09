

using Core.Entities.Consts;
using System.Security.AccessControl;

namespace Core.Specifications
{
    public class CarSpecParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;

        private int _pageSize { get; set; } = 6;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;


        }

        public int? makeId { get; set; }
        public int? modelId { get; set; }
        public ModelVariant? modelVariant { get; set; }
        public CarCondition? carCondition { get; set; }
        public int? priceFrom { get; set; }
        public int? priceTo { get; set; }
        public FuelType? fuel { get; set; }
        public int? yearFrom { get; set; }
        public int? yearTo { get; set; }
        public int? mileageFrom { get; set; }
        public int? mileageTo { get; set; }
        public string? color { get; set; }
        public string sortBy { get; set; } = "model";
        public string sortDirection { get; set; } = "asc";

        private string? _searchValue;

        public string SearchValue 
        { 
            
            get => (_searchValue is null) ? string.Empty : _searchValue;

            set => _searchValue = value; 
        }



    }
}
