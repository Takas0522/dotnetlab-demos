import { PublicClientApplication, AccountInfo } from '@azure/msal-browser';
import { authEnv } from '../environments/environment.using';

export class Auth {
  account: AccountInfo | null = null;
  private client: PublicClientApplication = new PublicClientApplication(authEnv);

  async loginRedirect(): Promise<void> {
    const res = await this.client.handleRedirectPromise();
    if (res) {
      console.log(res);
      this.account = res.account;
      return;
    }
    this.client.loginRedirect({ scopes: [
      'ExternalItem.Read.All',
      'Calendars.Read',
      'User.Read'
    ] });
  }

  async aquireToken(scopes: string[]): Promise<string> {
    if (!this.account) {
      return '';
    }
    const res = await this.client.acquireTokenSilent({ account: this.account, scopes: scopes }).then((r: any) => {
        if (r) {
            return r.accessToken;
        } else {
            return '';
        }
    });
    return res;
  }
}