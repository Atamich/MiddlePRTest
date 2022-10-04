import { Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'BarcodeLabelPrinringWebClient';

	constructor(private router: Router, private activateRoute: ActivatedRoute) {
		router.navigateByUrl("upload");
	}
}
