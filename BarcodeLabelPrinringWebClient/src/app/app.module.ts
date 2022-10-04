import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LayoutModule } from '@angular/cdk/layout';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatToolbarModule } from '@angular/material/toolbar'
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';

const materialModules = [
  MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, MatToolbarModule, MatGridListModule
];

import { FileUploadComponent } from './file-upload/file-upload.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { StartComponent } from './start/start.component';
import { SettingsComponent } from './settings/settings.component';
import { FinalComponent } from './final/final.component';
import { WinAuthInterceptor } from './services/WinAuthService';

const customComponents = [
  FileUploadComponent,FooterComponent,HeaderComponent,SettingsComponent,StartComponent
];

@NgModule({
  declarations: [
    AppComponent,
    customComponents,
    FinalComponent
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
  bootstrap: [AppComponent]
})
export class AppModule { }
