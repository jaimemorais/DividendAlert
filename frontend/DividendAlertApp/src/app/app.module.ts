import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ShowDividendsComponent } from './show-dividends/show-dividends.component';
import { SelectStockComponent } from './select-stock/select-stock.component';

@NgModule({
  declarations: [
    AppComponent,
    ShowDividendsComponent,
    SelectStockComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
