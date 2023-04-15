import { Component, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { forbiddenNameValidator } from 'projects/takas0522/utility/src/public-api';

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
