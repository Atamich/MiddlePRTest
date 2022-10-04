import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { StartComponent } from './start/start.component';
import { SettingsComponent } from './settings/settings.component';
import { FinalComponent } from './final/final.component';

const routes: Routes = [
  {path: 'upload', component: StartComponent},
  {path: 'settings', component: SettingsComponent},
  {path: 'final', component: FinalComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
