import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { CampusServiceProxy, CampusDto } from '@shared/service-proxies/admissions-proxies';
import { CreateEditCampusDialogComponent } from './create-edit-campus/create-edit-campus-dialog.component';

@Component({
    templateUrl: './campuses.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [CreateEditCampusDialogComponent],
})
export class CampusesComponent extends AppComponentBase implements OnInit {
    items: CampusDto[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private cd: ChangeDetectorRef,
        private _service: CampusServiceProxy,
        private _modal: BsModalService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this._service.getAll().subscribe(r => {
            this.items = r?.items ?? [];
            this.loading = false;
            this.cd.detectChanges();
        });
    }

    openCreate(): void {
        const ref: BsModalRef = this._modal.show(CreateEditCampusDialogComponent, { class: 'modal-md' });
        ref.content.onSave.subscribe(() => this.load());
    }

    openEdit(item: CampusDto): void {
        const ref: BsModalRef = this._modal.show(CreateEditCampusDialogComponent, {
            class: 'modal-md',
            initialState: { id: item.id },
        });
        ref.content.onSave.subscribe(() => this.load());
    }

    delete(item: CampusDto): void {
        abp.message.confirm(`Delete "${item.name}"?`, undefined, (confirmed: boolean) => {
            if (confirmed) {
                this._service.delete(item.id).subscribe(() => {
                    abp.notify.success('Deleted successfully.');
                    this.load();
                });
            }
        });
    }
}
