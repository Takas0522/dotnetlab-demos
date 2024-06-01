import { bootstrapApplication } from '@angular/platform-browser';
import { genAppConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { featureFlagOverride } from './featureflag-override';
import { environment } from './environments/environment';
import { loginChallenge } from './app/auth/auth';

// APIで提供されている環境変数情報からFeatureFlagの情報を取得しInjectionTokenに格納する
loginChallenge().then(res => {
  if (res) {
    featureFlagOverride(environment.featureFlag).then((env => {
      bootstrapApplication(AppComponent, genAppConfig(env))
      .catch((err) => console.error(err));
    }));
  }
})

