import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'orderStatusTransform',
  standalone: true,
})
export class OrderStatusTransformPipe implements PipeTransform {
  transform(value: number): number {
    value += 1;

    if (value === 4 || value === 3) {
      // delivery state
      return 2;
    }
    if (value === 5) {
      return 99;
    }

    if (value >= 5) {
      return 3;
    }

    return value;
  }
}
