import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { SectionsRoutingModule } from './sections-routing.module';
import { SectionsComponent } from './sections.component';
import { CreateEditSectionDialogComponent } from './create-edit-section/create-edit-section-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        SectionsRoutingModule,
        SectionsComponent,
        CreateEditSectionDialogComponent,
    ],
})
export class SectionsModule {}
