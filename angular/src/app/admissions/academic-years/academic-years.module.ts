import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { AcademicYearsRoutingModule } from './academic-years-routing.module';
import { AcademicYearsComponent } from './academic-years.component';
import { CreateEditAcademicYearDialogComponent } from './create-edit-academic-year/create-edit-academic-year-dialog.component';

@NgModule({
    imports: [
        SharedModule,
        AcademicYearsRoutingModule,
        AcademicYearsComponent,
        CreateEditAcademicYearDialogComponent,
    ],
})
export class AcademicYearsModule {}
