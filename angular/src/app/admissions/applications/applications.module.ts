import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { ApplicationsRoutingModule } from './applications-routing.module';
import { ApplicationsComponent } from './applications.component';
import { CreateApplicationDialogComponent } from './create-application/create-application-dialog.component';
import { ChangeStatusDialogComponent } from './change-status/change-status-dialog.component';
import { ConvertToStudentDialogComponent } from './convert-to-student/convert-to-student-dialog.component';
import { ApplicationDetailComponent } from './application-detail/application-detail.component';
import { CreateGuardianDialogComponent } from './application-detail/create-guardian-dialog.component';
import { CreateDocumentDialogComponent } from './application-detail/create-document-dialog.component';
import { VerifyDocumentDialogComponent } from './application-detail/verify-document-dialog.component';
import { CreateAssessmentDialogComponent } from './application-detail/create-assessment-dialog.component';
import { RecordResultDialogComponent } from './application-detail/record-result-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        ApplicationsRoutingModule,
        ApplicationsComponent,
        CreateApplicationDialogComponent,
        ChangeStatusDialogComponent,
        ConvertToStudentDialogComponent,
        ApplicationDetailComponent,
        CreateGuardianDialogComponent,
        CreateDocumentDialogComponent,
        VerifyDocumentDialogComponent,
        CreateAssessmentDialogComponent,
        RecordResultDialogComponent,
    ],
})
export class ApplicationsModule {}
