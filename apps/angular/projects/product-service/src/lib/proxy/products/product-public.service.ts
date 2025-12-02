import type { ProductDto } from './models';
import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductPublicService {
  apiName = 'ProductService';

  getList = () =>
    this.restService.request<any, ProductDto[]>(
      {
        method: 'GET',
        url: `/api/product-service/public/products`,
      },
      { apiName: this.apiName }
    );

  constructor(private restService: RestService) {}
}
