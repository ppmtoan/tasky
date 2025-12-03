import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44322/',
  redirectUri: baseUrl,
  clientId: 'Angular',
  responseType: 'code',
  scope:
    'offline_access openid profile email phone AccountService IdentityService AdministrationService SaasService ProductService',
  requireHttps: true,
  impersonation: {
    userImpersonation: true,
    tenantImpersonation: true,
  },
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'ModuleTest',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44325',
      rootNamespace: 'ModuleTest',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
    SaasService: {
      url: 'https://localhost:44325',
      rootNamespace: 'ModuleTest.SaasService',
    },
    ProductService: {
      url: 'https://localhost:44325',
      rootNamespace: 'ModuleTest.ProductService',
    },
  },
} as Environment;
