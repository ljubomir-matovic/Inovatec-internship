import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-change-language',
  templateUrl: './change-language.component.html',
  styleUrls: ['./change-language.component.scss']
})
export class ChangeLanguageComponent implements OnInit {

  languages!:any[];
  selectedLanguage!:string;

  constructor(private translateService:TranslateService, private primeNgConfig:PrimeNGConfig) {}

  ngOnInit(): void {
    this.setLanguage();
    this.languages = [{label: 'EN', value: 'en'}, {label: 'SR', value: 'sr'}];;
  }

  changeLanguage(event:any): void {
    this.translateService.use(event.value);
    localStorage.setItem("language",this.selectedLanguage);
    this.changePrimeNGLanguage();
  }

  private setLanguage(): void {
    let language = localStorage.getItem("language");

    if(language != "en" && language != "sr") {
      language = "en";
    }

    this.selectedLanguage = language;
    this.translateService.use(this.selectedLanguage);
    this.changePrimeNGLanguage();
  }

  changePrimeNGLanguage(): void {
    this.translateService.get("PrimeNG").subscribe(translation => {
      this.primeNgConfig.setTranslation(translation)
    });
  }
}
