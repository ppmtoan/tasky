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
      children: [
        {
          path: '/tenant-admin/dashboard',
          name: 'Dashboard',
          iconClass: 'fas fa-chart-line',
          order: 1,
        },
      ],
    },
    {
      path: '/host-admin',
      name: 'Host Admin',
      iconClass: 'fas fa-shield-alt',
      order: 4,
      layout: eLayoutType.application,
      requiredPolicy: 'AbpSaas.Tenants',
      children: [
        {
          path: '/host-admin/editions',
          name: 'Editions',
          iconClass: 'fas fa-box',
          order: 1,
        },
        {
          path: '/host-admin/subscriptions',
          name: 'Subscriptions',
          iconClass: 'fas fa-calendar-check',
          order: 2,
        },
        {
          path: '/host-admin/invoices',
          name: 'Invoices',
          iconClass: 'fas fa-file-invoice-dollar',
          order: 3,
        },
      ],
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
