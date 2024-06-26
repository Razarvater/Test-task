import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

/**
 * Prefixes all requests with `environment.serverUrl` or `environment.nodeServerUrl`.
 */
@Injectable()
export class ApiPrefixInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.indexOf('assets/') === -1 && request.url.indexOf('http') === -1) {
        request = request.clone({ url: environment.apiUrl + request.url });
    }
    return next.handle(request);
  }
}