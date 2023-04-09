import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { add, minus } from '@takas0522/utility';

@Component({
  selector: 'lib-base-control',
  templateUrl: './base-control.component.html',
  styleUrls: ['./base-control.component.css']
})
export class BaseControlComponent {

  protected ctrlA = new FormControl('');
  protected ctrlB = new FormControl('');

  protected output = '';

  clickAdd() {
    const aval = Number(this.ctrlA.value == null ? '0' : this.ctrlA.value);
    const bval = Number(this.ctrlB.value == null ? '0' : this.ctrlB.value);
    this.output = (add(aval, bval)).toString();
  }

  clickMinus() {
    const aval = Number(this.ctrlA.value == null ? '0' : this.ctrlA.value);
    const bval = Number(this.ctrlB.value == null ? '0' : this.ctrlB.value);
    this.output = (minus(aval, bval)).toString();
  }
}
