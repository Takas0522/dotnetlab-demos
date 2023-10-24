import { expect, Page } from "@playwright/test";
import 'dotenv/config'

const baseUrl = process.env.TARGET_URL ? process.env.TARGET_URL : '';

export class ToDoPageUserAction {

  constructor(
    private readonly page: Page
  ) {}

  async ToDoページにアクセスする() {
    await this.page.goto(baseUrl);
  }

  async ToDoのLoadを待つ() {
    await this.page.waitForSelector('mat-card');
  }

  async すべてのToDoを表示() {
    await this.page.getByText('すべてのToDoを表示').click();
  }

  async ToDoに指定された文字列を登録する(writeText: string) {
    await this.page.getByText('とぅーどぅー').fill(writeText);
    await this.page.getByRole('button', { name: '追加' }).click();
    await this.page.waitForSelector('mat-snack-bar-container', { state: 'hidden' });
    await this.page.waitForSelector(`text=${writeText}`);
  }

  async 指定された文字列のToDoをCompleteにする(text: string) {
    await this.page.locator('mat-card').filter({ hasText: 'check_box_outline_blank ' + text }).getByRole('button').click();
    await this.page.waitForSelector('mat-snack-bar-container', { state: 'hidden' });
    await this.page.waitForSelector(`text=${text}`, { state: 'hidden' });
  }

  async すべてのToDoを表示する() {
    await this.page.getByLabel('すべてのToDoを表示').click();
    await this.page.waitForTimeout(500);
  }
}