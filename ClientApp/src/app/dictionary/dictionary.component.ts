import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Translation } from '../models/transation';
import 'rxjs/add/operator/debounceTime';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-dictionary',
  templateUrl: './dictionary.component.html',
  styleUrls: ['./dictionary.component.scss']
})
export class DictionaryComponent {
  public queryControl = new FormControl();
  public translations: Translation[] = [];

  public removeTranslation: Function;
  public file: any;

  private baseUrl: string;
  private http: HttpClient;

  public filter = '';
  public loading = true;

  public query: string;
  public branch: string;

  public options: string[] = [
    'Desktop.App',
    'Portal',
    'Sign',
    'Desktop',
    'OldDesktop',
    'Web'
  ];

  constructor(
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.baseUrl = baseUrl;
    this.http = http;
  }

  // tslint:disable-next-line:use-life-cycle-interface
  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.query = params['env'];
      this.branch = params['branch'];

      console.log(this.query);
      console.log(this.branch);
      if (this.query) {
        this.getQueryResult(this.query);
        return;
      }

      this.getAllTranslations(this.branch);
    });
  }

  public getQueryResult(query) {
    this.loading = true;
    const options = {};
    if (Boolean(this.branch)) {
      options['headers'] = {
        'X-esignatur-branch': this.branch
      };
    }
    this.http
      .get<Translation[]>(
        `${this.baseUrl}api/translation/query/${Boolean(query) ? query : ''}`,
        options
      )
      .subscribe(
        (result: Translation[]) => {
          this.translations = result;
          this.loading = false;
        },
        error => {
          console.error(error);
          this.loading = false;
        }
      );
  }

  getAllTranslations(branch?: string) {
    this.loading = true;
    const options = {};
    if (Boolean(this.branch)) {
      options['headers'] = {
        'X-esignatur-branch': this.branch
      };
    }
    this.http
      .get<Translation[]>(`${this.baseUrl}api/translation`, options)
      .subscribe(
        (result: Translation[]) => {
          this.translations = result;
          this.loading = false;
        },
        error => {
          console.error(error);
          this.loading = false;
        }
      );
  }

  uploadJson(e) {
    this.file = e.target.files[0];
  }

  uploadDocument() {
    this.loading = true;
    const fileReader = new FileReader();
    fileReader.onload = e => {
      const fileResult = JSON.parse(fileReader.result as string);
      this.http
        .post<any>(
          `${this.baseUrl}api/translation/updatefromOldjson`,
          fileResult
        )
        .subscribe(
          result => {
            if (this.query) {
              this.getQueryResult(this.query);
              return;
            }

            this.getAllTranslations(this.branch);
            this.loading = false;
          },
          error => {
            console.error(error);
            this.loading = false;
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
