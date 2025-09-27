import { ErrorHandler, Injectable, inject } from '@angular/core';
import { ApplicationInsightsService } from './application-insights.service';

@Injectable()
export class ApplicationInsightsErrorHandler implements ErrorHandler {
  private appInsights = inject(ApplicationInsightsService);

  handleError(error: any): void {
    // コンソールにもエラーを出力（開発時のデバッグ用）
    console.error('Global error caught:', error);

    try {
      // エラーの詳細情報を収集
      const errorInfo = this.extractErrorInfo(error);

      // Application Insightsにエラーを送信
      this.appInsights.trackException(error, {
        errorSource: 'GlobalErrorHandler',
        errorType: errorInfo.type,
        errorMessage: errorInfo.message,
        stackTrace: errorInfo.stack,
        timestamp: new Date().toISOString(),
        userAgent: navigator.userAgent,
        url: window.location.href,
        referrer: document.referrer
      });

      // エラーメトリクスも追跡
      this.appInsights.trackMetric('Frontend.Errors', 1, {
        errorType: errorInfo.type,
        errorSource: 'GlobalErrorHandler'
      });

      // トレースとしても記録
      this.appInsights.trackTrace(`Global Error: ${errorInfo.message}`, 3, {
        errorType: errorInfo.type,
        stackTrace: errorInfo.stack
      });

    } catch (loggingError) {
      // Application Insightsへの送信でエラーが発生した場合
      console.error('Failed to log error to Application Insights:', loggingError);
    }
  }

  private extractErrorInfo(error: any): { type: string; message: string; stack?: string } {
    if (error instanceof Error) {
      return {
        type: error.constructor.name,
        message: error.message,
        stack: error.stack
      };
    }

    if (typeof error === 'string') {
      return {
        type: 'StringError',
        message: error
      };
    }

    if (error && typeof error === 'object') {
      if (error.message) {
        return {
          type: error.constructor?.name || 'ObjectError',
          message: error.message,
          stack: error.stack
        };
      }

      // オブジェクトエラーの場合、JSON文字列化を試行
      try {
        return {
          type: 'ObjectError',
          message: JSON.stringify(error)
        };
      } catch {
        return {
          type: 'ObjectError',
          message: 'Unable to serialize error object'
        };
      }
    }

    return {
      type: 'UnknownError',
      message: 'An unknown error occurred'
    };
  }
}
