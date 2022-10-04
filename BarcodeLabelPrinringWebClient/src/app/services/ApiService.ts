import { HttpClient, HttpEventType, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { LiteEvent } from './LiteEvent';
import { ErrorComponent } from '../error-component/error-component.component';
import { MatDialog } from '@angular/material/dialog';

@Injectable({
  providedIn: "root"
})
export class ApiService {
  readonly baseAddress = environment.apiUrl;
  token: string = "";

  authError: LiteEvent<ApiService> = new LiteEvent<ApiService>();

  constructor(private http: HttpClient, private dialog: MatDialog) { }

  setToken(token: string) {
    this.token = token;
  }

  get<T>(url: string): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      this.http.get<T>(this.baseAddress + url, this.getOptions()).subscribe(
        response => resolve(response),
        error => this.errorHandler(error, reject)
      );
    }).catch();
  }

  post<T>(url: string, body: any): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      this.http.post<T>(this.baseAddress + url, body).subscribe(
        response => resolve(response),
        error => this.errorHandler(error, reject)
      );
    }).catch();
  }

  async uploadAndDownload<T>(url: string, body: any, name: string) {
    return new Promise<T | null>((resolve, reject) => {
      this.http.post(this.baseAddress + url, body, {
        responseType: "blob"
      }).subscribe(
        response => {
          resolve(null);
          this.downloadFile(response, name);
        },
        error => {
          this.dialog.open(ErrorComponent, { data: { message: error.error } });
          reject(error);
        }
      );
    }).catch();
  }

  async download<T>(url: string, name: string) {
    return new Promise<T>((resolve, reject) => {
      this.http.get(this.baseAddress + url, {
        responseType: "blob"
      }).subscribe(
        response => {
          this.downloadFile(response, name);
        },
        error => this.errorHandler(error, reject)
      );
    }).catch();
  }

  private downloadFile(data: any, name: string) {
    const downloadedFile = new Blob([data], { type: data.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = name;
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  }

  getOptions() {
    return {
      headers: new HttpHeaders().set("Authorization", "Bearer " + this.token)
    };
  }

  put<T>(url: string, body: any): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      this.http.put<T>(this.baseAddress + url, body, this.getOptions())
        .subscribe(
          response => resolve(response),
          error => this.errorHandler(error, reject)
        );
    });
  }

  delete<T = null>(url: string): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      this.http.delete<T>(this.baseAddress + url, this.getOptions())
        .subscribe(
          response => resolve(response),
          error => this.errorHandler(error, reject)
        );
    });
  }

  private errorHandler(error: any, reject: (reason?: any) => void) {
    if (error.status == 401) {
      this.authError.trigger(this);
    }
    reject(error);
  }
}
