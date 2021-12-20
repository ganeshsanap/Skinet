import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IPagination } from '../shared/models/pagination';
import { IType } from '../shared/models/productType';
import { map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  basrUrl = 'https://localhost:44387/api/';

  constructor(private http: HttpClient) { }

  getProducts(brandId?: number, typeId?: number, sort?: string) {
    let params = new HttpParams();

    if(brandId) {
      params = params.append('brandId', brandId.toString());
    }

    if(typeId) {
      params = params.append('typeId', typeId.toString());
    }

    if(sort) {
      params = params.append('sort', sort);
    }

    return this.http.get<IPagination>(this.basrUrl + 'products', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

  getBrands() {
    return this.http.get<IBrand[]>(this.basrUrl + 'products/brands');
  }

  getTypes() {
    return this.http.get<IType[]>(this.basrUrl + 'products/types');
  }
}
