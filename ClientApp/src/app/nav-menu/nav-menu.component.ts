import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { finalize } from 'rxjs/operators'

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {

  isExpanded = false;
  http: HttpClient;
  baseUrl: string;
  public isSend: boolean = false;
  public isSending: boolean = false;

  constructor(
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
  ) {
    this.http = http;
    this.baseUrl = baseUrl;
  }

  public menuitems = [
    {
      title: 'Guide',
      baseUrl: '/',
      options: {},
      icon: 'fas fa-question',
      subItems: []
    },
    {
      title: 'Translations',
      baseUrl: null,
      options: {},
      icon: 'fas fa-list-ul',
      subItems: [
        {
          title: 'All translations',
          baseUrl: '/dictionary',
          options: {},
          icon: 'fas fa-atlas',
          subItems: []
        },
        {
          title: 'New desktop',
          baseUrl: '/dictionary',
          options: { env: 'desktop', branch: null },
          icon: 'fas fa-desktop',
          subItems: []
        },
        {
          title: 'Old desktop',
          baseUrl: '/dictionary',
          options: { env: 'OldDesktop', branch: null },
          icon: 'fas fa-laptop',
          subItems: []
        },
        {
          title: 'Sign',
          baseUrl: '/dictionary',
          options: { env: 'sign', branch: null },
          icon: 'fas fa-file-signature',
          subItems: []
        },
        {
          title: 'Web',
          baseUrl: '/dictionary',
          options: { env: 'web', branch: null },
          icon: 'fab fa-angular',
          subItems: []
        },
        {
          title: 'Portal',
          baseUrl: '/dictionary',
          options: { env: 'portal', branch: null },
          icon: 'fas fa-toolbox',
          subItems: []
        },
        {
          title: 'Api',
          baseUrl: '/dictionary',
          options: { env: 'Api', branch: null },
          icon: 'fas fa-user-secret ',
          subItems: []
        }
        /*   {
          title: 'Google Translate',
          baseUrl: '/dictionary',
          options: { env: 'googletranslate', branch: null },
          icon: 'fas fa-language',
          subItems: []
        } */
      ]
    }
  ];

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  sendNotification() {
    if (this.isSending) return;
    this.isSending = true;
    this.http
      .get(this.baseUrl + 'api/translation/notify')
      .subscribe(
        (result: any) => {
          this.isSend = true;
          setTimeout(() => {
            this.isSend = false;
            this.isSending = false;
          }, 3500);
        },
        error => {
          console.error(error);
          this.isSending = false;
        }
      );
  }
}
