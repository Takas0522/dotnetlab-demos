import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, firstValueFrom, catchError, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserModel } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private httpClient: HttpClient
  ) { }

  fetchUserData$(id: number): Observable<UserModel | null> {
    return this.httpClient.get<UserModel>(`${environment.apiEndpoint}/api/user/${id}`, {observe: 'response'}).pipe(
      map(m => {
        console.log(m)
        if (m.status == 404) {
          return null;
        }
        return m.body;
      }),
      catchError((err, ca) => {
        if (err.status == 404) {
          return of(null);
        }
        throw err;
      })
    );
  }

  postUserDataAsync(data: UserModel): Promise<Object> {
    return firstValueFrom(this.httpClient.post(`${environment.apiEndpoint}/api/user`, data));
  }

  deleteUserDataAsync(id: number): Promise<Object> {
    return firstValueFrom(this.httpClient.delete(`${environment.apiEndpoint}/api/user/${id}`));
  }

  userDataValidator(data: UserModel): {[key: string]: any}[] {
    const errors = [];
    if (data.id == null || data.id == 0) {
      errors.push({ idError: 'required' })
    }
    if (data.name == null || data.name === '') {
      errors.push({ nameError: 'required' })
    }
    return errors;

  }
}
