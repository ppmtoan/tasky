import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@volo/abp.ng.account/public').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@volo/abp.ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'language-management',
    loadChildren: () =>
      import('@volo/abp.ng.language-management').then(c => c.createRoutes()),
  },
  {
    path: 'saas',
    loadChildren: () => import('@volo/abp.ng.saas').then(c => c.createRoutes()),
  },
  {
    path: 'audit-logs',
    loadChildren: () => import('@volo/abp.ng.audit-logging').then(c => c.createRoutes()),
  },
  {
    path: 'openiddict',
    loadChildren: () => import('@volo/abp.ng.openiddictpro').then(c => c.createRoutes()),
  },
  {
    path: 'text-template-management',
    loadChildren: () =>
      import('@volo/abp.ng.text-template-management').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
  // {
  //   path: 'product-service',
  //   loadChildren: () => import('product-service').then(c => c.provideProductService()),
  // },
];
