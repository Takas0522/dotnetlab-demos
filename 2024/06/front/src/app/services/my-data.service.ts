import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { MyDayaBase, NewMyData } from '../models/my-data.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MyDataService {

  private readonly httpClient = inject(HttpClient);

  async getMyData() {
    return lastValueFrom(this.httpClient.get<NewMyData[]>(`${environment.apiendpoint}api/mydata`));
  }

  async getMyDataById(id: string) {
    return lastValueFrom(this.httpClient.get<NewMyData[]>(`${environment.apiendpoint}api/mydata/${id}`));
  }
}
