import { NgModule } from '@angular/core';
import { WordOfDayComponent } from './components/word-of-day.component';
import { WordRoutingModule } from './word-routing.module';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { MatOptionModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
@NgModule({
  declarations: [
    WordOfDayComponent
  ],
  imports: [
    CommonModule,
    WordRoutingModule,

    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatOptionModule,

    MatButtonModule,
    MatListModule
  ],
  providers: []
})
export class WordModule { }
