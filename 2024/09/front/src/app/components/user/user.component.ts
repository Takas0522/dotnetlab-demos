import { Component, inject, OnInit, signal } from '@angular/core';
import { InputComponent } from '../../controls/input/input.component';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonComponent } from '../../controls/button/button.component';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../sevices/user.service';
import { User } from '../../models/user.interface';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    InputComponent,
    ReactiveFormsModule,
    ButtonComponent
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss'
})
export class UserComponent implements OnInit {

  #route = inject(ActivatedRoute);
  #router = inject(Router);

  #userService = inject(UserService);

  protected isExistsUser = signal(false);

  protected formGroup = new FormGroup({
    userId: new FormControl<null | number>(null, Validators.required),
    userName: new FormControl('', Validators.required)
  });

  ngOnInit(): void {
    this.#route.params.subscribe(params => {
      const userId = params['id'];
      if (userId != null) {
        this.#getUser(userId);
        this.isExistsUser.set(true);
        return;
      }
      this.isExistsUser.set(false);
    });
  }

  #getUser(userId: string) {
    this.#userService.getUser(userId).subscribe(user => {
      this.formGroup.patchValue({
        userId: user.id,
        userName: user.name
      });
    });
  }

  submitUser() {
    if (this.formGroup.invalid) {
      return;
    }
    const value = this.formGroup.value;
    if (value.userId != null && !isNaN(value.userId) && value.userName != null) {
      const user: User = {
        id: value.userId,
        name: value.userName
      };
      this.#userService.postUser(user).subscribe(() => {
        this.#router.navigate(['']);
      });
    }
  }
}
