import { Component, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    AdmissionApplicationServiceProxy,
    ChangeApplicationStatusDto,
    ApplicationStatus,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './change-status-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class ChangeStatusDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    @Input() applicationId: number;
    @Input() currentStatus: ApplicationStatus;

    saving = false;
    ApplicationStatus = ApplicationStatus;

    model: ChangeApplicationStatusDto = {
        applicationId: 0,
        newStatus: ApplicationStatus.Submitted,
        notes: '',
        rejectionReason: '',
    };

    statusOptions = [
        { label: 'Submitted', value: ApplicationStatus.Submitted },
        { label: 'Under Review', value: ApplicationStatus.UnderReview },
        { label: 'Test Scheduled', value: ApplicationStatus.TestScheduled },
        { label: 'Approved', value: ApplicationStatus.Approved },
        { label: 'Rejected', value: ApplicationStatus.Rejected },
        { label: 'Withdrawn', value: ApplicationStatus.Withdrawn },
    ];

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _applicationService: AdmissionApplicationServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.model.applicationId = this.applicationId;
        this.model.newStatus = this.currentStatus;
    }

    get isRejection(): boolean {
        return this.model.newStatus === ApplicationStatus.Rejected;
    }

    save(): void {
        this.saving = true;
        this._applicationService.changeStatus(this.model).subscribe(
            () => {
                this.notify.info('Status updated successfully.');
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => { this.saving = false; }
        );
    }
}
