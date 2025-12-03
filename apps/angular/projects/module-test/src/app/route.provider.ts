import { eLayoutType, RoutesService } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/',
      name: '::Menu:Home',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/tenant-signup',
      name: 'Sign Up',
      iconClass: 'fas fa-user-plus',
      order: 2,
      layout: eLayoutType.empty,
    },
    {
      path: '/tenant-admin',
      name: 'Tenant Admin',
      iconClass: 'fas fa-tachometer-alt',
      order: 3,
      layout: eLayoutType.application,
    },
    {
      path: '/tenant-admin/dashboard',
      name: 'Dashboard',
      iconClass: 'fas fa-chart-line',
      parentName: 'Tenant Admin',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/saas-management',
      name: 'SaaS Management',
      iconClass: 'fas fa-shield-alt',
      order: 4,
      layout: eLayoutType.application,
      requiredPolicy: 'AbpSaas.Tenants',
    },
    {
      path: '/saas-management/editions',
      name: 'Editions',
      iconClass: 'fas fa-box',
      parentName: 'SaaS Management',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/saas-management/subscriptions',
      name: 'Subscriptions',
      iconClass: 'fas fa-calendar-check',
      parentName: 'SaaS Management',
      order: 2,
      layout: eLayoutType.application,
    },
    {
      path: '/saas-management/invoices',
      name: 'Invoices',
      iconClass: 'fas fa-file-invoice-dollar',
      parentName: 'SaaS Management',
      order: 3,
      layout: eLayoutType.application,
    },
    {
      path: '/tasks',
      name: 'Tasks',
      iconClass: 'fas fa-tasks',
      order: 5,
      layout: eLayoutType.application,
    },
  ]);
}
