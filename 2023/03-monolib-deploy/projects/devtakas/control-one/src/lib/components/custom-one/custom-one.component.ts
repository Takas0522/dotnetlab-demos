import { Component, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { forbiddenNameValidator } from '@devtakas/utility';

@Component({
  selector: 'lib-custom-one',
  templateUrl: './custom-one.component.html',
  styleUrls: ['./custom-one.component.css']
})
export class CustomOneComponent {

  @Input()
  name = '';

  protected control = new FormControl('', { validators: forbiddenNameValidator(new RegExp(this.name)) })
}
