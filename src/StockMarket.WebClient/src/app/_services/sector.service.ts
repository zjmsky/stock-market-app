import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";

import { Sector } from "@root/_models/sector";
import { environment } from "@env/environment";

@Injectable({ providedIn: "root" })
export class SectorService {
    private _http: HttpClient;
    private _url: string;

    constructor(http: HttpClient) {
        this._http = http;
        this._url = environment.apiUrl;
    }

    public getList(
        page: number,
        count: number
    ): Observable<HttpResponse<Sector[]>> {
        const getUrl = `${this._url}/sectors?page=${page}&count=${count}`;
        return this._http.get<Sector[]>(getUrl, { observe: "response" });
    }

    public getOne(code: string): Observable<Sector> {
        const getUrl = `${this._url}/sectors/${code}`;
        return this._http.get<Sector>(getUrl);
    }

    public create(sector: Sector): Observable<object> {
        const createUrl = `${this._url}/sectors/${sector.sectorCode}`;
        const createBody = sector;
        return this._http.post(createUrl, createBody);
    }

    public update(sector: Sector): Observable<object> {
        const updateUrl = `${this._url}/sectors/${sector.sectorCode}`;
        const updateBody = sector;
        return this._http.put(updateUrl, updateBody);
    }

    public delete(code: string): Observable<object> {
        const deleteUrl = `${this._url}/sectors/${code}`;
        return this._http.delete(deleteUrl);
    }
}
