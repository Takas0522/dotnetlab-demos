import { Inject, Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { MOCK_SETTINGS_TOKEN } from '../const/mock-settings.token';
import { MockSetting } from '../models/mock-setting.interface';

@Injectable()
export class BackendMock implements HttpInterceptor {
  constructor(
    @Inject(MOCK_SETTINGS_TOKEN)
    private readonly settings: MockSetting[]
  ) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const { url, method } = req;
    const includePathData = this.settings.filter((s) =>
      url.includes(s.mockingPath)
    );
    if (includePathData.length > 0 && method === 'GET') {
      const data = includePathData[0].mockBody;
      return of(new HttpResponse({ status: 200, body: data }));
    }
    return next.handle(req);
  }
}
