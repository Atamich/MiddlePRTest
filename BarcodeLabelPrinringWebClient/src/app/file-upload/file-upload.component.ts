import { Component, OnInit, Input, Output, ViewChild, ElementRef } from '@angular/core';
import { filter } from 'rxjs';
import { FileUploadService } from '../services/file-upload.service';

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
  @Input() filter: string = "*";
  @Input() route: string;
  @Input() exampleFileRoute: string;
  afterDownload: boolean = true;
  @Input() fileName: string = ""
  @Input() exampleFileName: string = ""
  // @Output() dataChanged: EventEmitter<any> = new EventEmitter<any>()
  path = '';
  @ViewChild('fileInput')
  fileInput: ElementRef;

  file: File;

  isProcess: boolean = false;

  async onClickDownloadPreset(event: any, control: any) {
     await this.service.donwloadPreset(this.exampleFileRoute, this.exampleFileName);
  }

  onClickFileInputButton(): void {
    this.fileInput.nativeElement.click();
  }

  async onChangeFileInput(event: any, control: any) {

    const files: { [key: string]: File } = this.fileInput.nativeElement.files;
    this.file = files[0];
    this.path = this.file.name;
    this.fileInput.nativeElement.value = "";

    await this.uploadFile();

  }

  async uploadFile() {
    await this.service.uploadFile(this.file, this.route, this.afterDownload, this.fileName);
  }

  constructor(public service: FileUploadService) { }

  async ngOnInit() {

  }

}