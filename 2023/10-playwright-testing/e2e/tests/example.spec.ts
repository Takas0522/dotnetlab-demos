import { test, expect } from '@playwright/test';
require('dotenv').config();

test.describe('Init時に作成されたテスト', () => {
  test('has title', async ({ page }) => {
    await page.goto('https://playwright.dev/');

    // Expect a title "to contain" a substring.
    await expect(page).toHaveTitle(/Playwright/);
  });

  test('get started link', async ({ page }) => {
    await page.goto('https://playwright.dev/');

    // Click the get started link.
    await page.getByRole('link', { name: 'Get started' }).click();

    // Expects page to have a heading with the name of Installation.
    await expect(page.getByRole('heading', { name: 'Installation' })).toBeVisible();
  });
});

test.describe('ToDoのアクセス時処理', () => {
  test('トップページにアクセスできる', async ({ page }) => {
    const url = process.env.TARGET_URL ? process.env.TARGET_URL : '';
    console.log(`URL is ${url}`)
    await page.goto(url);
    await expect(page).toHaveTitle(/Todo/);
  });

  test('ToDoの入力が不可から可能状態に変更される', async ({ page }) => {
  });

  test('ToDoの入力が不可から可能状態に変更されたら既存のToDoを参照できるようになる', async ({ page }) => {
  });

});

test.describe('ToDoの追加', () => {

  test('ToDoを登録したらリストにincompletedで追加される。再アクセスしたときも同様の状態になる', async ({ page }) => {
  });

  test('再アクセスしたときも同様の状態になる', async ({ page }) => {
  });
});

test.describe('ToDoの編集', () => {

  test('作成したToDoをComplete状態にすると画面に表示されなくなる', async ({ page }) => {
  });

  test('すべてのToDoを表示するとComplete状態にしたデータが表示される', async ({ page }) => {
  });

  test('再度アクセスしたときも同様の状態が維持される', async ({ page }) => {
  });

  test('既存のToDoに変更を行なうと画面に表示されなくなる', async ({ page }) => {
  });

  test('すべてのToDoを表示するとComplete状態にした既存データが表示される', async ({ page }) => {
  });
});

