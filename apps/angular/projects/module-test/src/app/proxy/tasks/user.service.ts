import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { UserDto } from './models';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  getUsers(): Observable<UserDto[]> {
    return this.restService.request<void, UserDto[]>({
      method: 'GET',
      url: '/api/app/user',
    },
    { apiName: this.apiName });
  }

  getTenantUsers(): Observable<UserDto[]> {
    return this.restService.request<void, UserDto[]>({
      method: 'GET',
      url: '/api/app/user/tenant-users',
    },
    { apiName: this.apiName });
  }
}
