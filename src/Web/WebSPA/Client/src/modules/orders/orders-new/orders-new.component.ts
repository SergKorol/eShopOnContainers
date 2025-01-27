
import {throwError as observableThrowError,  Observable } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { catchError } from 'rxjs/operators';

import { OrdersService } from '../orders.service';
import { BasketService } from '../../basket/basket.service';
import { IOrder }                                   from '../../shared/models/order.model';
import { BasketWrapperService }                     from '../../shared/services/basket.wrapper.service';
import { ICoupon } from '../../shared/models/coupon.model';

import { FormGroup, FormBuilder, Validators  }      from '@angular/forms';
import { Router }                                   from '@angular/router';
import {IPoint} from "../../shared/models/points.model";

@Component({
    selector: 'esh-orders_new .esh-orders_new .mb-5',
    styleUrls: ['./orders-new.component.scss'],
    templateUrl: './orders-new.component.html'
})
export class OrdersNewComponent implements OnInit {
    newOrderForm: FormGroup;  // new order form
    isOrderProcessing: boolean;
    errorReceived: boolean;
    order: IOrder;
    coupon: ICoupon;
    point: IPoint;
    discountCode: string;
    balance: number = 0;
    discount: number;
    couponValidationMessage: string;
    pointValidationMessage: string;
    checkedPoints: boolean = false;

    constructor(private orderService: OrdersService, private basketService: BasketService, fb: FormBuilder, private router: Router) {
        // Obtain user profile information
        if (this.checkedPoints == false) {
            this.getBalance();
        }
        this.order = orderService.mapOrderAndIdentityInfoNewOrder();
        this.newOrderForm = fb.group({
            'street': [this.order.street, Validators.required],
            'city': [this.order.city, Validators.required],
            'state': [this.order.state, Validators.required],
            'country': [this.order.country, Validators.required],
            'cardnumber': [this.order.cardnumber, Validators.required],
            'cardholdername': [this.order.cardholdername, Validators.required],
            'expirationdate': [this.order.expiration, Validators.required],
            'securitycode': [this.order.cardsecuritynumber, Validators.required],
        });
    }

    ngOnInit() {
        
    }

    keyAgreeUsePoints(event: any) {
        if(event.target.checked==true){
            this.checkedPoints = true;
            this.checkValidationPoints();
        }
        else{
            this.checkedPoints = false;
            this.point = null;
        }
    }

    keyDownValidationCoupon(event: KeyboardEvent, discountCode: string) {
        if(event.keyCode === 13) {
            event.preventDefault();
            this.checkValidationCoupon(discountCode);
        }
    }

    checkValidationCoupon(discountCode: string) {
        this.couponValidationMessage = null;
        this.coupon = null;
        this.orderService
            .checkValidationCoupon(discountCode)
            .subscribe(
                coupon => this.coupon = coupon,
                error => {
                    if (error.status == 404) {
                        this.couponValidationMessage = `${error.error}!`;
                    } else {
                        this.couponValidationMessage = `ERROR: ${error.status} - ${error.statusText}!`;
                    }
                    console.log(error);
                });
    }

    checkValidationPoints() {
        this.pointValidationMessage = null;
        this.point = null;
        this.coupon = null;
        this.orderService
            .checkValidationPoints()
            .subscribe(
                point => {
                    this.point = point;
                    if (this.point != null){
                        let maxDiscountByPoints: number = this.order.total / 10;
                        if (this.point.cash >= maxDiscountByPoints){
                            this.discount = maxDiscountByPoints;
                            // this.balance = -(Math.round(maxDiscountByPoints * 100));
                            this.balance = Math.round(this.point.cash - this.discount + this.order.total);
                            console.log("BALANCE: " + this.balance);
                        } else if(maxDiscountByPoints >= this.point.cash){
                            this.discount = this.point.cash;
                            // this.balance = -(Math.round(this.point.cash * 100));
                            this.balance = Math.round(this.point.cash - this.discount + this.order.total);
                            console.log("BALANCE: " + this.balance);
                        }
                    }
                },
                error => {
                    if (error.status == 404) {
                        this.pointValidationMessage = `${error.error}!`;
                    } else {
                        this.pointValidationMessage = `ERROR: ${error.status} - ${error.statusText}!`;
                    }
                    console.log(error);
                });
        
    }
    
    getBalance(){
        this.orderService
            .checkValidationPoints()
            .subscribe(
                point => {
                    this.point = point;
                    if (this.point != null){
                        this.coupon = new class implements ICoupon {
                            code: string;
                            discount: number;
                            message: string;
                        };
                        this.balance = this.point.cash;
                    }
                },
                error => {
                    if (error.status == 404) {
                        this.pointValidationMessage = `${error.error}!`;
                    } else {
                        this.pointValidationMessage = `ERROR: ${error.status} - ${error.statusText}!`;
                    }
                    console.log(error);
                });
    }

    submitForm(value: any) {
        
        this.order.street = this.newOrderForm.controls['street'].value;
        this.order.city = this.newOrderForm.controls['city'].value;
        this.order.state = this.newOrderForm.controls['state'].value;
        this.order.country = this.newOrderForm.controls['country'].value;
        this.order.cardnumber = this.newOrderForm.controls['cardnumber'].value;
        this.order.cardtypeid = 1;
        this.order.cardholdername = this.newOrderForm.controls['cardholdername'].value;
        this.order.cardexpiration = new Date(20 + this.newOrderForm.controls['expirationdate'].value.split('/')[1], this.newOrderForm.controls['expirationdate'].value.split('/')[0]);
        this.order.cardsecuritynumber = this.newOrderForm.controls['securitycode'].value;
        if (this.coupon && this.checkedPoints == false && !this.point) {
            console.log(`Coupon: ${this.coupon.code} (${this.coupon.discount})`);

            this.order.coupon = this.coupon.code;
            this.order.discount = this.coupon.discount;
        }
        else if (this.coupon && this.checkedPoints == true && !this.point) {
            this.order.discount = this.coupon.discount = this.discount;
        }
        else {
            this.discount = null
            this.order.balance = Math.floor(this.order.total);
        }
        
        let basketCheckout = this.basketService.mapBasketInfoCheckout(this.order);
        this.basketService.setBasketCheckout(basketCheckout)
            .pipe(catchError((errMessage) => {
                this.errorReceived = true;
                this.isOrderProcessing = false;
                return observableThrowError(errMessage); 
            }))
            .subscribe(res => {
                this.router.navigate(['orders']);
            });
        this.errorReceived = false;
        this.isOrderProcessing = true;
    }
}

