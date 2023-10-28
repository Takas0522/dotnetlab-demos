import { test, expect } from '@playwright/test';

test.describe('localhostのテスト', () => {
  test('アクセスできるか？', async ({ page }) => {
    await page.goto('http://localhost:5500');

    // Expect a title "to contain" a substring.
    await expect(page).toHaveTitle(/Front2/);
  });

});
