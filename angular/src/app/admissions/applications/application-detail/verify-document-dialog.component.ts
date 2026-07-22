import { Component, Injector, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    AdmissionApplicationServiceProxy,
    VerifyDocumentDto,
    ApplicationDocumentDto,
    VerificationStatus,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './verify-document-dialog.component.html',
    standalone: true,
    imports: [FormsModule, NgIf, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class VerifyDocumentDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<ApplicationDocumentDto>();
    @Input() document: ApplicationDocumentDto;

    saving = false;
    VerificationStatus = VerificationStatus;

    model: VerifyDocumentDto = {
        verificationStatus: VerificationStatus.Verified,
        rejectionNote: '',
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _appService: AdmissionApplicationServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.model.verificationStatus = this.document.verificationStatus;
        this.model.rejectionNote = this.document.rejectionNote ?? '';
    }

    get isRejection(): boolean {
        return this.model.verificationStatus === VerificationStatus.Rejected;
    }

    save(): void {
        this.saving = true;
        this._appService.verifyDocument(this.document.id, this.model).subscribe(
            (result) => {
                this.notify.info('Document status updated.');
                this.bsModalRef.hide();
                this.onSave.emit(result);
            },
            () => { this.saving = false; }
        );
    }
}
