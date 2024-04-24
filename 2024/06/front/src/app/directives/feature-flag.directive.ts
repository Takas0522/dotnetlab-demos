import { NgIf } from '@angular/common';
import { Directive, Input, OnInit, inject } from '@angular/core';
import { FEATURE_FLAG_ENVIRONMENT } from '../static/feature-flag';

@Directive({
  selector: '[appFeatureFlag]',
  standalone: true,
  hostDirectives: [NgIf]
})
export class FeatureFlagDirective implements OnInit {

  @Input()
  appFeatureFlag = '';

  readonly #ngifDirective = inject(NgIf);
  readonly #featureFlag = inject(FEATURE_FLAG_ENVIRONMENT);

  ngOnInit() {
    this.#ngifDirective.ngIf = this.#featureFlag[this.appFeatureFlag];
  }

}
