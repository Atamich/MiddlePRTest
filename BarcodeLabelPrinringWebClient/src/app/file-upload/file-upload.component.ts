import { Component, OnInit, Input, Output, ViewChild, ElementRef } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { filter } from 'rxjs';
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { AdditionalFormData, FileUploadService } from '../services/file-upload.service';
import { SettingsService } from '../services/setting.service';

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
  afterDownload: boolean = true;
  @Input() fileName: string = ""

  path = '';
  mergeFiles = false;
  @ViewChild('fileInput')
  fileInput: ElementRef;

  file: File;

  InProgress: boolean = false;

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
    this.InProgress = true;
    let settings = this.settingsService.getSettings();
    let underInvoiceTextFormData = {} as AdditionalFormData;
    underInvoiceTextFormData.name = "underInvoiceText"
    underInvoiceTextFormData.value = settings.underInvoiceText;

    let mergeFilesFormData = {} as AdditionalFormData;
    mergeFilesFormData.name = "mergeFiles";
    mergeFilesFormData.value = String(this.mergeFiles);


    var promise = this.service.uploadFile(this.file, this.route, this.afterDownload, this.fileName, [underInvoiceTextFormData, mergeFilesFormData]);

    promise.catch(() => {
      this.InProgress = false;
    });

    promise.then(()=>{
      this.router.navigateByUrl("final");
    });
    
    

  }

  constructor(public service: FileUploadService, private settingsService: SettingsService, private router: Router, private dialog: MatDialog) { }

  async ngOnInit() {

  }

}