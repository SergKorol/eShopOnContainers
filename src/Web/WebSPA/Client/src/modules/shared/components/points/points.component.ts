import { Component, OnInit } from '@angular/core';
import {PointsService} from "../../services/points.service";
import {SecurityService} from "../../services/security.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'esh-points',
  templateUrl: './points.component.html',
  styleUrls: ['./points.component.scss']
})
export class PointsComponent implements OnInit {
  authenticated: boolean = false;
  private subscription: Subscription;
  private userName: string = '';
  public numberOfPoints: number = 0;
  public cash:number = 0;
  private errorMessage: string = '';
  constructor(private service: SecurityService, private pointService: PointsService) { }

  ngOnInit(): void {
    this.authenticated = this.service.IsAuthorized;
    console.log("Enter into eshop: " + this.authenticated);
    if (this.authenticated) {
      this.getUserPoints(this.service.UserData.email);
    }
  }
  getUserPoints(userId: string) {
    this.pointService
        .getPointsByUser(userId)
        .subscribe(
            point => {
              if (point != null) {
                this.numberOfPoints = point.numberOfPoints;
                this.cash = point.cash;
              }
            },
            error => {
              if (error.status == 404) {
                this.errorMessage = `${error.error}!`;
              } else {
                this.errorMessage = `ERROR: ${error.status} - ${error.statusText}!`;
              }
              console.log(error);
            });
  }
}
