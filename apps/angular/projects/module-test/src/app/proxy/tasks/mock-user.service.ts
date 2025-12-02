import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { UserDto } from './models';

@Injectable({
  providedIn: 'root',
})
export class MockUserService {
  private mockUsers: UserDto[] = [
    {
      id: 'user-1',
      userName: 'john.doe',
      name: 'John',
      surname: 'Doe',
      email: 'john.doe@example.com'
    },
    {
      id: 'user-2',
      userName: 'jane.smith',
      name: 'Jane',
      surname: 'Smith',
      email: 'jane.smith@example.com'
    },
    {
      id: 'user-3',
      userName: 'bob.wilson',
      name: 'Bob',
      surname: 'Wilson',
      email: 'bob.wilson@example.com'
    },
    {
      id: 'user-4',
      userName: 'admin',
      name: 'Admin',
      surname: 'User',
      email: 'admin@example.com'
    }
  ];

  getUsers(): Observable<UserDto[]> {
    return of(this.mockUsers).pipe(delay(300));
  }

  getTenantUsers(): Observable<UserDto[]> {
    return of(this.mockUsers).pipe(delay(300));
  }
}
