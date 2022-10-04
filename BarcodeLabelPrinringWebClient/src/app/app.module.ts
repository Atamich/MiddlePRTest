import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LayoutModule } from '@angular/cdk/layout';

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

const customComponents = [
  FileUploadComponent
];

@NgModule({
  declarations: [
    AppComponent,
    customComponents,
    HeaderComponent,
    FooterComponent,
    StartComponent,
    SettingsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LayoutModule,
    BrowserAnimationsModule,
    materialModules,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
