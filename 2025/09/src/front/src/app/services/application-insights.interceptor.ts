import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, finalize, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { ApplicationInsightsService } from './application-insights.service';

export const applicationInsightsInterceptor: HttpInterceptorFn = (req, next) => {
  const appInsights = inject(ApplicationInsightsService);
  const startTime = performance.now();
  const operationId = Math.random().toString(36).substring(7);

  // リクエスト開始のログ
  appInsights.trackTrace(`HTTP Request Start: ${req.method} ${req.url}`, 1, {
    operationId,
    method: req.method,
    url: req.url,
    headers: JSON.stringify(req.headers.keys())
  });

  return next(req).pipe(
    tap(response => {
      const duration = performance.now() - startTime;
      
      // 成功時のテレメトリ
      appInsights.trackDependency(
        operationId,
        `HTTP ${req.method}`,
        req.url,
        duration,
        true,
        200, // レスポンスコードがある場合は実際の値を使用
        {
          method: req.method,
          url: req.url,
          statusText: 'Success'
        }
      );

      // パフォーマンスメトリクス
      appInsights.trackMetric('HTTP.RequestDuration', duration, {
        method: req.method,
        endpoint: req.url,
        success: 'true'
      });

      appInsights.trackTrace(`HTTP Request Success: ${req.method} ${req.url}`, 1, {
        operationId,
        duration: duration.toString(),
        success: 'true'
      });
    }),
    catchError(error => {
      const duration = performance.now() - startTime;
      const statusCode = error.status || 0;

      // エラー時のテレメトリ
      appInsights.trackDependency(
        operationId,
        `HTTP ${req.method}`,
        req.url,
        duration,
        false,
        statusCode,
        {
          method: req.method,
          url: req.url,
          error: error.message || 'Unknown error',
          statusCode: statusCode.toString()
        }
      );

      // エラーメトリクス
      appInsights.trackMetric('HTTP.RequestDuration', duration, {
        method: req.method,
        endpoint: req.url,
        success: 'false',
        statusCode: statusCode.toString()
      });

      // 例外追跡
      appInsights.trackException(new Error(`HTTP Error: ${req.method} ${req.url}`), {
        operationId,
        httpMethod: req.method,
        url: req.url,
        statusCode: statusCode.toString(),
        duration: duration.toString(),
        errorMessage: error.message || 'HTTP request failed'
      });

      appInsights.trackTrace(`HTTP Request Error: ${req.method} ${req.url}`, 3, {
        operationId,
        duration: duration.toString(),
        success: 'false',
        statusCode: statusCode.toString(),
        error: error.message || 'Unknown error'
      });

      return throwError(() => error);
    }),
    finalize(() => {
      // リクエスト完了のログ
      const duration = performance.now() - startTime;
      appInsights.trackTrace(`HTTP Request Complete: ${req.method} ${req.url}`, 1, {
        operationId,
        totalDuration: duration.toString()
      });
    })
  );
};
