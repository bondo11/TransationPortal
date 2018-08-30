import { environment } from './../../environments/environment.prod';
import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Translation } from '../models/transation';
import { ActivatedRoute } from '@angular/router';
import TranslationsEnvironment from '../models/TranslationsEnvironment';

@Component({
  // tslint:disable-next-line:component-selector
  selector: '[translation-add]',
  template: `
    <tr>
      <td>
        <input type='text' [(ngModel)]='translation.key' placeholder="Key for translation"/>
      </td>
      <td>
        <textarea cols="30" rows="4" [(ngModel)]='translation.da' placeholder="Dansk oversættelse"></textarea>
      </td>
      <td>
        <textarea cols="30" rows="4" [(ngModel)]='translation.en' placeholder="Dansk oversættelse"></textarea>
      </td>
      <td>
        <textarea cols="30" rows="4" [(ngModel)]='translation.sv' placeholder="Svensk oversættelse"></textarea>
      </td>
      <td>
        <textarea cols="30" rows="4" [(ngModel)]='translation.nb' placeholder="Norsk oversættelse"></textarea>
      </td>
      <td>
      <button (click)="addTranslation()"><i class="fas fa-plus"></i> Add</button>
      </td>
    </tr>`
})
export class TranslationAddComponent {
  // tslint:disable-next-line:no-input-rename
  @Input('translations')
  translations: Translation[];

  translation: Translation = {
    id: '',
    key: '',
    da: '',
    en: '',
    sv: '',
    nb: '',
    Branch: ''
  };

  env: TranslationsEnvironment;
  branch: string;

  http: HttpClient;
  baseUrl: string;
  route: ActivatedRoute;

  error: boolean;

  constructor(
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    route: ActivatedRoute
  ) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.error = false;
    this.route = route;
    this.route.queryParams.subscribe(params => {
      const paramEnv: string = params['env'];
      this.translation.environment = TranslationsEnvironment[paramEnv];
      this.branch = params['branch'];
    });
  }

  // tslint:disable-next-line:use-life-cycle-interface
  ngOnInit() {
    this.translation.Branch = this.branch;
    this.translation.environment = this.env;
  }

  addTranslation() {
    this.http
      .post<Translation>(this.baseUrl + 'api/translation/add', this.translation)
      .subscribe(
        (result: Translation) => {
          this.translations.unshift(result);
          this.reset();
          this.error = false;
        },
        error => {
          this.error = true;
          console.error(error);
        }
      );
  }

  reset() {
    this.translation.id = '';
    this.translation.key = '';
    this.translation.da = '';
    this.translation.en = '';
    this.translation.sv = '';
    this.translation.nb = '';
    this.translation.Branch = this.branch;
    this.translation.environment = this.env;
  }
}
