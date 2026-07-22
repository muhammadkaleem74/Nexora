import { Component, Injector, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    AdmissionApplicationServiceProxy,
    CreateGuardianDto,
    ApplicationGuardianDto,
    GuardianRelationship,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-guardian-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateGuardianDialogComponent extends AppComponentBase {
    @Output() onSave = new EventEmitter<ApplicationGuardianDto>();
    @Input() applicationId: number;

    saving = false;

    model: CreateGuardianDto = {
        fullName: '',
        relationship: GuardianRelationship.Father,
        nationalIdNumber: '',
        email: '',
        phone: '',
        occupation: '',
        address: '',
        isPrimaryContact: false,
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _appService: AdmissionApplicationServiceProxy
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;
        this._appService.addGuardian(this.applicationId, this.model).subscribe(
            (result) => {
                this.notify.info('Guardian added.');
                this.bsModalRef.hide();
                this.onSave.emit(result);
            },
            () => { this.saving = false; }
        );
    }
}
