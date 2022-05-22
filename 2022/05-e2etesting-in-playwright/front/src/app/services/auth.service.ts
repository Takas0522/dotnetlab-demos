import { Injectable } from '@angular/core';
import { AccountInfo, PublicClientApplication } from '@azure/msal-browser';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private client = new PublicClientApplication(environment.msalConfig);
  private userAccount = new BehaviorSubject<AccountInfo | null>(null);
  get userName$() {
    return this.userAccount.pipe(
      map(m => {
        if (m != null && typeof(m.name) !== 'undefined') {
          return m.name
        }
        return '';
      })
    )
  }

  constructor() { }

  async clientSetup() {
    const res = await this.client.handleRedirectPromise();
    if (!res) {
      this.client.loginRedirect();
      return;
    }
    this.userAccount.next(res.account);
  }
}
