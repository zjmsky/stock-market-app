import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import { Exchange } from "@root/_models/exchange";
import { environment } from "@env/environment";

@Injectable({ providedIn: "root" })
export class ExchangeService {
    private _http: HttpClient;

    constructor(http: HttpClient) {
        this._http = http;
    }

    public getList(page: number, count: number): Observable<Array<Exchange>> {
        const getUrl = environment.apiUrl + '/exchange/';
        const getQuery = `page=${page};count=${count}`;
        return this._http.get<Array<Exchange>>(getUrl + '?' + getQuery);
    }

    public getOne(code: string): Observable<Exchange> {
        const getUrl = environment.apiUrl = '/exchange/' + code;
        return this._http.get<Exchange>(getUrl);
    }

    public create(exchange: Exchange): Observable<object> {
        const createUrl = environment.apiUrl + '/exchange/' + exchange.exchangeCode;
        const createBody = exchange;
        return this._http.post(createUrl, createBody);
    }

    public update(exchange: Exchange): Observable<object> {
        const updateUrl = environment.apiUrl + '/exchange/' + exchange.exchangeCode;
        const updateBody = exchange;
        return this._http.put(updateUrl, updateBody);
    }
}