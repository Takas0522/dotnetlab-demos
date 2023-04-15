import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BaseControlComponent } from './components/base-control/base-control.component';
import { UtilityModule } from 'projects/takas0522/utility/src/public-api';
import { ControlTwoModule } from 'projects/takas0522/control-two/src/public-api';
import { ControlOneModule } from 'projects/takas0522/control-one/src/public-api';



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
