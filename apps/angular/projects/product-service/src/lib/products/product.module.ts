import { NgModule } from '@angular/core';
import {
  NgbCollapseModule,
  NgbDatepickerModule,
  NgbDropdownModule,
} from '@ng-bootstrap/ng-bootstrap';
import { CoreModule } from '@abp/ng.core';
import { PageModule } from '@abp/ng.components/page';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { AdvancedEntityFiltersModule } from '@volo/abp.commercial.ng.ui';
import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';

@NgModule({
  declarations: [ProductComponent],
  imports: [
    ProductRoutingModule,
    CoreModule,
    ThemeSharedModule,
    NgbCollapseModule,
    NgbDatepickerModule,
    NgbDropdownModule,
    PageModule,
    AdvancedEntityFiltersModule,
  ],
})
export class ProductModule {}
