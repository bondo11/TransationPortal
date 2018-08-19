import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import {
  MatAutocompleteModule,
  MatFormFieldModule,
  MatOptionModule,
  MatInputModule
} from "@angular/material";

import { AppComponent } from "./app.component";
import { NavMenuComponent } from "./nav-menu/nav-menu.component";
import { HomeComponent } from "./home/home.component";
import { DictionaryComponent } from "./dictionary/dictionary.component";
import { TranslationInputComponent } from "./dictionary/translation-input.component";
import { TranslationAddComponent } from "./dictionary/translation-add.component";
import { CallbackPipe } from "./dictionary/filter/callback.pipe.ts";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    DictionaryComponent,
    TranslationInputComponent,
    TranslationAddComponent,
    CallbackPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatOptionModule,
    MatInputModule,
    RouterModule.forRoot([
      { path: "", component: HomeComponent, pathMatch: "full" },
      { path: "dictionary", component: DictionaryComponent }
    ]),
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
