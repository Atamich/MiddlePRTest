import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-error-component',
  templateUrl: './error-component.component.html',
  styleUrls: ['./error-component.component.scss']
})
export class ErrorComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: { message: string | Blob }) {
    if (this.data.message instanceof Blob) {
      this.data.message = this.blobToString(this.data.message);
    }

    var error = "error";
    try {
      error = JSON.parse(this.data.message);
    }
    catch (e) {
      error = this.data.message;
    }
    console.log(error);

    this.data.message = "Произошла ошибка. Проверьте файл и обратитесь к администратору/разработчику\nДополнительное сообщение об ошибке в консоли браузера (F12)";
  }

  blobToString(b: Blob) {
    var u, x;
    u = URL.createObjectURL(b);
    x = new XMLHttpRequest();
    x.open('GET', u, false); // although sync, you're not fetching over internet
    x.send();
    URL.revokeObjectURL(u);
    return x.responseText;
  }
}  
