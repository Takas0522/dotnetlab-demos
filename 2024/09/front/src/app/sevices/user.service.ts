import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../models/user.interface';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  #httpClient = inject(HttpClient);

  getUsers() {
    return this.#httpClient.get<User[]>(environment.apiUrl + '/users');
  }

  getUser(userId: string) {
    return this.#httpClient.get<User>(environment.apiUrl + '/user/' + userId);
  }

  postUser(user: User) {
    return this.#httpClient.post(environment.apiUrl + '/user', user);
  }
}
