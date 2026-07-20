import { Component, Injector, OnInit, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import {
    AdmissionApplicationServiceProxy,
    CreateAdmissionApplicationDto,
    AcademicYearDto,
    CampusDto,
    GradeLevelDto,
    GradeLevelServiceProxy,
    GenderType,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-application-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent, LocalizePipe],
})
export class CreateApplicationDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    @Input() campuses: CampusDto[] = [];
    @Input() academicYears: AcademicYearDto[] = [];

    gradeLevels: GradeLevelDto[] = [];
    saving = false;

    model: CreateAdmissionApplicationDto = {
        academicYearId: 0,
        campusId: 0,
        desiredGradeLevelId: 0,
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        gender: GenderType.Male,
        nationality: '',
        religion: '',
        previousSchoolName: '',
        previousSchoolBoard: '',
        previousGradeLevel: '',
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _applicationService: AdmissionApplicationServiceProxy,
        private _gradeLevelService: GradeLevelServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {}

    onCampusChange(): void {
        this.model.desiredGradeLevelId = 0;
        this.gradeLevels = [];
        if (this.model.campusId) {
            this._gradeLevelService.getAll(this.model.campusId).subscribe(r => {
                this.gradeLevels = r.items;
                this.cd.detectChanges();
            });
        }
    }

    save(): void {
        this.saving = true;
        this._applicationService.create(this.model).subscribe(
            () => {
                this.notify.info('Application created successfully.');
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => { this.saving = false; }
        );
    }
}
