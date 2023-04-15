import { NgModule } from '@angular/core';
import { CustomOneComponent } from './components/custom-one/custom-one.component';
import { ReactiveFormsModule } from '@angular/forms';
import { CustomLabelComponent } from './components/custom-label/custom-label.component';
import { UtilityModule } from '@takas0522/utility';

@NgModule({
  declarations: [
    CustomOneComponent,
    CustomLabelComponent
  ],
  imports: [
    UtilityModule,
    ReactiveFormsModule
  ],
  exports: [
    CustomOneComponent,
    CustomLabelComponent
  ]
})
export class ControlOneModule { }
