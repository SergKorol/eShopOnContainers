import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {map, tap} from "rxjs/operators";
import {IPoint} from "../models/points.model";
import {DataService} from "./data.service";
import {ConfigurationService} from "./configuration.service";
import {ICoupon} from "../models/coupon.model";

@Injectable({
  providedIn: 'root'
})
export class PointsService {
  public UserData: any;
  private couponUrl: string = '';
  constructor(private service: DataService, private configurationService: ConfigurationService) {
    // this.couponUrl = this.configurationService.serverSettings.purchaseUrl;
  }


  getPointsByUser(userId: string): Observable<IPoint> {
    let url = "http://localhost:5106" + "/api/v1/point/" + userId;
    
    return this.service.get(url).pipe<IPoint>(map<Response, IPoint>((response: any) => {
      return response;
    }));
  }
}