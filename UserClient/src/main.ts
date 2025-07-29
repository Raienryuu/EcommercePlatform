import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/core/app.component';
import { NgxMatInputTelComponent } from 'ngx-mat-input-tel';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { NgxStripeModule } from 'ngx-stripe';
import { MatStepperModule } from '@angular/material/stepper';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatTreeModule } from '@angular/material/tree';
import { NgOptimizedImage } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { provideAnimations } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app/app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { authInterceptor } from './app/interceptors/auth.interceptor';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { MatFormFieldModule } from '@angular/material/form-field';

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      BrowserModule,
      FormsModule,
      AppRoutingModule,
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
      MatFormFieldModule,
    ),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
  ],
}).catch((err) => console.error(err));
