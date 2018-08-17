import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Translation } from '../models/transation';

@Component({
  selector: 'app-dictionary',
  templateUrl: './dictionary.component.html'
})
export class DictionaryComponent {
  public translations: Translation[] = [];

  public removeTranslation: Function;
  public file: any;

  private baseUrl: string;
  private http: HttpClient;

  public filter = '';
  public branch = '';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.http = http;
    this.getAllTranslations();
  }
  getAllTranslations() {
    this.http.get<Translation[]>(this.baseUrl + 'api/translation').subscribe(
      (result: Translation[]) => {
        this.translations = result;
      },
      error => {
        console.error(error);
      }
    );
  }
  uploadJson(e) {
    this.file = e.target.files[0];
  }

  uploadDocument() {
    const fileReader = new FileReader();
    fileReader.onload = e => {
      const fileResult = JSON.parse(fileReader.result as string);
      this.http
        .post<any>(
          this.baseUrl + 'api/translation/updatefromOldjson',
          fileResult
        )
        .subscribe(
          result => {
            this.getAllTranslations();
          },
          error => {
            console.error(error);
          }
        );
    };

    fileReader.readAsText(this.file);
  }

  filteredTranslations() {
    return this.translations.filter(x =>
      this.filterTranslations(x, this.filter)
    );
  }

  filterTranslations(translation: Translation, filter: string) {
    if (!translation) {
      return false;
    }
    if (!filter) {
      return true;
    }
    for (const key in translation) {
      if (key === 'id') {
        continue;
      }
      if (translation.hasOwnProperty(key)) {
        if (typeof translation[key] !== 'string') {
          continue;
        }
        if (translation[key].indexOf(filter) !== -1) {
          return true;
        }
      }
    }
    return false;
  }
}
