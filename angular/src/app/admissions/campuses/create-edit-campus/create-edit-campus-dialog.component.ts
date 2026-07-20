import { ChangeDetectorRef, Component, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { CampusServiceProxy, CampusDto } from '@shared/service-proxies/admissions-proxies';

@Component({
    templateUrl: './create-edit-campus-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateEditCampusDialogComponent extends AppComponentBase implements OnInit {
    @Input() id?: number;
    @Output() onSave = new EventEmitter<void>();

    isEdit = false;
    saving = false;

    model: CampusDto = {
        id: 0,
        name: '',
        city: '',
        country: '',
        address: '',
        contactNumber: '',
        isActive: true,
    };

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef,
        private _service: CampusServiceProxy
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
