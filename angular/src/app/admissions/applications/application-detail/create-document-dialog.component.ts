import { Component, Injector, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    AdmissionApplicationServiceProxy,
    CreateApplicationDocumentDto,
    ApplicationDocumentDto,
    DocumentType,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-document-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateDocumentDialogComponent extends AppComponentBase {
    @Output() onSave = new EventEmitter<ApplicationDocumentDto>();
    @Input() applicationId: number;

    saving = false;

    model: CreateApplicationDocumentDto = {
        documentType: DocumentType.BirthCertificate,
        fileName: '',
        fileUrl: '',
        fileSizeKb: undefined,
    };

    documentTypes = [
        { label: 'Birth Certificate',  value: DocumentType.BirthCertificate },
        { label: 'Passport Photo',     value: DocumentType.PassportPhoto },
        { label: 'School Report',      value: DocumentType.PreviousSchoolReport },
        { label: 'Medical Record',     value: DocumentType.MedicalRecord },
        { label: 'National ID',        value: DocumentType.NationalId },
        { label: 'Other',              value: DocumentType.Other },
    ];

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _appService: AdmissionApplicationServiceProxy
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;
        this._appService.addDocument(this.applicationId, this.model).subscribe(
            (result) => {
                this.notify.info('Document added.');
                this.bsModalRef.hide();
                this.onSave.emit(result);
            },
            () => { this.saving = false; }
        );
    }
}
