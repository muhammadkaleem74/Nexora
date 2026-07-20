import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DatePipe } from '@angular/common';
import { AcademicYearServiceProxy, AcademicYearDto } from '@shared/service-proxies/admissions-proxies';
import { CreateEditAcademicYearDialogComponent } from './create-edit-academic-year/create-edit-academic-year-dialog.component';

@Component({
    templateUrl: './academic-years.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [DatePipe, CreateEditAcademicYearDialogComponent],
})
export class AcademicYearsComponent extends AppComponentBase implements OnInit {
    items: AcademicYearDto[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private cd: ChangeDetectorRef,
        private _service: AcademicYearServiceProxy,
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
        const ref: BsModalRef = this._modal.show(CreateEditAcademicYearDialogComponent, { class: 'modal-md' });
        ref.content.onSave.subscribe(() => this.load());
    }

    openEdit(item: AcademicYearDto): void {
        const ref: BsModalRef = this._modal.show(CreateEditAcademicYearDialogComponent, {
            class: 'modal-md',
            initialState: { id: item.id },
        });
        ref.content.onSave.subscribe(() => this.load());
    }

    delete(item: AcademicYearDto): void {
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
