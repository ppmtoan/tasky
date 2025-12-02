using ModuleTest.ProductService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.ProductService;

public abstract class ProductServiceController : AbpController
{
    protected ProductServiceController()
    {
        LocalizationResource = typeof(ProductServiceResource);
    }
}
