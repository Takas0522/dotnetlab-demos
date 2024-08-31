import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './input.component.html',
  styleUrl: './input.component.scss'
})
export class InputComponent implements OnInit, ControlValueAccessor {

  protected ctrl = new FormControl('');
  onChange!: (obj: any) => void;
  onTouched!: (obj: any) => void;
  hasError = false;
  @Input() placeholder = 'AAA';

  constructor(
    @Self() @Optional() public ngControl: NgControl
  ) {
    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  ngOnInit(): void {
    if (this.ngControl.control?.validator) {
      this.ctrl.validator = this.ngControl.control?.validator;
    }
    this.ctrl.valueChanges.subscribe((value) => {
      this.onChange(value);
    });
    this.ctrl.statusChanges.subscribe(x => {
      if (x === 'INVALID') {
        console.log(this.ctrl.errors);
        this.ngControl.control?.setErrors(this.ctrl.errors);
        this.hasError = true;
      }
      if (x === 'VALID') {
        this.hasError = false;
        this.ngControl.control?.setErrors(null);
      }
    });
  }
  writeValue(obj: string): void {
    this.ctrl.setValue(obj);
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
      if (isDisabled) {
        this.ctrl.disable();
      } else {
        this.ctrl.enable();
    }
  }
}
