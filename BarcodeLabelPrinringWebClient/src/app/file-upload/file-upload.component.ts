import { Component, OnInit, Input, Output } from '@angular/core';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})

export class FileUploadComponent implements OnInit {

  @Input() width: string
  @Input() inputLabel: string
  @Input() underInputText: string
  @Input() fileFilter: string
  @Input() buttonText: string
  // @Output() dataChanged: EventEmitter<any> = new EventEmitter<any>()

  constructor() { }

  ngOnInit(): void {
  }

}