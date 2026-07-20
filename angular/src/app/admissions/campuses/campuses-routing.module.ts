import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CampusesComponent } from './campuses.component';

const routes: Routes = [
    { path: '', component: CampusesComponent, pathMatch: 'full' },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CampusesRoutingModule {}
