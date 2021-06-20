import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";

import { Company } from "@root/_models/company";
import { environment } from "@env/environment";

@Injectable({ providedIn: "root" })
export class CompanyService {
    private _http: HttpClient;
    private _url: string;

    constructor(http: HttpClient) {
        this._http = http;
        this._url = environment.apiUrl;
    }

    public getList(
        page: number,
        count: number
    ): Observable<HttpResponse<Company[]>> {
        const getUrl = `${this._url}/companies?page=${page}&count=${count}`;
        return this._http.get<Company[]>(getUrl, { observe: "response" });
    }

    public getOne(code: string): Observable<Company> {
        const getUrl = `${this._url}/companies/${code}`;
        return this._http.get<Company>(getUrl);
    }

    public create(company: Company): Observable<object> {
        const createUrl = `${this._url}/companies/${company.companyCode}`;
        const createBody = company;
        return this._http.post(createUrl, createBody);
    }

    public update(company: Company): Observable<object> {
        const updateUrl = `${this._url}/companies/${company.companyCode}`;
        const updateBody = company;
        return this._http.put(updateUrl, updateBody);
    }

    public delete(code: string): Observable<object> {
        const deleteUrl = `${this._url}/companies/${code}`;
        return this._http.delete(deleteUrl);
    }
}
