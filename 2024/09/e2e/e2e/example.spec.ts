import { test, expect } from '@playwright/test';

test('トップページにアクセスする', async ({ page }) => {
  await page.goto('http://localhost:4200/');
  await expect(page.getByRole('button', { name: '新規作成' })).toBeVisible();
});

// test('新規作成ページから移動すると空の状態でユーザーIDとユーザー名が表示される', async ({ page }) => {
//   await page.goto('http://localhost:4200/');
//   const button = page.getByRole('button', { name: '新規作成' });
//   await button.click();
//   await expect(page.getByRole('textbox', { name: 'ユーザーID' })).toHaveValue('');
//   await expect(page.getByRole('textbox', { name: 'ユーザー名' })).toHaveValue('');
// });

// test('新規作成ページから移動すると削除ボタンが存在しない', async ({ page }) => {
//   await page.goto('http://localhost:4200/');
//   const button = page.getByRole('button', { name: '新規作成' });
//   await button.click();
//   await expect(page.getByRole('button', { name: '削除' })).not.toBeVisible();
// });

// test('既存データから移動すると選択した行のユーザーIDとユーザー名が表示される', async ({ page }) => {
//   await page.goto('http://localhost:4200/');
//   const row = page.getByRole('row', { name: 'John Doe3' });
//   await row.getByRole('cell', { name: '>' }).first().click();
//   await expect(page.getByRole('textbox', { name: 'ユーザーID' })).toHaveValue('3');
//   await expect(page.getByRole('textbox', { name: 'ユーザー名' })).toHaveValue('John Doe3');
// });

// test('既存データから移動すると削除ボタンが存在する', async ({ page }) => {
//   await page.goto('http://localhost:4200/');
//   const row = page.getByRole('row', { name: 'John Doe3' });
//   await row.getByRole('cell', { name: '>' }).first().click();
//   await expect(page.getByRole('button', { name: '削除' })).toBeVisible();
// });