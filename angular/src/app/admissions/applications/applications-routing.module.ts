import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApplicationsComponent } from './applications.component';

const routes: Routes = [
    { path: '', component: ApplicationsComponent, pathMatch: 'full' },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ApplicationsRoutingModule {}
