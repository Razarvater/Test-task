import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WordOfDayComponent } from './components/word-of-day.component';

const routes: Routes = [
    { 
        path: '',
        component: WordOfDayComponent
    },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class WordRoutingModule { }
