import {Component, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {OidcSecurityService} from "angular-auth-oidc-client";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'WebApp';

  constructor(private oidcSecurityService: OidcSecurityService) {}

  ngOnInit(): void {
   this.oidcSecurityService.checkAuth().subscribe((response)=> {
      console.log('isAuthenticated: ' + response.isAuthenticated);
    })
  }

}
