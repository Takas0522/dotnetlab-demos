import { Component, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { forbiddenNameValidator } from '@takas0522/utility';

@Component({
  selector: 'lib-custom-one',
  templateUrl: './custom-one.component.html',
  styleUrls: ['./custom-one.component.css']
})
export class CustomOneComponent {

  @Input()
  name = 'aaaaaaaaaaaaaaaaaaaaaaaaaaa';

  protected control = new FormControl('', { validators: forbiddenNameValidator(new RegExp(this.name)) })
}
