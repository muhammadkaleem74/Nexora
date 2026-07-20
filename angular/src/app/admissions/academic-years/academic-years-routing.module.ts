import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AcademicYearsComponent } from './academic-years.component';

const routes: Routes = [
    { path: '', component: AcademicYearsComponent, pathMatch: 'full' },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AcademicYearsRoutingModule {}
