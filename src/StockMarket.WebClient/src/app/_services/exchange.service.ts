import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";

import { Exchange } from "@root/_models/exchange";
import { environment } from "@env/environment";

@Injectable({ providedIn: "root" })
export class ExchangeService {
    private _http: HttpClient;
    private _url: string;

    constructor(http: HttpClient) {
        this._http = http;
        this._url = environment.apiUrl;
    }

    public getList(page: number, count: number): Observable<HttpResponse<Exchange[]>> {
        const getUrl = `${this._url}/exchanges?page=${page}&count=${count}`;
        return this._http.get<Exchange[]>(getUrl, { observe: 'response' });
    }

    public getOne(code: string): Observable<Exchange> {
        const getUrl = `${this._url}/exchanges/${code}`
        return this._http.get<Exchange>(getUrl);
    }

    public create(exchange: Exchange): Observable<object> {
        const createUrl = `${this._url}/exchanges/${exchange.exchangeCode}`;
        const createBody = exchange;
        return this._http.post(createUrl, createBody);
    }

    public update(exchange: Exchange): Observable<object> {
        const updateUrl = `${this._url}/exchanges/${exchange.exchangeCode}`;
        const updateBody = exchange;
        return this._http.put(updateUrl, updateBody);
    }
}
