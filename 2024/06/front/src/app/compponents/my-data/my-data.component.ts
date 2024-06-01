import { Component, WritableSignal, inject, signal } from '@angular/core';
import { FeatureFlagDirective } from '../../directives/feature-flag.directive';
import { CommonModule } from '@angular/common';
import { MyDataService } from '../../services/my-data.service';
import { MyDayaBase, NewMyData } from '../../models/my-data.model';
import { FEATURE_FLAG_ENVIRONMENT } from '../../static/feature-flag';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-my-data',
  standalone: true,
  imports: [
    CommonModule,
    FeatureFlagDirective,
    FormsModule,
    MatInputModule,
    MatFormFieldModule
  ],
  templateUrl: './my-data.component.html',
  styleUrl: './my-data.component.scss'
})
export class MyDataComponent {

  protected myData: WritableSignal<NewMyData[]>  = signal([]);

  protected  inputId = '';

  readonly #myDataService = inject(MyDataService);
  readonly #featureEnv = inject(FEATURE_FLAG_ENVIRONMENT);

  async search() {
    if (this.#featureEnv['FeatureSearch']) {
      const data = await this.#myDataService.getMyDataById(this.inputId);
      this.myData.set(data);
      return;
    }
    const data = await this.#myDataService.getMyData();
    this.myData.set(data);
  }

  protected cauUseFeature(featureflag: string): boolean {
    return this.#featureEnv[featureflag];
  }
}
