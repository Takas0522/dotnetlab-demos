import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BaseControlComponent } from './components/base-control/base-control.component';
import { UtilityModule } from '@takas0522/utility';
import { ControlTwoModule } from '@takas0522/control-two';
import { ControlOneModule } from '@takas0522/control-one';



@NgModule({
  declarations: [
    BaseControlComponent
  ],
  imports: [
    ControlOneModule,
    ControlTwoModule,
    UtilityModule,
    ReactiveFormsModule
  ],
  exports: [
    BaseControlComponent
  ]
})
export class BaseModule { }
