import {
  EnvironmentProviders,
  inject,
  makeEnvironmentProviders,
  provideAppInitializer,
} from '@angular/core';
import { RoutesService, eLayoutType } from '@abp/ng.core';
import { eProductServiceRouteNames } from '../enums';

export const PRODUCT_SERVICE_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    const routesService = inject(RoutesService);
    routesService.add([
      {
        name: eProductServiceRouteNames.ProductService,
        iconClass: 'fas fa-book',
        layout: eLayoutType.application,
        order: 999,
      },
      {
        path: '/product-service/products',
        name: eProductServiceRouteNames.Products,
        parentName: eProductServiceRouteNames.ProductService,
        layout: eLayoutType.application,
        requiredPolicy: 'ProductService.Products',
      },
    ]);
  }),
];

export function provideProductServiceConfig(): EnvironmentProviders {
  return makeEnvironmentProviders([PRODUCT_SERVICE_ROUTE_PROVIDER]);
}
