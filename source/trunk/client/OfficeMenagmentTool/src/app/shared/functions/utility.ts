import { HttpParams } from "@angular/common/http";

export class AppUtility {
    static getParamsFromObject(object: object): HttpParams {
        return new HttpParams({ fromObject: Object.fromEntries(Object.entries(object).filter(([_, v]) => v)) });
    }
}