using Microsoft.Extensions.Localization;
using ModuleTest.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ModuleTest.AuthServer;

[Dependency(ReplaceServices = true)]
public class ModuleTestBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ModuleTestResource> _localizer;

    public ModuleTestBrandingProvider(IStringLocalizer<ModuleTestResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
