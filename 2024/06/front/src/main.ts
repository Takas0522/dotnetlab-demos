import { bootstrapApplication } from '@angular/platform-browser';
import { genAppConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { featureFlagOverride } from './featureflag-override';
import { environment } from './environments/environment';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

// APIで提供されている環境変数情報からFeatureFlagの情報を取得しInjectionTokenに格納する
featureFlagOverride(environment.featureFlag).then((env => {
  bootstrapApplication(AppComponent, genAppConfig(env))
  .catch((err) => console.error(err));
}));
