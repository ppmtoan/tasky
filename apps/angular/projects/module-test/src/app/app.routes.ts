import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'tenant-signup',
    loadComponent: () => import('./tenant-signup/tenant-signup.component').then(c => c.TenantSignupComponent),
  },
  {
    path: 'tenant-admin',
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./tenant-admin/tenant-dashboard/tenant-dashboard.component').then(c => c.TenantDashboardComponent),
      },
    ]
  },
  {
    path: 'tasks',
    loadComponent: () => import('./tasks/task-list/task-list.component').then(c => c.TaskListComponent),
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
  // Custom SaaS UI routes - more specific paths must come before ABP's generic 'saas' route
  {
    path: 'saas-management',  // Changed from 'saas' to avoid conflict with ABP's built-in module
    children: [
      {
        path: 'editions',
        loadComponent: () => import('./host-admin/editions/editions.component').then(c => c.EditionsComponent),
      },
      {
        path: 'subscriptions',
        loadComponent: () => import('./host-admin/subscriptions/subscriptions.component').then(c => c.SubscriptionsComponent),
      },
      {
        path: 'invoices',
        loadComponent: () => import('./host-admin/invoices/invoices.component').then(c => c.InvoicesComponent),
      },
    ]
  },
  // ABP's built-in SaaS module (tenant management)
  {
    path: 'saas',
    loadChildren: () => import('@volo/abp.ng.saas').then(c => c.createRoutes()),
  },
  // {
  //   path: 'product-service',
  //   loadChildren: () => import('product-service').then(c => c.provideProductService()),
  // },
];
