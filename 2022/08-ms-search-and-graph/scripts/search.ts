import { Auth } from "./auth";

const baseAddress = 'https://graph.microsoft.com/v1.0/search/query';

export class Search {

  constructor(
    private auth: Auth
  ) {}

  async eventSearchAsync() {
    const token = await this.aquireTokenAsync();
    const resp = await fetch(baseAddress, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(eventSearchProperty)
    });
    const respJson = await resp.json();
    console.log(respJson);
  }

  async externalItemSearchAsync() {
    const token = await this.aquireTokenAsync();
    const resp = await fetch(baseAddress, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(externalItemSearchProperty)
    });
    const respJson = await resp.json();
    console.log(respJson);
  }

  private async aquireTokenAsync() {
    return await this.auth.aquireToken([
        'ExternalItem.Read.All',
        'Calendars.Read',
        'User.Read']);
  }
}

const eventSearchProperty = {
    requests: [
        {
            entityTypes: [
                "event"
            ],
            query: {
                queryString: "SampleEvent"
            }
        }
    ]
};

const externalItemSearchProperty = {
    requests: [
        {
            entityTypes: [
                "externalItem"
            ],
            contentSources: [
                "/external/connections/samplecondevops"
            ],
            query: {
                queryString: "ユーザーストーリー"
            }
        }
    ]
};
