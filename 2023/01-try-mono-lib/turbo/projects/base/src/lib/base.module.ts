import { NgModule } from '@angular/core';
import { BaseComponent } from './base.component';

import { ControlsOneModule } from '@devtakas/control-one';
import { ControlsTwoModule } from '@devtakas/control-two';

@NgModule({
  declarations: [
    BaseComponent
  ],
  imports: [
    ControlsOneModule,
    ControlsTwoModule
  ],
  exports: [
    BaseComponent
  ]
})
export class BaseModule { }
