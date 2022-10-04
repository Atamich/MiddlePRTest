import { Injectable } from '@angular/core';
import { ResultModel } from '../models/ResultModel';
import { ApiService } from "./ApiService";

@Injectable({
  providedIn: 'root'
})
export class FileUploadService<T = any> {

  public returnContent: T;
  constructor(private api: ApiService) { }

  async uploadFile(file: File, route: string) {
    const formData = new FormData();
    formData.append(file.name, file);
    const result = await this.api.post<ResultModel<T>>(route, formData);
    if (!result.isSuccess) {
      //this.notifier.error(result.errorMessage); on error alarm
      return;
    }
    this.returnContent = result.content;
  }
}