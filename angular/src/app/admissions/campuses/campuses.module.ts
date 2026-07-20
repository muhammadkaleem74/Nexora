import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { CampusesRoutingModule } from './campuses-routing.module';
import { CampusesComponent } from './campuses.component';
import { CreateEditCampusDialogComponent } from './create-edit-campus/create-edit-campus-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        CampusesRoutingModule,
        CampusesComponent,
        CreateEditCampusDialogComponent,
    ],
})
export class CampusesModule {}
