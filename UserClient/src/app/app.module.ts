import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './core/app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ProductsComponent } from './components/catalog/catalog.component';

import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';
import { provideHttpClient } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSelectModule } from '@angular/material/select';
import { ReactiveFormsModule } from '@angular/forms';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NgOptimizedImage } from '@angular/common';
import { MatTreeModule } from '@angular/material/tree';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FooterComponent } from './components/footer/footer.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { CartComponent } from './components/cart/cart.component';
import { MatStepperModule } from '@angular/material/stepper';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { CountrySelectComponent } from './components/country-select/country-select.component';
import { NgxStripeModule } from 'ngx-stripe';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgxMatInputTelComponent } from 'ngx-mat-input-tel';
import { MatDialogModule } from '@angular/material/dialog';
import { AddressEditorComponent } from './components/address-editor/address-editor.component';
import { ProductDetailsComponent } from './components/product-details/product-details.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    NavbarComponent,
    ProductsComponent,
    FooterComponent,
    CartComponent,
    CheckoutComponent,
    AddressEditorComponent,
    ProductDetailsComponent,
  ],
  bootstrap: [AppComponent],
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
    NgxMatInputTelComponent,
    CountrySelectComponent,
  ],
  providers: [provideHttpClient()],
})
export class AppModule { }
