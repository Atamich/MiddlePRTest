import { Component, OnInit } from '@angular/core';
import { FileUploadComponent } from '../file-upload/file-upload.component';
@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.scss']
})
export class StartComponent implements OnInit {

  constructor() { }
  
  ngOnInit(): void {
  }
  fileUploaded(event : Event){

  }
}
