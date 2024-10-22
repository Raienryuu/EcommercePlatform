import { CustomerAddress } from "./customer-address.model";

export interface NewUser {
    UserName: string;
    Password: string;
    Address: CustomerAddress;
}