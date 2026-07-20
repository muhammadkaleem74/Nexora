import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AppComponent } from './app.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    {
                        path: 'home',
                        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'about',
                        loadChildren: () => import('./about/about.module').then((m) => m.AboutModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'users',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'roles',
                        loadChildren: () => import('./roles/roles.module').then((m) => m.RolesModule),
                        data: { permission: 'Pages.Roles' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'tenants',
                        loadChildren: () => import('./tenants/tenants.module').then((m) => m.TenantsModule),
                        data: { permission: 'Pages.Tenants' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'update-password',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admissions/applications',
                        loadChildren: () => import('./admissions/applications/applications.module').then((m) => m.ApplicationsModule),
                        data: { permission: 'Pages.Admissions.Applications' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admissions/students',
                        loadChildren: () => import('./admissions/students/students.module').then((m) => m.StudentsModule),
                        data: { permission: 'Pages.Admissions.Students' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admissions/academic-years',
                        loadChildren: () => import('./admissions/academic-years/academic-years.module').then((m) => m.AcademicYearsModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admissions/campuses',
                        loadChildren: () => import('./admissions/campuses/campuses.module').then((m) => m.CampusesModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admissions/grade-levels',
                        loadChildren: () => import('./admissions/grade-levels/grade-levels.module').then((m) => m.GradeLevelsModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admissions/sections',
                        loadChildren: () => import('./admissions/sections/sections.module').then((m) => m.SectionsModule),
                        canActivate: [AppRouteGuard],
                    },
                ],
            },
        ]),
    ],
    exports: [RouterModule],
})
export class AppRoutingModule {}
