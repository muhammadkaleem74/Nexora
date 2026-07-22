import { Component, Injector, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    StudentServiceProxy,
    CreateGuardianDto,
    StudentGuardianDto,
    GuardianRelationship,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './student-guardian-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class StudentGuardianDialogComponent extends AppComponentBase {
    @Output() onSave = new EventEmitter<StudentGuardianDto>();
    @Input() studentId: number;

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
        private _studentService: StudentServiceProxy
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;
        this._studentService.addGuardian(this.studentId, this.model).subscribe(
            (result) => {
                this.notify.info('Guardian added.');
                this.bsModalRef.hide();
                this.onSave.emit(result);
            },
            () => { this.saving = false; }
        );
    }
}
