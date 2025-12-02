import { Component } from '@angular/core';
import { DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
  `,
  styleUrls: ['./app.component.scss'],
  imports: [LoaderBarComponent, DynamicLayoutComponent],
})
export class AppComponent {}
