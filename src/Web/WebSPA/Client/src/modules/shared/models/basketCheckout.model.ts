﻿export interface IBasketCheckout {
    city: number;
    street: string;
    state: string;
    country: number;
    zipcode: string;
    cardnumber: string;
    cardexpiration: Date;
    expiration: string;
    cardsecuritynumber: string;
    cardholdername: string;
    cardtypeid: number;
    buyer: string;
    ordernumber: string;
    coupon: string;
    total: number;
    discount: number;
    balance: number;
}