import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GradeLevelServiceProxy, GradeLevelDto, CampusServiceProxy, CampusDto } from '@shared/service-proxies/admissions-proxies';
import { CreateEditGradeLevelDialogComponent } from './create-edit-grade-level/create-edit-grade-level-dialog.component';

@Component({
    templateUrl: './grade-levels.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [CreateEditGradeLevelDialogComponent],
})
export class GradeLevelsComponent extends AppComponentBase implements OnInit {
    items: GradeLevelDto[] = [];
    campuses: CampusDto[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private cd: ChangeDetectorRef,
        private _service: GradeLevelServiceProxy,
        private _campusService: CampusServiceProxy,
        private _modal: BsModalService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._campusService.getAll().subscribe(r => { this.campuses = r.items; this.cd.detectChanges(); });
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
        const ref: BsModalRef = this._modal.show(CreateEditGradeLevelDialogComponent, {
            class: 'modal-md',
            initialState: { campuses: this.campuses },
        });
        ref.content.onSave.subscribe(() => this.load());
    }

    openEdit(item: GradeLevelDto): void {
        const ref: BsModalRef = this._modal.show(CreateEditGradeLevelDialogComponent, {
            class: 'modal-md',
            initialState: { id: item.id, campuses: this.campuses },
        });
        ref.content.onSave.subscribe(() => this.load());
    }

    delete(item: GradeLevelDto): void {
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
