export interface DeliveryMethod {
  id: string;
  name: string;
  deliveryType: 'DeliveryPoint' | 'DirectCustomerAddress';
  paymentType: 'Cash' | 'Online';
  price: number;
}
