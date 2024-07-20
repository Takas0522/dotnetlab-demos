import { AccountInfo, PublicClientApplication } from '@azure/msal-browser';
import { environment } from '../../environments/environment';

export class Auth {

    private readonly client: PublicClientApplication;
    #authenticated = false;
    #account: undefined | AccountInfo;

    get authenticated() {
        return this.#authenticated;
    }

    constructor() {
        this.client = new PublicClientApplication(environment.authConfig);
    }

    async login() {
        await this.client.initialize();
        const response = await this.client.handleRedirectPromise();
        if (response === null) {
            this.#authenticated = false;
            await this.client.loginRedirect({scopes: environment.authScopes});
            return;
        }
        this.#authenticated = true;
        this.#account = response.account;
    }

    async aquireToken(scopes: string[]) {
        if (this.#account == null) {
            return '';
        }
        const res = await this.client.acquireTokenSilent({ account: this.#account, scopes: scopes });
        return res.accessToken;
    }
}

let authClientObj: Auth;

export const authClient = () => {
    if (!authClientObj) {
        authClientObj = new Auth();
    }
    return authClientObj;
};

export const loginChallenge  = async () => {
    const client = authClient();
    await client.login();
    console.log(client.authenticated)
    return client.authenticated;
}