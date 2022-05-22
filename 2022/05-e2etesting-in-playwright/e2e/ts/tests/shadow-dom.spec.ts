import { expect, test } from '@playwright/test'
import 'dotenv/config'

const baseUrl = process.env.FRONT_ENDPOINT;

test.describe('Shadow DOMのテスト', () => {

    test.use({ storageState: 'user1StorageState.json' });

    test('Shadow DOM内のコンテンツを補足しテストできること', async ({ page }) => {
        await page.goto(`${baseUrl}/shadow`);
        const inputctrl = page.locator('input[name="shadow-dom-input"]')
        expect(inputctrl).toHaveCount(1);
    });
});