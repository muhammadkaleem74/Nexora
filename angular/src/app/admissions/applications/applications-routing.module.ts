import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApplicationsComponent } from './applications.component';
import { ApplicationDetailComponent } from './application-detail/application-detail.component';

const routes: Routes = [
    { path: '', component: ApplicationsComponent, pathMatch: 'full' },
    { path: ':id', component: ApplicationDetailComponent },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ApplicationsRoutingModule {}
