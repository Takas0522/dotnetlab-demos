import { test } from '@playwright/test';
import { UserTestActions } from './user-test.actions';

const extendsTest = test.extend<{ userAction: UserTestActions }>({
  userAction: async ({ page }, use) => {
    const userAction = new UserTestActions(page);
    await use(userAction);
  }
});

test.describe('ユーザー情報の登録テスト', () => {

  test.use({ storageState: 'user1StorageState.json' });

  extendsTest('ユーザー登録で不備があると画面上にエラーが表示され登録できないこと', async ({ userAction }) => {
    await userAction.ユーザー一覧ページに移動する();
    await userAction.一覧ページから新規登録ページに移動();
    await userAction.不備がある形でデータの入力();
    await userAction.エラーが表示されデータの登録が行えないこと();
  });

  extendsTest('ユーザー登録を行うと一覧のデータ件数が増加していること', async ({ userAction }) => {
    await userAction.ユーザー一覧ページに移動しデータ件数を確認する();
    await userAction.一覧ページから新規登録ページに移動();
    await userAction.不備がない形でデータの入力と登録を行う();
    await userAction.ユーザー一覧ページのデータの件数が増えていること();
  });
});