import { InjectionToken } from "@angular/core";

export const FEATURE_FLAG_ENVIRONMENT = new InjectionToken<Record<string, boolean>>("featureflagenv");
