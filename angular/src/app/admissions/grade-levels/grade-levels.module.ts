import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { GradeLevelsRoutingModule } from './grade-levels-routing.module';
import { GradeLevelsComponent } from './grade-levels.component';
import { CreateEditGradeLevelDialogComponent } from './create-edit-grade-level/create-edit-grade-level-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        GradeLevelsRoutingModule,
        GradeLevelsComponent,
        CreateEditGradeLevelDialogComponent,
    ],
})
export class GradeLevelsModule {}
