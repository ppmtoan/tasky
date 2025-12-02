import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface GetProductsInput extends PagedAndSortedResultRequestDto {
  filterText?: string;
  name?: string;
  priceMin?: number;
  priceMax?: number;
}

export interface ProductCreateDto {
  name: string;
  price: number;
}

export interface ProductDto extends FullAuditedEntityDto<string> {
  name?: string;
  price: number;
}

export interface ProductUpdateDto {
  name: string;
  price: number;
}
