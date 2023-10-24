import { defineConfig, devices } from '@playwright/test';
import config from './playwright.config';
import dotenv from 'dotenv';

dotenv.config();

// Name the test run if it's not named yet.
process.env.PLAYWRIGHT_SERVICE_RUN_ID = process.env.PLAYWRIGHT_SERVICE_RUN_ID || new Date().toISOString();

// Can be 'linux' or 'windows'.
const os = process.env.PLAYWRIGHT_SERVICE_OS || 'linux';

export default defineConfig(config, {
    testDir: 'tests-examples',
    workers: 20,
    retries: process.env.CI ? 2 : 0,
    ignoreSnapshots: false,
    snapshotPathTemplate: `{testDir}/__screenshots__/{testFilePath}/${os}/{arg}{ext}`,

    use: {
        // Specify the service endpoint.
        connectOptions: {
            wsEndpoint: `${process.env.PLAYWRIGHT_SERVICE_URL}?cap=${JSON.stringify({
                // Can be 'linux' or 'windows'.
                os,
                runId: process.env.PLAYWRIGHT_SERVICE_RUN_ID
            })}`,
            timeout: 30000,
            headers: {
                'x-mpt-access-key': process.env.PLAYWRIGHT_SERVICE_ACCESS_TOKEN!
            },
            // Allow service to access the localhost.
            exposeNetwork: '<loopback>'
        },
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