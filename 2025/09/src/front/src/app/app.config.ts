import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection, ErrorHandler } from '@angular/core';
import { provideRouter, withHashLocation } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, withInterceptors, HTTP_INTERCEPTORS } from '@angular/common/http';

import { routes } from './app.routes';
import { AuthInterceptor } from './auth/auth.interceptor';
import { applicationInsightsInterceptor } from './services/application-insights.interceptor';
import { ApplicationInsightsErrorHandler } from './services/application-insights-error-handler.service';
import { ApplicationInsightsService } from './services/application-insights.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withHashLocation()),
    provideHttpClient(
      withInterceptorsFromDi(),
      withInterceptors([applicationInsightsInterceptor])
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: ErrorHandler,
      useClass: ApplicationInsightsErrorHandler
    },
    ApplicationInsightsService
  ]
};
