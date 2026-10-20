using CatologService.Api.Core.Domain;
using Microsoft.Data.SqlClient;
using Polly;
using System.Globalization;

namespace CatologService.Api.Infrastructure.Context
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
        {
            var policy = Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exeption, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exeption, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retry}");
                }
                );
            var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFile");
            var picturePath = "Pics";
            await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath, logger));
        }

        private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath, ILogger<CatalogContextSeed> logger)
        {
            if (!context.CatalogBrands.Any())
            {
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));
                await context.SaveChangesAsync();
            }

            if (!context.CatalogTypes.Any())
            {
                await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));
                await context.SaveChangesAsync();
            }

            if (!context.CatalogItems.Any())
            {
                await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath,context));
                await context.SaveChangesAsync();
            }
        }

        private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string setupDirPath)
        {
            IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
            {
                return new List<CatalogBrand>()
                {
                    new CatalogBrand() {Brand="Azure"},
                    new CatalogBrand() {Brand=".NET"},
                    new CatalogBrand() {Brand="Visual Studio"},
                    new CatalogBrand() {Brand="SQL Server"},
                    new CatalogBrand() {Brand="Other"},
                };
            }
            string fileName = Path.Combine(setupDirPath, "BrandsTextFile,txt");
            if (!File.Exists(fileName))
            {
                return GetPreconfiguredCatalogBrands();
            }
            var fileContent = File.ReadAllLines(fileName);
            var list = fileContent.Select(i => new CatalogBrand()
            {
                Brand = i.Trim('"')
            }).Where(i => i != null);
            return list ?? GetPreconfiguredCatalogBrands();

        }
        private IEnumerable<CatalogType> GetCatalogTypesFromFile(string setupDirPath)
        {
            IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
            {
                return new List<CatalogType>()
                {
                    new CatalogType() {Type="Mug"},
                    new CatalogType() {Type="T-Shirt"},
                    new CatalogType() {Type="Sheet"},
                    new CatalogType() {Type="USB memory stick"},
                };
            }

            string fileName = Path.Combine(setupDirPath, "CatalogTypes,txt");
            if (!File.Exists(fileName))
            {
                return GetPreconfiguredCatalogTypes();
            }

            var fileContent = File.ReadAllLines(fileName);
            var list = fileContent.Select(i => new CatalogType()
            {
                Type = i.Trim('"')
            }).Where(i => i != null);
            return list ?? GetPreconfiguredCatalogTypes();

        }
        private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
        {
            IEnumerable<CatalogItem> GetPreconfiguredItems()
            {
                return new List<CatalogItem>()
                {
                    new CatalogItem(1, "Azure Mug", "Azure branded mug", 10.00m, "azure-mug.png", "https://example.com/azure-mug.png", 1, 150, false, 2),
            new CatalogItem(2, "SQL Server T-shirt", "High-quality SQL Server themed t-shirt", 20.00m, "sqlserver-tshirt.png", "https://example.com/sqlserver-tshirt.png", 3, 75, true, 1),
            new CatalogItem(3, ".NET Sheet", ".NET branded sheet", 23.50m, "dotnet-sheet.png", "https://example.com/dotnet-sheet.png", 4, 60, true, 3),
            new CatalogItem(4, "Visual Studio USB Memory Stick", "Visual Studio branded USB memory stick", 33.00m, "visualstudio-usb.png", "https://example.com/visualstudio-usb.png", 2, 20, false, 4),
            new CatalogItem(5, "Generic T-shirt", "High-quality generic t-shirt", 15.00m, "generic-tshirt.png", "https://example.com/generic-tshirt.png", 1, 200, true, 3),
            new CatalogItem(6, ".NET Mug", ".NET branded mug", 8.50m, "dotnet-mug.png", "https://example.com/dotnet-mug.png", 4, 200, true, 2)

                };
            }

            string fileName = Path.Combine(contentPath, "CatalogItems,txt");
            if (!File.Exists(fileName))
            {
                return GetPreconfiguredItems();
            }
            var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
            var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);

            var fileContent = File.ReadAllLines(fileName)
                .Skip(1)
                .Select(i => i.Split(','))
                .Select(i => new CatalogItem()
                {
                    CatalogTypeId = catalogBrandIdLookup[i[0]],
                    CatalogBrandId = catalogBrandIdLookup[i[1]],
                    Description = i[2].Trim('"').Trim(),
                    Name = i[3].Trim('"').Trim(),
                    Price = Decimal.Parse(i[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                    PictureFileName = i[5].Trim('"').Trim(),
                    AvailableStock = string.IsNullOrEmpty(i[6]) ? 0 : int.Parse(i[6]),
                    OnReorder = Convert.ToBoolean(i[7])
                });
            return fileContent;
        }
    }
}
