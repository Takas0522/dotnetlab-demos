import { expect, Page } from "@playwright/test";
import 'dotenv/config'

const baseUrl = process.env.FRONT_ENDPOINT;

export class UserTestActions {

  constructor(
    private page:Page
  ) {
  }

  async ページの状態が完了するまで待機() {
    await this.page.goto(`${baseUrl}/users`);
    await this.page.waitForSelector('.user-name');
  }

  async ユーザー一覧ページに移動する() {
    await this.page.goto(`${baseUrl}/users`);
    await this.page.waitForSelector('.user-name');
    await this.page.waitForSelector(`mat-list-item`);
  }

  async 一覧ページから新規登録ページに移動() {
    this.page.locator('button', { hasText: 'Add Item' }).click();
  }

  async 不備がある形でデータの入力() {
    await this.page.waitForSelector('input[data-placeholder="ID"]');
    const idInput = this.page.locator('input[data-placeholder="ID"]');
    await idInput.focus();
    await idInput.fill('');
    await idInput.evaluate(e => e.blur());
  }

  async エラーが表示されデータの登録が行えないこと() {
    // 設定以外に自前でスクショをとること然可能
    await this.page.screenshot({ path: 'reports/images/scs.png' });
    const errmsg = this.page.locator('mat-error', { hasText: '必須です' });
    expect(errmsg).not.toBeEmpty();

    const regButton = this.page.locator('button', { hasText: 'データ登録' });
    expect(regButton).toBeDisabled();
  }

  private listCount = 0;

  async ユーザー一覧ページに移動しデータ件数を確認する() {
    await this.page.goto(`${baseUrl}/users`);
    await this.page.waitForSelector('.user-name');
    await this.page.waitForSelector(`mat-list-item`);
    const loc = this.page.locator('mat-list-item');
    this.listCount = await loc.count();
  }

  async 不備がない形でデータの入力と登録を行う() {
    await this.page.waitForSelector('input[data-placeholder="ID"]');
    const idInput = this.page.locator('input[data-placeholder="ID"]');
    const now = Date.now().toString();
    await idInput.fill(now);
    const nameInput = this.page.locator('input[data-placeholder="名前"]');
    await nameInput.fill(now);
    const submitbutton = this.page.locator('button', { hasText: 'データ登録' });
    await submitbutton.click();
  }

  async ユーザー一覧ページのデータの件数が増えていること() {
    await this.page.goto(`${baseUrl}/users`);
    await this.page.waitForSelector('.user-name');
    await this.page.waitForSelector(`mat-list-item`);
    const loc = this.page.locator('mat-list-item');
    const count = await loc.count();
    expect(count).toBeGreaterThan(this.listCount);
  }

}