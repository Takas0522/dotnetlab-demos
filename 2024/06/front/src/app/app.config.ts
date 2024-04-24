import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { FEATURE_FLAG_ENVIRONMENT } from './static/feature-flag';
import { HttpClientModule } from '@angular/common/http';

export const genAppConfig = (overridideenv: Record<string, boolean>): ApplicationConfig => {
  const res: ApplicationConfig = {
    providers: [
      provideRouter(routes),
      { provide: FEATURE_FLAG_ENVIRONMENT, useValue: overridideenv },
      importProvidersFrom(HttpClientModule)
    ],
  };
  return res
}
