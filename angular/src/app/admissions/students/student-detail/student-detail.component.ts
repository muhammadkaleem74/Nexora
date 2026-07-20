import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { NgIf, NgFor, DatePipe } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import {
    StudentServiceProxy,
    StudentDetailDto,
    StudentGuardianDto,
    EnrollmentHistoryDto,
    GuardianRelationship,
    PromotionStatus,
    StudentStatus,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './student-detail.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [NgIf, NgFor, DatePipe, RouterLink, LocalizePipe],
})
export class StudentDetailComponent extends AppComponentBase implements OnInit {
    student: StudentDetailDto | undefined;
    loading = true;

    GuardianRelationship = GuardianRelationship;
    PromotionStatus = PromotionStatus;
    StudentStatus = StudentStatus;

    constructor(
        injector: Injector,
        private _route: ActivatedRoute,
        private _studentService: StudentServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        const id = Number(this._route.snapshot.paramMap.get('id'));
        this._studentService.getDetail(id).subscribe({
            next: (s) => {
                this.student = s;
                this.loading = false;
                this.cd.detectChanges();
            },
            error: () => {
                this.loading = false;
                this.cd.detectChanges();
            },
        });
    }

    getRelationshipLabel(r: GuardianRelationship): string {
        return GuardianRelationship[r] ?? 'Unknown';
    }

    getPromotionLabel(p: PromotionStatus): string {
        return PromotionStatus[p] ?? 'Unknown';
    }

    getStatusClass(s: StudentStatus): string {
        return s === StudentStatus.Active ? 'nx-status--active' : 'nx-status--inactive';
    }

    getStatusLabel(s: StudentStatus): string {
        return StudentStatus[s] ?? 'Unknown';
    }

    getGenderLabel(g: number): string {
        return g === 1 ? 'Male' : g === 2 ? 'Female' : 'Other';
    }
}
