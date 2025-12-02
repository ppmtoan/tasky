import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { provideAbpCore, withOptions } from '@abp/ng.core';
import { provideAbpOAuth } from '@abp/ng.oauth';
import { provideSettingManagementConfig } from '@abp/ng.setting-management/config';
import { provideFeatureManagementConfig } from '@abp/ng.feature-management';
import { registerLocale } from '@volo/abp.ng.language-management/locale';
import { provideAccountAdminConfig } from '@volo/abp.ng.account/admin/config';
import { provideAccountPublicConfig } from '@volo/abp.ng.account/public/config';
import { provideIdentityConfig } from '@volo/abp.ng.identity/config';
import { provideLanguageManagementConfig } from '@volo/abp.ng.language-management/config';
import { provideSaasConfig } from '@volo/abp.ng.saas/config';
import { provideAuditLoggingConfig } from '@volo/abp.ng.audit-logging/config';
import { provideOpeniddictproConfig } from '@volo/abp.ng.openiddictpro/config';
import { provideTextTemplateManagementConfig } from '@volo/abp.ng.text-template-management/config';
import { provideLogo, withEnvironmentOptions } from '@volo/ngx-lepton-x.core';
import { provideCommercialUiConfig } from '@volo/abp.commercial.ng.ui/config';
import { provideProductServiceConfig } from 'product-service/config';
import { environment } from '../environments/environment';
import { APP_ROUTES } from './app.routes';
import { APP_ROUTE_PROVIDER } from './route.provider';
import { ThemeBasicModule, provideThemeBasicConfig } from '@abp/ng.theme.basic';
import { ThemeSharedModule, withValidationBluePrint, provideAbpThemeShared } from '@abp/ng.theme.shared';
export const appConfig: ApplicationConfig = {
    providers: [
    importProvidersFrom([
        BrowserModule,
    ]),
    provideAnimations(),
    provideRouter(APP_ROUTES),
    APP_ROUTE_PROVIDER,
    provideAbpCore(withOptions({
        environment,
        registerLocaleFn: registerLocale(),
    })),
    provideAbpOAuth(),
    provideAccountAdminConfig(),
    provideAccountPublicConfig(),
    provideIdentityConfig(),
    provideLanguageManagementConfig(),
    provideSaasConfig(),
    provideAuditLoggingConfig(),
    provideOpeniddictproConfig(),
    provideTextTemplateManagementConfig(),
    provideSettingManagementConfig(),
    provideCommercialUiConfig(),
    provideFeatureManagementConfig(),
    provideLogo(withEnvironmentOptions(environment)),
    provideProductServiceConfig(), importProvidersFrom(ThemeBasicModule, ThemeSharedModule), provideThemeBasicConfig(), provideAbpThemeShared(withValidationBluePrint({
        wrongPassword: 'Please choose 1q2w3E*'
    }))
],
};
