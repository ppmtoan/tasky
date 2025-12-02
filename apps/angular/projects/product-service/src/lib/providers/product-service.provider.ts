import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';

export function provideProductService(): EnvironmentProviders {
  return makeEnvironmentProviders([]);
}
