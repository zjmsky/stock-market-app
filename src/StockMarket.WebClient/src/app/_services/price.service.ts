import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import { Price } from "@root/_models/price";
import { environment } from "@env/environment";

@Injectable({ providedIn: "root" })
export class PriceService {
    private _http: HttpClient;
    private _url: string;

    constructor(http: HttpClient) {
        this._http = http;
        this._url = environment.apiUrl;
    }

    public getList(
        exchange: string,
        ticker: string,
        fromTime: Date,
        toTime: Date
    ): Observable<Price[]> {
        const fromTimeStr = fromTime.toISOString();
        const toTimeStr = toTime.toISOString();
        const getUrl = `${this._url}/prices/${exchange}:${ticker}?fromTime=${fromTimeStr}&toTime=${toTimeStr}`;
        return this._http.get<Price[]>(getUrl);
    }
}
