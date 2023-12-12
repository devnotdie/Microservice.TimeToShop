import { NgModule } from '@angular/core';
import {AuthModule, LogLevel} from 'angular-auth-oidc-client';
import {environment} from "../environments/environment.development";

@NgModule({
  imports: [
    AuthModule.forRoot({
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
    }),
  ],
  exports: [AuthModule],
})
export class AuthConfigModule {}
