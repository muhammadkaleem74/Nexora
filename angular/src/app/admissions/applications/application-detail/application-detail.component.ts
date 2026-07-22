import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { NgIf, NgFor, NgClass, DatePipe } from '@angular/common';
import {
    AdmissionApplicationServiceProxy,
    AdmissionApplicationDetailDto,
    ApplicationGuardianDto,
    ApplicationDocumentDto,
    AdmissionAssessmentDto,
    ApplicationStatus,
    AssessmentResult,
    AssessmentType,
    VerificationStatus,
    DocumentType,
    GuardianRelationship,
} from '@shared/service-proxies/admissions-proxies';
import { CreateGuardianDialogComponent } from './create-guardian-dialog.component';
import { CreateDocumentDialogComponent } from './create-document-dialog.component';
import { VerifyDocumentDialogComponent } from './verify-document-dialog.component';
import { CreateAssessmentDialogComponent } from './create-assessment-dialog.component';
import { RecordResultDialogComponent } from './record-result-dialog.component';

@Component({
    templateUrl: './application-detail.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [NgIf, NgFor, NgClass, DatePipe, RouterLink],
})
export class ApplicationDetailComponent extends AppComponentBase implements OnInit {
    application: AdmissionApplicationDetailDto | undefined;
    loading = true;
    activeTab: 'overview' | 'guardians' | 'documents' | 'assessments' = 'overview';

    guardians: ApplicationGuardianDto[] = [];
    documents: ApplicationDocumentDto[] = [];
    assessments: AdmissionAssessmentDto[] = [];

    AssessmentResult = AssessmentResult;
    VerificationStatus = VerificationStatus;

    tabs = [
        { id: 'overview',     label: 'Overview',     icon: 'fa-info-circle' },
        { id: 'guardians',    label: 'Guardians',    icon: 'fa-users' },
        { id: 'documents',    label: 'Documents',    icon: 'fa-file-alt' },
        { id: 'assessments',  label: 'Assessments',  icon: 'fa-clipboard-check' },
    ];

    constructor(
        injector: Injector,
        private _route: ActivatedRoute,
        private _appService: AdmissionApplicationServiceProxy,
        private _modalService: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        const id = Number(this._route.snapshot.paramMap.get('id'));
        this._appService.getDetail(id).subscribe({
            next: (a) => {
                this.application = a;
                this.guardians = a.guardians ?? [];
                this.documents = a.documents ?? [];
                this.assessments = a.assessments ?? [];
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
        if (id === 'documents') return this.documents.length;
        if (id === 'assessments') return this.assessments.length;
        return -1;
    }

    // ── Guardians ─────────────────────────────────────────────────────────────

    addGuardian(): void {
        const ref: BsModalRef = this._modalService.show(CreateGuardianDialogComponent, {
            class: 'modal-lg',
            initialState: { applicationId: this.application.id },
        });
        ref.content.onSave.subscribe((g: ApplicationGuardianDto) => {
            this.guardians = [...this.guardians, g];
            this.cd.detectChanges();
        });
    }

    removeGuardian(g: ApplicationGuardianDto): void {
        abp.message.confirm(`Remove ${g.fullName}?`, undefined, (confirmed: boolean) => {
            if (!confirmed) return;
            this._appService.removeGuardian(g.id).subscribe(() => {
                this.guardians = this.guardians.filter(x => x.id !== g.id);
                this.cd.detectChanges();
            });
        });
    }

    // ── Documents ─────────────────────────────────────────────────────────────

    addDocument(): void {
        const ref: BsModalRef = this._modalService.show(CreateDocumentDialogComponent, {
            class: 'modal-md',
            initialState: { applicationId: this.application.id },
        });
        ref.content.onSave.subscribe((doc: ApplicationDocumentDto) => {
            this.documents = [...this.documents, doc];
            this.cd.detectChanges();
        });
    }

    removeDocument(doc: ApplicationDocumentDto): void {
        abp.message.confirm(`Remove "${doc.fileName}"?`, undefined, (confirmed: boolean) => {
            if (!confirmed) return;
            this._appService.removeDocument(doc.id).subscribe(() => {
                this.documents = this.documents.filter(x => x.id !== doc.id);
                this.cd.detectChanges();
            });
        });
    }

    verifyDocument(doc: ApplicationDocumentDto): void {
        const ref: BsModalRef = this._modalService.show(VerifyDocumentDialogComponent, {
            class: 'modal-md',
            initialState: { document: doc },
        });
        ref.content.onSave.subscribe((updated: ApplicationDocumentDto) => {
            this.documents = this.documents.map(d => d.id === updated.id ? updated : d);
            this.cd.detectChanges();
        });
    }

    // ── Assessments ───────────────────────────────────────────────────────────

    scheduleAssessment(): void {
        const ref: BsModalRef = this._modalService.show(CreateAssessmentDialogComponent, {
            class: 'modal-md',
            initialState: { applicationId: this.application.id },
        });
        ref.content.onSave.subscribe((a: AdmissionAssessmentDto) => {
            this.assessments = [...this.assessments, a];
            this.cd.detectChanges();
        });
    }

    recordResult(assessment: AdmissionAssessmentDto): void {
        const ref: BsModalRef = this._modalService.show(RecordResultDialogComponent, {
            class: 'modal-md',
            initialState: { assessment },
        });
        ref.content.onSave.subscribe((updated: AdmissionAssessmentDto) => {
            this.assessments = this.assessments.map(a => a.id === updated.id ? updated : a);
            this.cd.detectChanges();
        });
    }

    // ── Label helpers ─────────────────────────────────────────────────────────

    getStatusClass(s: ApplicationStatus): string {
        switch (s) {
            case ApplicationStatus.Approved:
            case ApplicationStatus.Enrolled:  return 'nx-status--active';
            case ApplicationStatus.Rejected:
            case ApplicationStatus.Withdrawn: return 'nx-status--inactive';
            default:                          return 'nx-status--pending';
        }
    }

    getStatusLabel(s: ApplicationStatus): string { return ApplicationStatus[s] ?? 'Unknown'; }

    getRelationshipLabel(r: GuardianRelationship): string { return GuardianRelationship[r] ?? 'Other'; }

    getGenderLabel(g: number): string { return g === 1 ? 'Male' : g === 2 ? 'Female' : 'Other'; }

    getDocTypeLabel(t: DocumentType): string {
        const map: Record<number, string> = {
            1: 'Birth Certificate', 2: 'Passport Photo', 3: 'School Report',
            4: 'Medical Record', 5: 'National ID', 99: 'Other',
        };
        return map[t] ?? 'Other';
    }

    getVerificationClass(s: VerificationStatus): string {
        if (s === VerificationStatus.Verified) return 'nx-status--active';
        if (s === VerificationStatus.Rejected) return 'nx-status--inactive';
        return 'nx-status--pending';
    }

    getVerificationLabel(s: VerificationStatus): string { return VerificationStatus[s] ?? 'Pending'; }

    getAssessmentTypeLabel(t: AssessmentType): string { return AssessmentType[t] ?? 'Unknown'; }

    getResultClass(r: AssessmentResult): string {
        if (r === AssessmentResult.Pass)        return 'nx-status--active';
        if (r === AssessmentResult.Fail)        return 'nx-status--inactive';
        if (r === AssessmentResult.Conditional) return 'nx-status--pending';
        return 'nx-status--pending';
    }

    getResultLabel(r: AssessmentResult): string { return AssessmentResult[r] ?? 'Pending'; }
}
