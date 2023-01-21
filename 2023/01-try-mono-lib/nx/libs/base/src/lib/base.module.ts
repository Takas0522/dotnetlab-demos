import { NgModule } from '@angular/core';
import { BaseComponent } from './base.component';

import { ControlsOneModule } from '@devtakas/controls-one';
import { ControlsTwoModule } from '@devtakas/controls-two';
import {  } from '@devtakas/sample';

@NgModule({
  declarations: [BaseComponent],
  imports: [ControlsOneModule, ControlsTwoModule],
  exports: [BaseComponent],
})
export class BaseModule {
  constructor() {
    
  }
}
