import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ControlOneModule } from '@takas0522/control-one';
import { ControlTwoModule } from '@takas0522/control-two';
import { UtilityModule } from '@takas0522/utility';
import { BaseControlComponent } from './components/base-control/base-control.component';



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
