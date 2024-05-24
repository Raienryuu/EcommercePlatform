import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CountriesNoPhonesSorted } from '../components/register/RegisterRawData';

@Component({
  selector: 'app-country-select',
  templateUrl: './country-select.component.html',
})
export class CountrySelectComponent {
  countriesNoPhonesSorted: string[] = CountriesNoPhonesSorted;
  @Input()
  country: string = null!; 
  @Output()
  countryChange = new EventEmitter<string>();

  selectionChanged(){
    this.countryChange.emit(this.country);
  }
}
