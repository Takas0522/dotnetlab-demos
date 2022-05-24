import { expect, test } from '@playwright/test'
import 'dotenv/config'

const baseUrl = process.env.FRONT_ENDPOINT;

test.describe('ユーザー', () => {

  test.use({ storageState: 'user1StorageState.json' });

  test('/usersエンドポイントアクセスでユーザーの一覧が表示されていること', async ({ page }) => {
    /* これだとネットワーク不調のとき失敗する */
    await page.goto(`${baseUrl}/users`);
     await page.waitForTimeout(3000);
    /* Pageの遷移とコントロールの出現を待機するようにしておけば変更に強いコードになる */
    // await page.waitForSelector(`mat-list-item`);
    const loc = page.locator('mat-list-item');
    const rowcn = await loc.count();
    expect(rowcn).toBeGreaterThan(0);
  });
});
