import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { add, minus } from '@devtakas/utility';

@Component({
  selector: 'cno-add-input',
  template: `
    <input [formControl]="valueACtrl" type="number"/>
    <select [formControl]="optCtrl">
        <option value="add">+</option>
        <option value="minus">-</option>
        <option value=""></option>
    </select>
    <input [formControl]="valueBCtrl" type="number"/>
    =
    {{res}}
  `,
  styles: [
  ]
})
export class ControlsOneComponent implements OnInit {

  valueACtrl = new FormControl('');
  valueBCtrl = new FormControl('');
  optCtrl = new FormControl<'add' | 'minus' | ''>('');

  res = '';

  ngOnInit(): void {
    this.controlInit();
  }

  private controlInit() {
    this.valueACtrl.valueChanges.subscribe(val => {
      const bVal = this.valueBCtrl.value;
      const opt = this.optCtrl.value;
      this.calc(val, bVal, opt);
    });
  }

  private calc(a: string | null, b: string | null, opt: 'add' | 'minus' | '' | null) {
    if (a == null || b ==null || opt == null) {
      this.res = '';
      return;
    }
    const anum = Number(a);
    const bnum = Number(b);
    if (isNaN(anum) || isNaN(bnum)) {
      this.res = '';
      return;
    }
    if (opt === 'add') {
      this.res = add(anum, bnum).toString();
    }
    this.res = minus(anum, bnum).toString();
  }
}
