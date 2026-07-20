import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { ApplicationsRoutingModule } from './applications-routing.module';
import { ApplicationsComponent } from './applications.component';
import { CreateApplicationDialogComponent } from './create-application/create-application-dialog.component';
import { ChangeStatusDialogComponent } from './change-status/change-status-dialog.component';
import { ConvertToStudentDialogComponent } from './convert-to-student/convert-to-student-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        ApplicationsRoutingModule,
        ApplicationsComponent,
        CreateApplicationDialogComponent,
        ChangeStatusDialogComponent,
        ConvertToStudentDialogComponent,
    ],
})
export class ApplicationsModule {}
