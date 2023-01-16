import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ControlsOneComponent } from './controls-one.component';



@NgModule({
  declarations: [
    ControlsOneComponent
  ],
  imports: [
    ReactiveFormsModule
  ],
  exports: [
    ControlsOneComponent
  ]
})
export class ControlsOneModule { }
