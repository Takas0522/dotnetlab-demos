import { Injectable } from "@angular/core";
import { Auth, authClient } from "../auth/auth";
import { Observable, from, of } from "rxjs";

@Injectable({ providedIn: 'root'})
export class AuthService {

    #authClient: Auth;
    constructor() {
        this.#authClient = authClient();
    }

    aquireToken$(scopes: string[]): Observable<string> {
        return from(this.#authClient.aquireToken(scopes));
    }
}