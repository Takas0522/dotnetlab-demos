import { Injectable, inject } from '@angular/core';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApplicationInsightsService {
  private appInsights: ApplicationInsights;

  constructor() {
    this.appInsights = new ApplicationInsights({
      config: {
        connectionString: environment.applicationInsights.connectionString,
        enableAutoRouteTracking: true, // Angularルート変更の自動追跡
        enableCorsCorrelation: true,
        enableRequestHeaderTracking: true,
        enableResponseHeaderTracking: true,
        enableAjaxErrorStatusText: true,
        enableUnhandledPromiseRejectionTracking: true,
        disableFetchTracking: false,
        disableXhr: false,
        disableExceptionTracking: false,
        samplingPercentage: 100,
        autoTrackPageVisitTime: true,
        namePrefix: 'TodoApp-',
        enableDebug: !environment.production
      }
    });

    this.appInsights.loadAppInsights();
    this.appInsights.trackPageView(); // ページビュー追跡の手動開始
  }

  /**
   * カスタムイベントの追跡
   */
  trackEvent(name: string, properties?: Record<string, any>, measurements?: Record<string, number>): void {
    this.appInsights.trackEvent({ 
      name,
      properties,
      measurements
    });
  }

  /**
   * ページビューの追跡
   */
  trackPageView(name?: string, uri?: string, properties?: Record<string, any>): void {
    this.appInsights.trackPageView({ 
      name, 
      uri,
      properties 
    });
  }

  /**
   * 例外の追跡
   */
  trackException(error: Error, properties?: Record<string, any>): void {
    this.appInsights.trackException({ 
      exception: error,
      properties: {
        ...properties,
        timestamp: new Date().toISOString(),
        userAgent: navigator.userAgent,
        url: window.location.href
      }
    });
  }

  /**
   * メトリクスの追跡
   */
  trackMetric(name: string, average: number, properties?: Record<string, string>): void {
    this.appInsights.trackMetric({ name, average }, properties);
  }

  /**
   * 依存関係の追跡（API呼び出しなど）
   */
  trackDependency(
    id: string, 
    name: string, 
    data: string, 
    duration: number, 
    success: boolean,
    responseCode?: number,
    properties?: Record<string, any>
  ): void {
    this.appInsights.trackDependencyData({
      id,
      name,
      data,
      duration,
      success,
      responseCode: responseCode || (success ? 200 : 500),
      properties
    });
  }

  /**
   * トレースの追跡
   */
  trackTrace(message: string, severityLevel?: number, properties?: Record<string, any>): void {
    this.appInsights.trackTrace({ message, severityLevel }, properties);
  }

  /**
   * ユーザー情報の設定
   */
  setUser(userId: string, accountId?: string, userName?: string): void {
    this.appInsights.setAuthenticatedUserContext(userId, accountId);
    
    // カスタムプロパティとしてユーザー名も追加
    if (userName) {
      this.appInsights.addTelemetryInitializer((envelope) => {
        if (envelope.data) {
          envelope.data['userName'] = userName;
        }
      });
    }
  }

  /**
   * ユーザー情報のクリア
   */
  clearUser(): void {
    this.appInsights.clearAuthenticatedUserContext();
  }

  /**
   * パフォーマンス測定の開始
   */
  startTrackEvent(name: string): string {
    const timerId = Math.random().toString(36).substring(7);
    this.appInsights.trackEvent({ name: `${name}_Start` }, { timerId });
    return timerId;
  }

  /**
   * パフォーマンス測定の終了
   */
  stopTrackEvent(name: string, timerId: string, properties?: Record<string, any>): void {
    this.appInsights.trackEvent({ 
      name: `${name}_End` 
    }, { 
      ...properties, 
      timerId 
    });
  }

  /**
   * カスタムパフォーマンス測定
   */
  async measurePerformance<T>(
    operationName: string, 
    operation: () => Promise<T>,
    properties?: Record<string, any>
  ): Promise<T> {
    const startTime = performance.now();
    const timerId = this.startTrackEvent(operationName);

    try {
      const result = await operation();
      const duration = performance.now() - startTime;
      
      this.trackMetric(`${operationName}.Duration`, duration, {
        ...properties,
        success: 'true'
      });
      
      this.stopTrackEvent(operationName, timerId, { 
        ...properties, 
        duration: duration.toString(),
        success: 'true'
      });
      
      return result;
    } catch (error) {
      const duration = performance.now() - startTime;
      
      this.trackMetric(`${operationName}.Duration`, duration, {
        ...properties,
        success: 'false'
      });
      
      this.trackException(error as Error, { 
        ...properties, 
        operationName,
        duration: duration.toString()
      });
      
      this.stopTrackEvent(operationName, timerId, { 
        ...properties, 
        duration: duration.toString(),
        success: 'false',
        error: (error as Error).message
      });
      
      throw error;
    }
  }

  /**
   * Application Insightsインスタンスの取得（高度な使用用途）
   */
  getAppInsights(): ApplicationInsights {
    return this.appInsights;
  }
}
