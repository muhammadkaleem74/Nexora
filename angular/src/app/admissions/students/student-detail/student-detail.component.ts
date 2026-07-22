import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { NgIf, NgFor, NgClass, DatePipe } from '@angular/common';
import {
    StudentServiceProxy,
    StudentDetailDto,
    StudentGuardianDto,
    EnrollmentHistoryDto,
    GuardianRelationship,
    PromotionStatus,
    StudentStatus,
} from '@shared/service-proxies/admissions-proxies';
import { StudentGuardianDialogComponent } from './student-guardian-dialog.component';

@Component({
    templateUrl: './student-detail.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [NgIf, NgFor, NgClass, DatePipe, RouterLink],
})
export class StudentDetailComponent extends AppComponentBase implements OnInit {
    student: StudentDetailDto | undefined;
    loading = true;
    activeTab: 'overview' | 'guardians' | 'history' = 'overview';

    guardians: StudentGuardianDto[] = [];
    history: EnrollmentHistoryDto[] = [];

    PromotionStatus = PromotionStatus;

    tabs = [
        { id: 'overview',  label: 'Overview',            icon: 'fa-info-circle' },
        { id: 'guardians', label: 'Guardians',            icon: 'fa-users' },
        { id: 'history',   label: 'Enrollment History',   icon: 'fa-history' },
    ];

    constructor(
        injector: Injector,
        private _route: ActivatedRoute,
        private _studentService: StudentServiceProxy,
        private _modalService: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        const id = Number(this._route.snapshot.paramMap.get('id'));
        this._studentService.getDetail(id).subscribe({
            next: (s) => {
                this.student = s;
                this.guardians = s.guardians ?? [];
                this.history = s.enrollmentHistories ?? [];
                this.loading = false;
                this.cd.detectChanges();
            },
            error: () => {
                this.loading = false;
                this.cd.detectChanges();
            },
        });
    }

    tabCount(id: string): number {
        if (id === 'guardians') return this.guardians.length;
        if (id === 'history')   return this.history.length;
        return -1;
    }

    // ── Guardians ─────────────────────────────────────────────────────────────

    addGuardian(): void {
        const ref: BsModalRef = this._modalService.show(StudentGuardianDialogComponent, {
            class: 'modal-lg',
            initialState: { studentId: this.student.id },
        });
        ref.content.onSave.subscribe((g: StudentGuardianDto) => {
            this.guardians = [...this.guardians, g];
            this.cd.detectChanges();
        });
    }

    removeGuardian(g: StudentGuardianDto): void {
        abp.message.confirm(`Remove ${g.fullName}?`, undefined, (confirmed: boolean) => {
            if (!confirmed) return;
            this._studentService.removeGuardian(g.id).subscribe(() => {
                this.guardians = this.guardians.filter(x => x.id !== g.id);
                this.cd.detectChanges();
            });
        });
    }

    // ── Label helpers ─────────────────────────────────────────────────────────

    getStatusClass(s: StudentStatus): string {
        switch (s) {
            case StudentStatus.Active:      return 'nx-status--active';
            case StudentStatus.Inactive:    return 'nx-status--inactive';
            default:                        return 'nx-status--pending';
        }
    }

    getStatusLabel(s: StudentStatus): string { return StudentStatus[s] ?? 'Unknown'; }

    getRelationshipLabel(r: GuardianRelationship): string { return GuardianRelationship[r] ?? 'Other'; }

    getGenderLabel(g: number): string { return g === 1 ? 'Male' : g === 2 ? 'Female' : 'Other'; }

    getPromotionClass(p: PromotionStatus): string {
        if (p === PromotionStatus.Promoted)   return 'nx-status--active';
        if (p === PromotionStatus.Retained || p === PromotionStatus.Withdrawn) return 'nx-status--inactive';
        return 'nx-status--pending';
    }

    getPromotionLabel(p: PromotionStatus): string { return PromotionStatus[p] ?? 'Unknown'; }
}
