import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductDetailsComponent } from './product-details.component';
import { ImageService } from 'src/app/services/imagesService/images.service';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { BrowserModule, By } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NgOptimizedImage } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatTreeModule } from '@angular/material/tree';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatStepperModule } from '@angular/material/stepper';
import { NgxStripeModule } from 'ngx-stripe';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { environment } from 'src/enviroment';

describe('ProductDetailsComponent', () => {
  let component: ProductDetailsComponent;
  let fixture: ComponentFixture<ProductDetailsComponent>;
  let native: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        BrowserModule,
        FormsModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        MatButtonModule,
        MatInputModule,
        MatGridListModule,
        MatSelectModule,
        ReactiveFormsModule,
        MatIconModule,
        MatSidenavModule,
        NgOptimizedImage,
        MatTreeModule,
        MatListModule,
        MatProgressSpinnerModule,
        MatPaginatorModule,
        MatStepperModule,
        NgxStripeModule.forRoot(),
        MatCardModule,
        MatRadioModule,
        MatCheckboxModule,
        MatDialogModule,
        ProductDetailsComponent,
      ],
      providers: [
        ImageService,
        provideRouter([
          { path: '', pathMatch: 'full', redirectTo: 'details/1' },
          { path: 'details/:id', component: ProductDetailsComponent },
        ]),
        provideHttpClientTesting(),
        provideHttpClient(),
      ],
    }).compileComponents();

    environment.sampleData = true;
    fixture = TestBed.createComponent(ProductDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    native = fixture.nativeElement;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call addToCart', () => {
    const button = fixture.debugElement.query(By.css('[name=cartbtn]'));
    const cartButtonCall = spyOn(component, 'AddToCart');
    (button.nativeElement as HTMLButtonElement).click();
    expect(cartButtonCall)
      .withContext('add to cart button function was not called')
      .toHaveBeenCalled();
  });

  it('should update ngSrc of main image', async () => {
    const images = native.querySelectorAll('img');
    let imageMain: HTMLImageElement = null!;
    let previewImage: HTMLImageElement = null!;
    images.forEach((element) => {
      if (element.parentElement?.className === 'images') {
        imageMain = element;
      } else {
        previewImage = element;
      }
    });

    const currentNgSrc = imageMain.src;
    previewImage.click();
    fixture.detectChanges();

    expect(currentNgSrc)
      .withContext("ngSrc wasn't updated")
      .not.toEqual(imageMain.src);
  });

  it('should show spinner when loading is in progress', () => {
    component.isLoading = true;
    fixture.detectChanges();

    const spinner = native.querySelector('mat-spinner');

    expect(spinner)
      .withContext('mat-spinner was not found in the DOM')
      .toBeTruthy();
  });

  it('should show error message', () => {
    component.noProductFound = true;
    component.isLoading = false;

    fixture.detectChanges();
    const error = native.getElementsByClassName('error')[0];

    expect(error).withContext("coudln't find the error message").toBeTruthy();
  });
});
