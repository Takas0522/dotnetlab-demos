import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ControlOneModule } from '@devtakas/control-one';
import { ControlTwoModule } from '@devtakas/control-two';
import { UtilityModule } from '@devtakas/utility';
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
