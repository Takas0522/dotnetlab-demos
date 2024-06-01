import { authClient } from "./app/auth/auth";
import { environment } from "./environments/environment";

export const featureFlagOverride = async <T>(env: T): Promise<T> => {
  try {
    const client = authClient();
    const token = await client.aquireToken(environment.authScopes);
    // WebAPI経由でAzureのAppConfigurationを見る
    const res = await fetch(`${environment.apiendpoint}api/featureflags`, {
      headers: {
        'Authorization': `Bearer ${token}`,
      }
    });
    // 存在しないフラグがあれば追加、同一の値が存在する場合は上書きを行う
    const fetchEnv: T = await res.json();
    for (const key in fetchEnv) {
      const target: any = fetchEnv[key];
      if (target != null) {
          env[key] = target;
      }
    }
    return env;
  } catch (e) {
    // エラー発生時は何もしない
    return env;
  }
};
