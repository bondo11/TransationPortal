import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Translation } from '../models/transation';

@Component({
  // tslint:disable-next-line:component-selector
  selector: '[translation-row]',
  template: `
    <input type='hidden' [(ngModel)]='translation.id' />
    <td>
      <input type='text' cols="30" rows="4" [(ngModel)]='translation.key' />
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
    <button (click)="updateTranslation()"><i class="far fa-save"></i> Update</button>
    <button (click)="deleteTranslation()"><i class="fas fa-trash-alt"></i> Delete</button><br>
    <button (click)="googleTranslate()"><i class="fas fa-language"></i> GoogleTranslate</button>
    </td>`
})
export class TranslationInputComponent {
  // tslint:disable-next-line:no-input-rename
  @Input('translation')
  translation: Translation;

  // tslint:disable-next-line:no-input-rename
  @Input('translations')
  translations: Translation[];

  http: HttpClient;
  baseUrl: string;

  error: boolean;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.error = false;
  }

  updateTranslation() {
    this.http
      .patch<any>(this.baseUrl + 'api/translation/update', this.translation)
      .subscribe(
        result => {
          this.error = false;
        },
        error => {
          this.error = true;
          console.error(error);
        }
      );
  }

  googleTranslate() {
    this.http
      .post<Translation>(
        this.baseUrl + 'api/translation/googletranslatemissing',
        this.translation
      )
      .subscribe(
        (result: Translation) => {
          this.translation = result;
        },
        error => {
          this.error = true;
          console.error(error);
        }
      );
  }

  deleteTranslation() {
    this.http
      .delete<any>(
        `${this.baseUrl}api/translation/delete/${this.translation.id}`
      )
      .subscribe(
        result => {
          this.delete();
        },
        error => {
          console.error(error);
        }
      );
  }

  delete() {
    const index = this.translations.indexOf(this.translation);
    if (index > -1) {
      this.translations.splice(index, 1);
    }
  }
}
