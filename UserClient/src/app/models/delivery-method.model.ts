export interface DeliveryMethod {
  deliveryId: string;
  name: string;
  handlerName: string;
  deliveryType: 'DeliveryPoint' | 'DirectCustomerAddress';
  paymentType: 'Cash' | 'Online';
  price: number;
}
