import { ChangeDetectorRef, Component, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { AcademicYearServiceProxy, AcademicYearDto } from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-edit-academic-year-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateEditAcademicYearDialogComponent extends AppComponentBase implements OnInit {
    @Input() id?: number;
    @Output() onSave = new EventEmitter<void>();

    isEdit = false;
    saving = false;

    model: AcademicYearDto = {
        id: 0,
        name: '',
        startDate: '',
        endDate: '',
        isActive: true,
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef,
        private _service: AcademicYearServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.id) {
            this.isEdit = true;
            this._service.get(this.id).subscribe(r => {
                this.model = {
                    ...r,
                    startDate: r.startDate?.split('T')[0] ?? '',
                    endDate: r.endDate?.split('T')[0] ?? '',
                };
                this.cd.detectChanges();
            });
        }
    }

    save(): void {
        this.saving = true;
        const obs = this.isEdit ? this._service.update(this.model) : this._service.create(this.model);
        obs.subscribe(
            () => {
                this.notify.success('Saved successfully.');
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => { this.saving = false; }
        );
    }
}
