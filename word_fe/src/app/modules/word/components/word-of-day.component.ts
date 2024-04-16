import { Component } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { IRegion } from 'src/app/core/models/region.interface';
import { IAddWordResponse } from 'src/app/core/models/word.interface';
import { RegionService } from 'src/app/core/services/region.service';
import { WordService } from 'src/app/core/services/word.service';

enum WordOfDayFormFields {
    Region = 'region',
    Email = 'email',
    Value = 'value'
  }

@Component({
  selector: 'app-root',
  templateUrl: './word-of-day.component.html',
  styleUrls: ['./word-of-day.component.scss']
})
export class WordOfDayComponent {
    public regions = new BehaviorSubject<IRegion[]>([]);
    public response: BehaviorSubject<IAddWordResponse|null> = new BehaviorSubject<IAddWordResponse|null>(null);
    
    public emailErrorMessage: Boolean = false;
    public emailErrorMessageShow: Boolean = false;

    public formGroup: UntypedFormGroup;
    public formFields = WordOfDayFormFields;

    constructor(
        public formBuilder: UntypedFormBuilder,
        regionService: RegionService,
        private wordService:  WordService
    ) {
      this.formGroup = this.formBuilder.group({
        [WordOfDayFormFields.Region]: [ null,[ Validators.required, Validators.maxLength(100) ] ],
        [WordOfDayFormFields.Email]: [ '', [ Validators.required, Validators.email, Validators.maxLength(100)]  ],
        [WordOfDayFormFields.Value]: [ '', [ Validators.required, Validators.pattern('^([A-Z]|[a-z])*$')]  ],
      });

      this.formGroup.statusChanges.subscribe(status => {
        if (status === 'INVALID' && this.emailErrorMessageShow == true) {
          this.emailErrorMessage = true;
          this.emailErrorMessageShow = false;
        } else if (status === 'INVALID'){
          this.emailErrorMessage = false;
        }
      });

      regionService.LoadProjects();
      regionService.regions$.subscribe(x => this.regions.next(x));
    }


    public sendWord() {
        this.wordService.AddWord({ email: this.formGroup.value.email, region: { name: this.formGroup.value.region }, value: this.formGroup.value.value },
            (response: IAddWordResponse) => {
                debugger
                response.allRegionStats.closestWords = response.allRegionStats.closestWords.filter(x => x.value != response.allRegionStats.yourWord!.value);
                response.myRegionStats.closestWords = response.myRegionStats.closestWords.filter(x => x.value != response.myRegionStats.yourWord!.value);
                this.response.next(response);
            },
            (error: string) => {
              this.emailErrorMessageShow = true;
              this.formGroup.controls[WordOfDayFormFields.Email].setErrors({ notUnique: true });
            }
        );
    }

    public getFormattedStats(value: string|undefined, count: number|undefined){
      return `\"${value}\" was written by ${count} ${(count == 1 ? 'person' : 'people')} `
    }
}
