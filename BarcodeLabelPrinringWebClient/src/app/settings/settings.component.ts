import { Component, OnInit } from '@angular/core';
import { ILabelPrintSettings, SettingsService, } from '../services/setting.service';
import {HeaderTitleService} from '../services/HeaderTitleService';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  underInvoiceText = '1. Гарантийный срок 12 месяцев.\r\n'+
  '\r\n2. Если Вам не подошел товар, и он не был в\r\n'+
  'использовании, Вы можете вернуть его в\r\n'+
  'течение 90 дней по любой причине.\r\n'+
  '\r\n3. Товар возвращается в оригинальной упаковке,\r\n'+
  'разобранный, без дефектов.' +
  '\r\n4. Возврат платный. Возврат предварительно\r\n'+
  'согласовывается после оформления заявки на\r\n'+
  'возврат и оплаты забора товара.';

  private headerTitleService: HeaderTitleService;

    constructor(private service: SettingsService, private _headerTitleService: HeaderTitleService) { 
    let settings = service.getSettings();
    this.underInvoiceText = settings.underInvoiceText;
    this.headerTitleService = _headerTitleService;
    this.headerTitleService.updateTitle("Печать этикеток - Настройки");
  }

  ngOnInit(): void {
  }

  onChaged(event:any){
    let settings = {} as ILabelPrintSettings;
    settings.underInvoiceText = event.target.value;
    this.service.setSettings(settings)
  }

  goBack (){
    this.headerTitleService.updateTitle("Печать этикеток");
  }
}
