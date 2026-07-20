import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from 'shared/paged-listing-component-base';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Table, TableModule } from 'primeng/table';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgClass, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import {
    AdmissionApplicationServiceProxy,
    AdmissionApplicationListDto,
    ApplicationStatus,
    CampusServiceProxy,
    CampusDto,
    AcademicYearServiceProxy,
    AcademicYearDto,
} from '@shared/service-proxies/admissions-proxies';
import { CreateApplicationDialogComponent } from './create-application/create-application-dialog.component';
import { ChangeStatusDialogComponent } from './change-status/change-status-dialog.component';
import { ConvertToStudentDialogComponent } from './convert-to-student/convert-to-student-dialog.component';

@Component({
    templateUrl: './applications.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, NgClass, DatePipe, PaginatorModule, LocalizePipe],
})
export class ApplicationsComponent extends PagedListingComponentBase<AdmissionApplicationListDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    keyword = '';
    filterStatus: ApplicationStatus | undefined = undefined;
    filterCampusId: number | undefined = undefined;
    filterAcademicYearId: number | undefined = undefined;
    advancedFiltersVisible = false;

    campuses: CampusDto[] = [];
    academicYears: AcademicYearDto[] = [];

    ApplicationStatus = ApplicationStatus;

    statusOptions = [
        { label: 'All', value: undefined },
        { label: 'Draft', value: ApplicationStatus.Draft },
        { label: 'Submitted', value: ApplicationStatus.Submitted },
        { label: 'Under Review', value: ApplicationStatus.UnderReview },
        { label: 'Test Scheduled', value: ApplicationStatus.TestScheduled },
        { label: 'Approved', value: ApplicationStatus.Approved },
        { label: 'Rejected', value: ApplicationStatus.Rejected },
        { label: 'Enrolled', value: ApplicationStatus.Enrolled },
        { label: 'Withdrawn', value: ApplicationStatus.Withdrawn },
    ];

    constructor(
        injector: Injector,
        cd: ChangeDetectorRef,
        private _applicationService: AdmissionApplicationServiceProxy,
        private _campusService: CampusServiceProxy,
        private _academicYearService: AcademicYearServiceProxy,
        private _modalService: BsModalService
    ) {
        super(injector, cd);
    }

    ngOnInit(): void {
        this._campusService.getAll().subscribe(r => { this.campuses = r.items; this.cd.detectChanges(); });
        this._academicYearService.getAll().subscribe(r => { this.academicYears = r.items; this.cd.detectChanges(); });
        this.refresh();
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            if (this.primengTableHelper.records?.length > 0) return;
        }

        this.primengTableHelper.showLoadingIndicator();

        this._applicationService
            .getAll(
                this.keyword,
                this.filterStatus,
                this.filterCampusId,
                this.filterAcademicYearId,
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

    delete(record: AdmissionApplicationListDto): void {
        abp.message.confirm(
            `Delete application ${record.applicationNumber}?`,
            undefined,
            (confirmed: boolean) => {
                if (confirmed) {
                    this._applicationService.delete(record.id).subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
                }
            }
        );
    }

    createApplication(): void {
        const ref: BsModalRef = this._modalService.show(CreateApplicationDialogComponent, {
            class: 'modal-lg',
            initialState: { campuses: this.campuses, academicYears: this.academicYears },
        });
        ref.content.onSave.subscribe(() => this.refresh());
    }

    openChangeStatus(record: AdmissionApplicationListDto): void {
        const ref: BsModalRef = this._modalService.show(ChangeStatusDialogComponent, {
            class: 'modal-md',
            initialState: { applicationId: record.id, currentStatus: record.status },
        });
        ref.content.onSave.subscribe(() => this.refresh());
    }

    openConvertToStudent(record: AdmissionApplicationListDto): void {
        const ref: BsModalRef = this._modalService.show(ConvertToStudentDialogComponent, {
            class: 'modal-lg',
            initialState: {
                applicationId: record.id,
                campuses: this.campuses,
                academicYears: this.academicYears,
            },
        });
        ref.content.onSave.subscribe(() => this.refresh());
    }

    clearFilters(): void {
        this.keyword = '';
        this.filterStatus = undefined;
        this.filterCampusId = undefined;
        this.filterAcademicYearId = undefined;
        this.refresh();
    }

    getStatusClass(status: ApplicationStatus): string {
        switch (status) {
            case ApplicationStatus.Approved:
            case ApplicationStatus.Enrolled:
                return 'nx-status--active';
            case ApplicationStatus.Rejected:
            case ApplicationStatus.Withdrawn:
                return 'nx-status--inactive';
            default:
                return 'nx-status--pending';
        }
    }

    getStatusLabel(status: ApplicationStatus): string {
        return ApplicationStatus[status] ?? 'Unknown';
    }
}
