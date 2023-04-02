import { NgModule } from '@angular/core';
import { ForbiddenValidatorDirective } from './directives/forbidden-validator.directive';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    ForbiddenValidatorDirective
  ],
  imports: [
    CommonModule
  ],
  exports: [
    ForbiddenValidatorDirective
  ]
})
export class UtilityModule { }
