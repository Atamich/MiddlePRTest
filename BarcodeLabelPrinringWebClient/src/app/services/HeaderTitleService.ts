import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class HeaderTitleService {
  private resultList: BehaviorSubject<string> = new BehaviorSubject<string>("Печать этикеток");
  public resultList$: Observable<string> = this.resultList.asObservable();

  updateTitle(newTitle: string) {
    this.resultList.next(newTitle);
  }
}