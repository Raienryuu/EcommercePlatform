import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'orderStatusTransform',
  standalone: true,
})
export class OrderStatusTransformPipe implements PipeTransform {
  transform(value: number): number {
    if (value === 3 || value === 2) {
      // delivery state
      return 2;
    }
    if (value === 4) {
      return 99;
    }

    if (value >= 4) {
      return 3;
    }

    return value;
  }
}
