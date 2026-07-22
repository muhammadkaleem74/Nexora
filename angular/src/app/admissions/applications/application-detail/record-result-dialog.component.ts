import { Component, Injector, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    AdmissionApplicationServiceProxy,
    RecordAssessmentResultDto,
    AdmissionAssessmentDto,
    AssessmentResult,
    AssessmentType,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './record-result-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class RecordResultDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<AdmissionAssessmentDto>();
    @Input() assessment: AdmissionAssessmentDto;

    saving = false;
    assessmentTypeLabel = '';

    model: RecordAssessmentResultDto = {
        score: undefined,
        maxScore: undefined,
        remarks: '',
        result: AssessmentResult.Pass,
    };

    resultOptions = [
        { label: 'Pass',        value: AssessmentResult.Pass },
        { label: 'Fail',        value: AssessmentResult.Fail },
        { label: 'Conditional', value: AssessmentResult.Conditional },
    ];

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _appService: AdmissionApplicationServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.assessmentTypeLabel = AssessmentType[this.assessment.assessmentType] ?? 'Assessment';
        this.model.score = this.assessment.score;
        this.model.maxScore = this.assessment.maxScore;
        this.model.remarks = this.assessment.remarks ?? '';
        if (this.assessment.result !== AssessmentResult.Pending) {
            this.model.result = this.assessment.result;
        }
    }

    save(): void {
        this.saving = true;
        this._appService.recordAssessmentResult(this.assessment.id, this.model).subscribe(
            (result) => {
                this.notify.info('Result recorded.');
                this.bsModalRef.hide();
                this.onSave.emit(result);
            },
            () => { this.saving = false; }
        );
    }
}
