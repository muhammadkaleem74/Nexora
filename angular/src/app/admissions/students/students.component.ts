import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgClass, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import {
    StudentServiceProxy,
    StudentListDto,
    StudentStatus,
    CampusServiceProxy,
    CampusDto,
    GradeLevelServiceProxy,
    GradeLevelDto,
} from '@shared/service-proxies/admissions-proxies';
import { CreateStudentDialogComponent } from './create-student/create-student-dialog.component';

@Component({
    templateUrl: './students.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, NgClass, DatePipe, PaginatorModule, LocalizePipe],
})
export class StudentsComponent extends PagedListingComponentBase<StudentListDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    keyword = '';
    filterStatus: StudentStatus | undefined = undefined;
    filterCampusId: number | undefined = undefined;
    filterGradeLevelId: number | undefined = undefined;
    advancedFiltersVisible = false;

    campuses: CampusDto[] = [];
    gradeLevels: GradeLevelDto[] = [];

    StudentStatus = StudentStatus;

    statusOptions = [
        { label: 'All', value: undefined },
        { label: 'Active', value: StudentStatus.Active },
        { label: 'Inactive', value: StudentStatus.Inactive },
        { label: 'Alumni', value: StudentStatus.Alumni },
        { label: 'Transferred', value: StudentStatus.Transferred },
    ];

    constructor(
        injector: Injector,
        cd: ChangeDetectorRef,
        private _studentService: StudentServiceProxy,
        private _campusService: CampusServiceProxy,
        private _gradeLevelService: GradeLevelServiceProxy,
        private _modalService: BsModalService,
        private _router: Router
    ) {
        super(injector, cd);
    }

    ngOnInit(): void {
        this._campusService.getAll().subscribe(r => { this.campuses = r.items; this.cd.detectChanges(); });
        this._gradeLevelService.getAll().subscribe(r => { this.gradeLevels = r.items; this.cd.detectChanges(); });
        this.refresh();
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            if (this.primengTableHelper.records?.length > 0) return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._studentService
            .getAll(
                this.keyword,
                this.filterStatus,
                this.filterCampusId,
                this.filterGradeLevelId,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe(result => {
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(record: StudentListDto): void {
        abp.message.confirm(
            `Remove student ${record.firstName} ${record.lastName}?`,
            undefined,
            (confirmed: boolean) => {
                if (confirmed) {
                    this._studentService.delete(record.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
                }
            }
        );
    }

    createStudent(): void {
        const ref: BsModalRef = this._modalService.show(CreateStudentDialogComponent, {
            class: 'modal-lg',
            initialState: { campuses: this.campuses },
        });
        ref.content.onSave.subscribe(() => this.refresh());
    }

    viewDetail(record: StudentListDto): void {
        this._router.navigate(['/app/admissions/students', record.id]);
    }

    clearFilters(): void {
        this.keyword = '';
        this.filterStatus = undefined;
        this.filterCampusId = undefined;
        this.filterGradeLevelId = undefined;
        this.refresh();
    }

    getStatusClass(status: StudentStatus): string {
        switch (status) {
            case StudentStatus.Active: return 'nx-status--active';
            case StudentStatus.Inactive: return 'nx-status--inactive';
            case StudentStatus.Alumni: return 'nx-status--pending';
            case StudentStatus.Transferred: return 'nx-status--pending';
            default: return 'nx-status--inactive';
        }
    }

    getStatusLabel(status: StudentStatus): string {
        return StudentStatus[status] ?? 'Unknown';
    }
}
