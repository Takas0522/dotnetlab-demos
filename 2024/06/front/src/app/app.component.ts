import { Component, OnInit, WritableSignal, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FEATURE_FLAG_ENVIRONMENT } from './static/feature-flag';
import { CommonModule } from '@angular/common';
import { FeatureFlagDirective } from './directives/feature-flag.directive';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {

  #setting = inject(FEATURE_FLAG_ENVIRONMENT);
  private settingsData = signal(this.#setting);

  ngOnInit(): void {
    console.log(this.#setting)
  }


}
