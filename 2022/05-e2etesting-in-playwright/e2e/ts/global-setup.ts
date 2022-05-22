import { chromium, FullConfig } from "@playwright/test";
import 'dotenv/config'

const baseUrl = process.env.FRONT_ENDPOINT;
const user1Address = process.env.TEST_USER_1;
const user1Pass = process.env.TEST_YSER_1_PASS;

const user2Address = process.env.TEST_USER_2;
const user2Pass = process.env.TEST_YSER_2_PASS;

async function globalSetup (config: FullConfig) {

  console.log('set up!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!')

  const browser = await chromium.launch();

  const user1Page = await browser.newPage();

  await user1Page.goto(baseUrl);
  await user1Page.waitForURL('https://devtakasb2c.b2clogin.com/**');
  await user1Page.fill('input[placeholder="Email Address"]', user1Address);
  await user1Page.fill('input[placeholder="Password"]', user1Pass);
  const user1button = user1Page.locator('button');
  await user1button.click();
  await user1Page.waitForTimeout(2500);
  await user1Page.context().storageState({ path: 'user1StorageState.json' });

  const user2Page = await browser.newPage();
  await user2Page.goto(baseUrl);
  await user2Page.waitForURL('https://devtakasb2c.b2clogin.com/**');
  await user2Page.fill('input[placeholder="Email Address"]', user2Address);
  await user2Page.fill('input[placeholder="Password"]', user2Pass);
  const user2button = user2Page.locator('button');
  await user2button.click();
  await user2Page.waitForTimeout(2500);
  await user2Page.context().storageState({ path: 'user2StorageState.json' });

  await browser.close();

  console.log('set up exit!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!')
}

export default globalSetup;