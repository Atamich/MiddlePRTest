import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResultModel } from '../models/ResultModel';
import { ApiService } from "./ApiService";
import { CookieService } from 'ngx-cookie-service';

@Injectable({
    providedIn: 'root'
})
export class SettingsService {

    constructor(private api: ApiService,private cookieService:CookieService ) { }

    getSettings() : ILabelPrintSettings{
        var settings = {} as ILabelPrintSettings;
        settings.underInvoiceText = this.cookieService.get("underInvoiceText");

        if(settings.underInvoiceText == '') settings.underInvoiceText = 
        '1. Гарантийный срок 12 месяцев.\r\n'+
        '\r\n2. Если Вам не подошел товар, и он не был в\r\n'+
        'использовании, Вы можете вернуть его в\r\n'+
        'течение 90 дней по любой причине.\r\n'+
        '\r\n3. Товар возвращается в оригинальной упаковке,\r\n'+
        'разобранный, без дефектов.' +
        '\r\n4. Возврат платный. Возврат предварительно\r\n'+
        'согласовывается после оформления заявки на\r\n'+
        'возврат и оплаты забора товара.';

        return settings;
    }

    setSettings(settings : ILabelPrintSettings)  {
        this.cookieService.set("underInvoiceText",settings.underInvoiceText);
    }



}

export interface ILabelPrintSettings{
    underInvoiceText: string;
}
