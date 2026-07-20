import { ChangeDetectorRef, Component, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { SectionServiceProxy, SectionDto, GradeLevelDto } from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-edit-section-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateEditSectionDialogComponent extends AppComponentBase implements OnInit {
    @Input() id?: number;
    @Input() gradeLevels: GradeLevelDto[] = [];
    @Output() onSave = new EventEmitter<void>();

    isEdit = false;
    saving = false;

    model: SectionDto = {
        id: 0,
        gradeLevelId: 0,
        gradeLevelName: '',
        name: '',
        capacity: 0,
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef,
        private _service: SectionServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.id) {
            this.isEdit = true;
            this._service.get(this.id).subscribe(r => {
                this.model = r;
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
