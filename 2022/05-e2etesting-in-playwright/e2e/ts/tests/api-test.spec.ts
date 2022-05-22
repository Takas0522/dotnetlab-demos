import { expect, test } from '@playwright/test'
import 'dotenv/config'

const baseUrl = process.env.API_ENDPOINT;

test.describe('User APIのテスト', () => {

  test('GETで複数のユーザーデータが返却されること', async ({ request }) => {
    const res = await request.get(`${baseUrl}/api/user`);
    expect(res.ok()).toBeTruthy();
    const data = await res.json();
    expect(data.length).toBeGreaterThan(1);
  });

  test('GET/{id}で単一のユーザーデータが返却されること', async ({ request }) => {
    const res = await request.get(`${baseUrl}/api/user/1`);
    expect(res.ok()).toBeTruthy();
    const data = await res.json();
    expect(data['id']).toEqual(1);
  });

  test('GET/{id}で存在しないidが指定された場合は404が返却されること', async ({ request }) => {
    const res = await request.get(`${baseUrl}/api/user/404`);
    expect(res.status()).toEqual(404);
  });

  test('POSTでユーザーデータを更新できること', async ({ request }) => {
    const res = await request.post(`${baseUrl}/api/user`, {
      data: {
        id: 2,
        email: 'fuge@example.com',
        name: 'fuge',
        role: 1
      }
    });
    expect(res.ok()).toBeTruthy();

    const getres = await request.get(`${baseUrl}/api/user/2`);
    const data = await getres.json();
    expect(data['name']).toEqual('fuge');
  });

  test('POSTでユーザーデータを追加できること', async ({ request }) => {
    const res = await request.post(`${baseUrl}/api/user`, {
      data: {
        id: 5,
        email: 'adduser@example.com',
        name: 'adduser',
        role: 0
      }
    });
    expect(res.ok()).toBeTruthy();

    const getres = await request.get(`${baseUrl}/api/user/5`);
    const data = await getres.json();
    expect(data['name']).toEqual('adduser');
  });

  test('DELETEでユーザーデータを削除できること', async ({ request }) => {
    const res = await request.delete(`${baseUrl}/api/user/5`);
    expect(res.ok()).toBeTruthy();

    const getres = await request.get(`${baseUrl}/api/user/5`);
    expect(getres.status()).toEqual(404);
  });
});