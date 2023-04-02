import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CtControlComponent } from './components/ct-control/ct-control.component';
import { CtButtonComponent } from './components/ct-button/ct-button.component';



@NgModule({
  declarations: [
    CtControlComponent,
    CtButtonComponent
  ],
  imports: [
    ReactiveFormsModule
  ],
  exports: [
    CtControlComponent,
    CtButtonComponent
  ]
})
export class ControlTwoModule { }
