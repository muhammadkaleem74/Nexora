import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { StudentsRoutingModule } from './students-routing.module';
import { StudentsComponent } from './students.component';
import { StudentDetailComponent } from './student-detail/student-detail.component';
import { CreateStudentDialogComponent } from './create-student/create-student-dialog.component';
import { StudentGuardianDialogComponent } from './student-detail/student-guardian-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        StudentsRoutingModule,
        StudentsComponent,
        StudentDetailComponent,
        CreateStudentDialogComponent,
        StudentGuardianDialogComponent,
    ],
})
export class StudentsModule {}
