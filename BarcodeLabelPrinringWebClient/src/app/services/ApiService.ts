import { HttpClient, HttpEventType, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { LiteEvent } from './LiteEvent';

@Injectable({
  providedIn: "root"
})
export class ApiService {
  readonly baseAddress = environment.apiUrl;
  token: string = "";

  authError: LiteEvent<ApiService> = new LiteEvent<ApiService>();

  constructor(private http: HttpClient) { }

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

  async download<T>(url: string, name: string) {
    return new Promise<T>((resolve, reject) => {
      this.http.get(this.baseAddress + url, {
        headers: new HttpHeaders().set("Authorization", "Bearer " + this.token),
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
    console.log(typeof(data), data);
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

  post<T>(url: string, body: any): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      this.http.post<T>(this.baseAddress + url, body).subscribe(
        response => resolve(response),
        error => this.errorHandler(error, reject)
      );
    }).catch();
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
