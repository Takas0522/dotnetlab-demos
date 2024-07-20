import { Injectable } from "@angular/core";
import { AuthService } from "../services/auth.service";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable, mergeMap } from "rxjs";
import { environment } from "../../environments/environment";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(
        private readonly authService: AuthService
    ) {}
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return this.authService.aquireToken$(environment.authScopes).pipe(
            mergeMap(token => {
                console.log(token)
                const authReq = req.clone(
                    { headers: req.headers.set('Authorization', `Bearer ${token}`) }
                );
                console.log(authReq);
                return next.handle(authReq);
            })
        );
    }
}