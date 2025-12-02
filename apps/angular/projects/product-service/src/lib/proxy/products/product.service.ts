import type { GetProductsInput, ProductCreateDto, ProductDto, ProductUpdateDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  apiName = 'ProductService';

  create = (input: ProductCreateDto) =>
    this.restService.request<any, ProductDto>(
      {
        method: 'POST',
        url: `/api/product-service/products`,
        body: input,
      },
      { apiName: this.apiName }
    );

  delete = (id: string) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/product-service/products/${id}`,
      },
      { apiName: this.apiName }
    );

  get = (id: string) =>
    this.restService.request<any, ProductDto>(
      {
        method: 'GET',
        url: `/api/product-service/products/${id}`,
      },
      { apiName: this.apiName }
    );

  getList = (input: GetProductsInput) =>
    this.restService.request<any, PagedResultDto<ProductDto>>(
      {
        method: 'GET',
        url: `/api/product-service/products`,
        params: {
          filterText: input.filterText,
          name: input.name,
          priceMin: input.priceMin,
          priceMax: input.priceMax,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName }
    );

  update = (id: string, input: ProductUpdateDto) =>
    this.restService.request<any, ProductDto>(
      {
        method: 'PUT',
        url: `/api/product-service/products/${id}`,
        body: input,
      },
      { apiName: this.apiName }
    );

  constructor(private restService: RestService) {}
}
