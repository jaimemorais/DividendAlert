import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SelectStockComponent } from '../app/components/select-stock/select-stock.component';
import { AppComponent } from '../app/app.component';
import { MyNewsComponent } from '../app/components/my-news/my-news.component';


const routes: Routes = [
  { path: '.', component: AppComponent },
  { path: 'my-news', component: MyNewsComponent },
  { path: 'select-stocks', component: SelectStockComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
