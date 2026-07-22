import { Component, Injector, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    AdmissionApplicationServiceProxy,
    CreateAssessmentDto,
    AdmissionAssessmentDto,
    AssessmentType,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-assessment-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateAssessmentDialogComponent extends AppComponentBase {
    @Output() onSave = new EventEmitter<AdmissionAssessmentDto>();
    @Input() applicationId: number;

    saving = false;

    model: CreateAssessmentDto = {
        assessmentType: AssessmentType.Written,
        scheduledDate: undefined,
    };

    assessmentTypes = [
        { label: 'Written',     value: AssessmentType.Written },
        { label: 'Oral',        value: AssessmentType.Oral },
        { label: 'Practical',   value: AssessmentType.Practical },
        { label: 'Interview',   value: AssessmentType.Interview },
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
        this._appService.scheduleAssessment(this.applicationId, this.model).subscribe(
            (result) => {
                this.notify.info('Assessment scheduled.');
                this.bsModalRef.hide();
                this.onSave.emit(result);
            },
            () => { this.saving = false; }
        );
    }
}
