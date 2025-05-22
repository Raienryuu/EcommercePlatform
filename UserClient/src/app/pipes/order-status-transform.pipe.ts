import { Pipe, PipeTransform } from '@angular/core';
import { match } from 'ts-pattern';

@Pipe({
  name: 'orderStatusTransform',
  standalone: true,
})
export class OrderStatusTransformPipe implements PipeTransform {
  transform(value: string): number {
    const matcher = (value: string) =>
      match(value)
        .returnType<number>()
        .with('AwaitingConfirmation', () => 0)
        .with('Confirmed', () => 1)
        .with('Cancelled', () => 3)
        .with('Succeded', () => 3)
        .with('ReadyToShip', 'Shipped', () => 2)
        .otherwise(() => 0);

    const result = matcher(value);

    return result;
  }
}
