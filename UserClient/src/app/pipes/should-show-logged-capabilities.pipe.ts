import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'shouldShowLoggedCapabilities',
  standalone: true,
})
export class ShouldShowLoggedCapabilitiesPipe implements PipeTransform {
  transform(route: string): unknown {
    return !(route.includes('login') || route.includes('register'));
  }
}
