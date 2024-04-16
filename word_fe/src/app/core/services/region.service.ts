import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { IRegion } from '../models/region.interface';

@Injectable({
    providedIn: 'root'
  })
  export class RegionService {
    public regions$ = new BehaviorSubject<IRegion[]>([]);


    constructor(
        public http: HttpClient,
    ){

    }

    public LoadProjects() {
        this.http.get<IRegion[]>('regions').subscribe({
            next:(regions) => {
              this.regions$.next(regions);
            },
            error: (err: HttpErrorResponse): any => {
                console.log(err);
            }
          });   
    }
  }