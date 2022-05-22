import { Injectable } from "@angular/core";
import { map } from "rxjs";
import { UsersService } from "src/app/services/users.service";

@Injectable({
  providedIn: 'root'
})
export class UserListQuery {

  constructor(
    private service: UsersService
  ) {}
  get datas$() {
    return this.service.data$.pipe(
      map(m => {
        if (m) {
          return m;
        }
        return [];
      })
    )
  }
}