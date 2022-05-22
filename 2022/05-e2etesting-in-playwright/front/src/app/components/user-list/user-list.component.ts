import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { UserModel } from 'src/app/models/user.model';
import { UsersService } from 'src/app/services/users.service';
import { UserListQuery } from './user-list.query';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

  users$!: Observable<UserModel[]>;

  constructor(
    private query: UserListQuery,
    private service: UsersService
  ) { }

  ngOnInit(): void {
    this.parameterInit();
    this.componentInitAction();
  }

  private parameterInit() {
    this.users$ = this.query.datas$;
  }

  private componentInitAction() {
    this.service.fetchUserData();
  }

}
