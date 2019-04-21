import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';

// angular material
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DividendAlertMaterialModule } from './modules/material.module';
import { LayoutModule } from '@angular/cdk/layout';

// components
import { AppComponent } from './app.component';
import { SelectStockComponent } from './components/select-stock/select-stock.component';
import { MyNewsComponent } from './components/my-news/my-news.component';


@NgModule({
  declarations: [
    AppComponent,
    SelectStockComponent,
    MyNewsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    DividendAlertMaterialModule,
    LayoutModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
