import { Component, Injector, OnInit, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import {
    StudentServiceProxy,
    CreateStudentDto,
    CampusDto,
    GradeLevelDto,
    SectionDto,
    GradeLevelServiceProxy,
    SectionServiceProxy,
    GenderType,
    StudentStatus,
} from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-student-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateStudentDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    @Input() campuses: CampusDto[] = [];

    gradeLevels: GradeLevelDto[] = [];
    sections: SectionDto[] = [];
    saving = false;

    model: CreateStudentDto = {
        applicationId: undefined,
        registrationNumber: '',
        grNumber: '',
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        gender: GenderType.Male,
        bloodGroup: '',
        nationality: '',
        religion: '',
        campusId: 0,
        currentGradeLevelId: 0,
        currentSectionId: undefined,
        enrollmentDate: new Date().toISOString().split('T')[0],
        status: StudentStatus.Active,
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private _studentService: StudentServiceProxy,
        private _gradeLevelService: GradeLevelServiceProxy,
        private _sectionService: SectionServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {}

    onCampusChange(): void {
        this.model.currentGradeLevelId = 0;
        this.model.currentSectionId = undefined;
        this.gradeLevels = [];
        this.sections = [];
        if (this.model.campusId) {
            this._gradeLevelService.getAll(this.model.campusId).subscribe(r => {
                this.gradeLevels = r.items;
                this.cd.detectChanges();
            });
        }
    }

    onGradeLevelChange(): void {
        this.model.currentSectionId = undefined;
        this.sections = [];
        if (this.model.currentGradeLevelId) {
            this._sectionService.getAll(this.model.currentGradeLevelId).subscribe(r => {
                this.sections = r.items;
                this.cd.detectChanges();
            });
        }
    }

    save(): void {
        this.saving = true;
        this._studentService.create(this.model).subscribe(
            () => {
                this.notify.info('Student created successfully.');
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => { this.saving = false; }
        );
    }
}
