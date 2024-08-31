import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { mockUsers } from "../../mock/user.mock";

@Injectable({
    providedIn: 'root'
})
export class MockInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.url.includes('users') && req.method === 'GET') {
            return of(new HttpResponse({ status: 200, body: mockUsers }));
        }
        if (req.url.includes('user') && req.method === 'GET') {
            const userId = req.url.split('/').pop();
            if (userId != null && !isNaN(+userId)) {
                const user = mockUsers.find(u => u.id === +userId);
                if (user != null) {
                    return of(new HttpResponse({ status: 200, body: user}));
                }
            }
        }
        if (req.url.includes('user') && req.method === 'POST') {
            const userId = req.body.id;
            const user = mockUsers.find(u => u.id === userId);
            if (user != null) {
                user.name = req.body.name;
                return of(new HttpResponse({ status: 204, body: null}));
            }
            mockUsers.push({ id: userId, name: req.body.name });
            return of(new HttpResponse({ status: 204, body: null}));
        }
        return next.handle(req);
    }

}
