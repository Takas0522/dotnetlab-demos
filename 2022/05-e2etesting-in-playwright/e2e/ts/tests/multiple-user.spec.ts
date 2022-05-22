import { expect, test } from '@playwright/test'
import 'dotenv/config'

const baseUrl = process.env.FRONT_ENDPOINT;

test.describe('マルチユーザーのテスト(テストユーザー1)', () => {

    test.use({ storageState: 'user1StorageState.json' });

    test('テストユーザー1でログインされていること', async ({ page, context }) => {
        await page.goto(`${baseUrl}`);
        await page.waitForTimeout(5000);
        const inputctrl = page.locator('text=TestUser')
        expect(inputctrl).toHaveCount(1);
    });
});

test.describe('マルチユーザーのテスト(テストユーザー2)', () => {

    test.use({ storageState: 'user2StorageState.json' });

    test('テストユーザー2でログインされていること', async ({ page, context }) => {
        await page.goto(`${baseUrl}`);
        await page.waitForTimeout(5000);
        const inputctrl = page.locator('text=TestUser2')
        expect(inputctrl).toHaveCount(1);
    });
});