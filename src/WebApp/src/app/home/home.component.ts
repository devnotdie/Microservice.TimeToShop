import {Component, OnInit} from '@angular/core';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {environment} from "../../environments/environment";
import {forkJoin} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  isAuthenticated = false;

  constructor(public oidcSecurityService: OidcSecurityService) {}

  ngOnInit() {
    this.oidcSecurityService.isAuthenticated$.subscribe(
      ({ isAuthenticated }) => {
        this.isAuthenticated = isAuthenticated;
      }
    );
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  register() {
    const preloadAuthWellKnownDocument$ = this.oidcSecurityService.preloadAuthWellKnownDocument();
    const getAuthorizeUrl$ = this.oidcSecurityService.getAuthorizeUrl();

    forkJoin([preloadAuthWellKnownDocument$, getAuthorizeUrl$])
      .subscribe(([_, authorizeUrl])=> {
        if(authorizeUrl){
          window.location.href = environment.identityServerUrl + '/account/register?returnUrl=' + encodeURIComponent(authorizeUrl);
        }
      })
  }

  logout() {
    this.oidcSecurityService
      .logoff()
      .subscribe((result) => console.log(result));
  }
}
