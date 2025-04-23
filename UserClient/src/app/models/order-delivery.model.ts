import { CustomerAddress } from './customer-address.model';
export interface OrderDelivery {
  deliveryId: string;
  name: string;
  handlerName: string;
  deliveryType: 'DeliveryPoint' | 'DirectCustomerAddress';
  paymentType: 'Cash' | 'Online';
  price: number;
  customerInformation: CustomerAddress | null;
  externalDeliveryId: string | null;
}
