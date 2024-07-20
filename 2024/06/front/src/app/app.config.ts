import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { FEATURE_FLAG_ENVIRONMENT } from './static/feature-flag';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { AuthInterceptor } from './interceptor/auth.interceptor';

export const genAppConfig = (overridideenv: Record<string, boolean>): ApplicationConfig => {
  const res: ApplicationConfig = {
    providers: [
      provideRouter(routes),
      { provide: FEATURE_FLAG_ENVIRONMENT, useValue: overridideenv },
      importProvidersFrom(HttpClientModule),
      provideAnimations(),
      { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}
    ],
  };
  return res
}

