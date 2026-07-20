import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { SectionServiceProxy, SectionDto, GradeLevelServiceProxy, GradeLevelDto } from '@shared/service-proxies/admissions-proxies';
import { CreateEditSectionDialogComponent } from './create-edit-section/create-edit-section-dialog.component';

@Component({
    templateUrl: './sections.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [CreateEditSectionDialogComponent],
})
export class SectionsComponent extends AppComponentBase implements OnInit {
    items: SectionDto[] = [];
    gradeLevels: GradeLevelDto[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private cd: ChangeDetectorRef,
        private _service: SectionServiceProxy,
        private _gradeLevelService: GradeLevelServiceProxy,
        private _modal: BsModalService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._gradeLevelService.getAll().subscribe(r => { this.gradeLevels = r.items; this.cd.detectChanges(); });
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
        const ref: BsModalRef = this._modal.show(CreateEditSectionDialogComponent, {
            class: 'modal-md',
            initialState: { gradeLevels: this.gradeLevels },
        });
        ref.content.onSave.subscribe(() => this.load());
    }

    openEdit(item: SectionDto): void {
        const ref: BsModalRef = this._modal.show(CreateEditSectionDialogComponent, {
            class: 'modal-md',
            initialState: { id: item.id, gradeLevels: this.gradeLevels },
        });
        ref.content.onSave.subscribe(() => this.load());
    }

    delete(item: SectionDto): void {
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
