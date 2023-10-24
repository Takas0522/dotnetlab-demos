import { defineConfig, devices } from '@playwright/test';
import config from './playwright.config';
import dotenv from 'dotenv';

dotenv.config();

export default defineConfig(config, {
    workers: 20,
    testDir: 'tests-examples',
    retries: process.env.CI ? 2 : 0,
    use: {
        trace: 'on',
        video: 'on',
        screenshot: 'on',
    },

    projects: [
        /* Test against branded browsers. */
        {
          name: 'Microsoft Edge',
          use: { ...devices['Desktop Edge'], channel: 'msedge' },
        }
      ],
});