import { Component } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isExpanded = false;
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
}
