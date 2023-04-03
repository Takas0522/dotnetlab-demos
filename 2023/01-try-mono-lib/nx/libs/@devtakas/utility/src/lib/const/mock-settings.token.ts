import { InjectionToken } from '@angular/core';
import { MockSetting } from '../models/mock-setting.interface';

export const MOCK_SETTINGS_TOKEN = new InjectionToken<MockSetting[]>(
  'MockSettings'
);
