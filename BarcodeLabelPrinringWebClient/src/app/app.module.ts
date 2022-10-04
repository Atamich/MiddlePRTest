import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LayoutModule } from '@angular/cdk/layout';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';

const materialModules = [
  MatButtonModule, MatCheckboxModule, MatIconModule, MatFormFieldModule, MatInputModule, MatToolbarModule, MatGridListModule,MatDialogModule,MatProgressSpinnerModule, ReactiveFormsModule
];

import { FileUploadComponent } from './file-upload/file-upload.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { StartComponent } from './start/start.component';
import { SettingsComponent } from './settings/settings.component';
import { FinalComponent } from './final/final.component';
import { WinAuthInterceptor } from './services/WinAuthService';
import { ErrorComponent } from './error-component/error-component.component';

const customComponents = [
  FileUploadComponent,FooterComponent,HeaderComponent,SettingsComponent,StartComponent
];

@NgModule({
  declarations: [
    AppComponent,
    customComponents,
    FinalComponent,
    ErrorComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    LayoutModule,
    HttpClientModule,
    BrowserAnimationsModule,
    materialModules,
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: WinAuthInterceptor,
    multi: true
}
],
  bootstrap: [AppComponent],
  entryComponents: [ErrorComponent]
})
export class AppModule { }
