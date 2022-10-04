import { Component, OnInit } from '@angular/core';
import {HeaderTitleService} from '../services/HeaderTitleService';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  title = "Печать этикеток"
  
  constructor(private headerTitleService: HeaderTitleService) { 

    this.headerTitleService.resultList$
    .subscribe((newTitle) => {
      this.title = newTitle;
    });
  }

  ngOnInit(): void {
  }

}
