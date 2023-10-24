import { test, expect } from '@playwright/test';
import { ToDoPageUserAction } from './todo/todo-page.action';

const extendsTest = test.extend<{ userAction: ToDoPageUserAction }>({
  userAction: async ({ page }, use) => {
    const userAction = new ToDoPageUserAction(page);
    await use(userAction);
  }
})

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
  extendsTest('トップページにアクセスできる。アクセス直後はToDoが追加できないこと。', async ({ userAction, page }) => {

    await userAction.ToDoページにアクセスする();

    const ctrl = page.getByRole('button', { name: '追加' })
    const res = await ctrl.isDisabled();
    expect(res).toBeTruthy();
  });

  extendsTest('データ取得完了後ToDoの入力が不可から可能状態に変更される', async ({ userAction, page }) => {

    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();

    const ctrl = page.getByRole('button', { name: '追加' })
    const res = await ctrl.isDisabled();
    expect(res).toBeFalsy();
  });

  extendsTest('ToDoの入力が不可から可能状態に変更されたら既存のToDoを参照できるようになること', async ({ userAction, page }) => {
    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();

    // ToDoは1件以上
    const afTotos = await page.locator('mat-card').count();
    expect(afTotos).toBeGreaterThanOrEqual(1);
  });

  extendsTest('「すべてのToDoを表示」すると完了済みのToDoが表示されること', async ({ userAction, page }) => {
    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();
    await userAction.すべてのToDoを表示();
    await page.waitForSelector('text=check_box');

    const completedTodo = await page.locator('button').filter({ hasText: /^check_box$/ }).count();
    expect(completedTodo).toBeGreaterThanOrEqual(1);
  });

});

test.describe('ToDoの追加', () => {

  extendsTest('ToDoを登録したらリストにincompletedで追加され再アクセスしたときも同様となる', async ({ userAction, page }) => {
    const text = 'テストToDo';
    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();
    await userAction.ToDoに指定された文字列を登録する(text);

    const ctrlCount = await page.getByText(text).count();
    expect(ctrlCount).toBeGreaterThan(0);

    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();
    const ctrlCountAf = await page.getByText(text).count();
    expect(ctrlCountAf).toBeGreaterThan(0);
  });
});

test.describe('ToDoの編集とすべてのToDoの表示', () => {

  const writeText = 'Edit対象のToDo';
  extendsTest('作成したToDoをComplete状態にすると画面に表示されなくなり再度アクセスしたときもComplete状態が維持される', async ({userAction, page }) => {
    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();
    await userAction.ToDoに指定された文字列を登録する(writeText);

    const ctrlCountBf = await page.getByText(writeText).count();
    expect(ctrlCountBf).toBeGreaterThan(0);

    await userAction.指定された文字列のToDoをCompleteにする(writeText);

    const ctrlCount = await page.getByText(writeText).count();
    expect(ctrlCount).toEqual(0);

    await userAction.ToDoページにアクセスする();
    await userAction.ToDoのLoadを待つ();
    const ctrlCountReload = await page.getByText(writeText).count();
    expect(ctrlCountReload).toEqual(0);
  });

});
