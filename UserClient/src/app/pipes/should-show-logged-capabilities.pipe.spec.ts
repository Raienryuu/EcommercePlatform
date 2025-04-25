import { ShouldShowLoggedCapabilitiesPipe } from './should-show-logged-capabilities.pipe';

describe('ShouldShowLoggedCapabilitiesPipe', () => {
  it('create an instance', () => {
    const pipe = new ShouldShowLoggedCapabilitiesPipe();
    expect(pipe).toBeTruthy();
  });
});
