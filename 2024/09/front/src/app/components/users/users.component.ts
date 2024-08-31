import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../sevices/user.service';
import { User } from '../../models/user.interface';
import { Router, RouterModule } from '@angular/router';
import { ButtonComponent } from "../../controls/button/button.component";

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    RouterModule,
    ButtonComponent
],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {

  #usersService = inject(UserService);
  protected $users = signal<User[]>([]);
  #router = inject(Router);

  ngOnInit() {
    this.#usersService.getUsers().subscribe(users => {
      this.$users.set(users);
    });
  }

  makeNewUser() {
    this.#router.navigate(['user']);
  }

}
