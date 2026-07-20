import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GradeLevelsComponent } from './grade-levels.component';

const routes: Routes = [
    { path: '', component: GradeLevelsComponent, pathMatch: 'full' },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class GradeLevelsRoutingModule {}
