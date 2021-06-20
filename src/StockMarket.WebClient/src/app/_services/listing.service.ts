import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";

import { Listing } from "@root/_models/listing";
import { environment } from "@env/environment";

@Injectable({ providedIn: "root" })
export class ListingService {
    private _http: HttpClient;
    private _url: string;

    constructor(http: HttpClient) {
        this._http = http;
        this._url = environment.apiUrl;
    }

    public getList(
        page: number,
        count: number
    ): Observable<HttpResponse<Listing[]>> {
        const getUrl = `${this._url}/listings?page=${page}&count=${count}`;
        return this._http.get<Listing[]>(getUrl, { observe: "response" });
    }

    public getByCompany(companyCode: string): Observable<Listing[]> {
        const getUrl = `${this._url}/listings/${companyCode}`;
        return this._http.get<Listing[]>(getUrl);
    }

    public getOne(
        exchangeCode: string,
        tickerSymbol: string
    ): Observable<Listing> {
        const listingCode = `${exchangeCode}:${tickerSymbol}`;
        const getUrl = `${this._url}/listings/${listingCode}`;
        return this._http.get<Listing>(getUrl);
    }

    public create(listing: Listing): Observable<object> {
        const listingCode = `${listing.exchangeCode}:${listing.tickerSymbol}`;
        const createUrl = `${this._url}/listings/${listingCode}`;
        const createBody = listing;
        return this._http.post(createUrl, createBody);
    }

    public update(listing: Listing): Observable<object> {
        const listingCode = `${listing.exchangeCode}:${listing.tickerSymbol}`;
        const updateUrl = `${this._url}/listings/${listingCode}`;
        const updateBody = listing;
        return this._http.put(updateUrl, updateBody);
    }

    public delete(
        exchangeCode: string,
        tickerSymbol: string
    ): Observable<object> {
        const listingCode = `${exchangeCode}:${tickerSymbol}`;
        const deleteUrl = `${this._url}/listings/${listingCode}`;
        return this._http.delete(deleteUrl);
    }
}
