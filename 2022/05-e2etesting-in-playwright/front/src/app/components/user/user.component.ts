import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/services/user.service';
import { UsersService } from 'src/app/services/users.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {

  isUpdate = false

  formGroup = new FormGroup({
    id: new FormControl(''),
    name: new FormControl(''),
    email: new FormControl(''),
    userType: new FormControl(1)
  }, { validators: userDataValidators(this.service) });

  get idErrorsIsReq(): boolean {
    const d = this.hasError('idError');
    if (d == null) {
      return false;
    }
    return d['idError'] === 'required';
  }

  get nameErrorsIsReq(): boolean {
    const d = this.hasError('nameError');
    if (d == null) {
      return false;
    }
    return d['nameError'] === 'required';
  }

  constructor(
    private service: UserService,
    private usersService: UsersService,
    private route: ActivatedRoute,
    private router: Router

  ) { }

  ngOnInit(): void {
    this.valiableInit();
    this.controlInit();
  }

  private valiableInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.isUpdate = (id != null);
    if (id != null) {
      const idnum = Number(id);
      this.service.fetchUserData$(idnum).subscribe(x => {
        if (x) {
          this.formGroup.patchValue(x);
          return;
        }
        alert('デーがない！');
        this.router.navigate(['/users']);
      });
    }
  }

  private controlInit() {
    if(this.isUpdate) {
      const ctrl = this.formGroup.get('id');
      ctrl?.disable();
    }
  }

  private hasError(errorName: string): {[key: string]: any} | null {
    if (!this.formGroup.errors) {
      return null;
    }
    for (const prop in this.formGroup.errors) {
      const error = this.formGroup.errors[prop];
      if (error.hasOwnProperty(errorName)) {
        return error;
      }
    }
    return null;
  }

  async onSubmit() {
    console.log('onsubmit')
    const val = this.formGroup.value;
    await this.service.postUserDataAsync(val);
    this.usersService.fetchUserData(true);
  }

}

const userDataValidators = (service: UserService): ValidatorFn => {
  return (fg) => {
    // Control Errors Init
    fg.get('id')?.setErrors(null);
    fg.get('name')?.setErrors(null);

    // Business Logic Error Validation
    const validators = service.userDataValidator((fg as FormGroup).getRawValue());
    if (validators === []) {
      return null;
    }

    // Set Errors Control
    for (const prop in validators) {
      const error = validators[prop];
      if (error.hasOwnProperty('idError')) {
        fg.get('id')?.setErrors(error);
      }
      if (error.hasOwnProperty('nameError')) {
        fg.get('name')?.setErrors(error);
      }
    }
    return validators;
  }
}