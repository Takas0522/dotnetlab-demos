import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom, lastValueFrom, Observable, of, Subject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserModel } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private data: BehaviorSubject<UserModel[] | null> = new BehaviorSubject<UserModel[] | null>(null);

  constructor(
    private httpClient: HttpClient
  ) { }

  get data$(): Observable<UserModel[] | null> {
    return this.data.asObservable();
  }

  fetchUserData(force: boolean = false): void {
    if (this.data.value && force == false) {
      // マスタ系のデータはキャッシュしているデータでどうこうする
      return;
    }
    const url = `${environment.apiEndpoint}/api/user`;
    this.httpClient.get<UserModel[]>(url).subscribe(x => {
      this.data.next(x);
    });
  }
}
