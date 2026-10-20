namespace CatologService.Api.Core.Domain
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }
        public int CatalogTypeId { get; set; }
        public int AvailableStock { get; set; }
        public bool OnReorder { get; set; }
        public CatalogType CatalogType { get; set; }
        public int CatalogBrandId { get; set; }
        public CatalogBrand CatalogBrand { get; set; }

        public CatalogItem(int ıd, string name, string description, decimal price, string pictureFileName, string pictureUri, int catalogTypeId, int availableStock, bool onReorder, int catalogBrandId)
        {
            Id = ıd;
            Name = name;
            Description = description;
            Price = price;
            PictureFileName = pictureFileName;
            PictureUri = pictureUri;
            CatalogTypeId = catalogTypeId;
            AvailableStock = availableStock;
            OnReorder = onReorder;
            CatalogBrandId = catalogBrandId;
        }

        public CatalogItem()
        {
        }
    }
}
