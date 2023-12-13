import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {
  authInterceptor,
  LogLevel,
  provideAuth
} from "angular-auth-oidc-client";
import {environment} from "../environments/environment";
import {HttpClientModule, provideHttpClient, withInterceptors} from "@angular/common/http";

export const appConfig: ApplicationConfig = {
  providers: [
   provideHttpClient(withInterceptors([authInterceptor()])),
    provideRouter(routes),
    provideAuth({
    config: {
      authority: environment.identityServerUrl,
      redirectUrl: window.location.origin + '/signin-oidc' ,
      postLogoutRedirectUri: window.location.origin + '/home',
      clientId: 'webapp',
      scope: 'openid roles offline_access web.full',
      responseType: 'code',
      silentRenew: true,
      useRefreshToken: true,
      logLevel: LogLevel.Debug
    },
  })]
};
