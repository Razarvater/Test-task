import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { IAddWord, IAddWordResponse } from '../models/word.interface';

@Injectable({
    providedIn: 'root'
  })
  export class WordService {


    constructor(
        public http: HttpClient,
    ){

    }
    public AddWord(data: IAddWord, callback: (response: IAddWordResponse)=>void, errorCallback: (stringError: string)=>void) {
        this.http.put<IAddWordResponse>('word', data).subscribe({
            next:(word) => {
                callback(word);
            },
            error: (err: HttpErrorResponse): any => {
                errorCallback(err.error);
            }
          });
    }
    
  }