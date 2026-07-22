import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_BASE_URL } from './service-proxies';

// ── Enums ────────────────────────────────────────────────────────────────────

export enum ApplicationStatus {
    Draft = 0,
    Submitted = 1,
    UnderReview = 2,
    TestScheduled = 3,
    Approved = 4,
    Rejected = 5,
    Enrolled = 6,
    Withdrawn = 7,
}

export enum GenderType {
    Male = 1,
    Female = 2,
    Other = 3,
}

export enum GuardianRelationship {
    Father = 1,
    Mother = 2,
    Guardian = 3,
    Other = 4,
}

export enum StudentStatus {
    Active = 1,
    Inactive = 2,
    Alumni = 3,
    Transferred = 4,
}

export enum PromotionStatus {
    Promoted = 1,
    Retained = 2,
    Transferred = 3,
    Withdrawn = 4,
}

export enum DocumentType {
    BirthCertificate = 1,
    PassportPhoto = 2,
    PreviousSchoolReport = 3,
    MedicalRecord = 4,
    NationalId = 5,
    Other = 99,
}

export enum VerificationStatus {
    Pending = 0,
    Verified = 1,
    Rejected = 2,
}

export enum AssessmentType {
    Written = 1,
    Oral = 2,
    Practical = 3,
    Interview = 4,
}

export enum AssessmentResult {
    Pending = 0,
    Pass = 1,
    Fail = 2,
    Conditional = 3,
}

// ── Shared paged result ───────────────────────────────────────────────────────

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
}

// ABP wraps every response: { result: T, success: true }. This unwraps it.
// Returns any so map(unwrap) preserves the outer Observable<T> annotation.
function unwrap(r: any): any {
    return r?.result ?? r;
}

// ── Reference table DTOs ──────────────────────────────────────────────────────

export interface AcademicYearDto {
    id: number;
    name: string;
    startDate: string;
    endDate: string;
    isActive: boolean;
}

export interface CampusDto {
    id: number;
    name: string;
    city: string;
    country: string;
    address: string;
    contactNumber: string;
    isActive: boolean;
}

export interface GradeLevelDto {
    id: number;
    campusId: number;
    campusName: string;
    name: string;
    code: string;
    sortOrder: number;
    isActive: boolean;
}

export interface SectionDto {
    id: number;
    gradeLevelId: number;
    gradeLevelName: string;
    name: string;
    capacity: number;
}

// ── Guardian DTOs ─────────────────────────────────────────────────────────────

export interface ApplicationGuardianDto {
    id: number;
    guardianId: number;
    fullName: string;
    relationship: GuardianRelationship;
    nationalIdNumber: string;
    email: string;
    phone: string;
    occupation: string;
    isPrimaryContact: boolean;
}

export interface StudentGuardianDto extends ApplicationGuardianDto {
    canPickupStudent: boolean;
}

// ── Application DTOs ──────────────────────────────────────────────────────────

export interface AdmissionApplicationListDto {
    id: number;
    applicationNumber: string;
    academicYearId: number;
    academicYearName: string;
    campusId: number;
    campusName: string;
    desiredGradeLevelId: number;
    desiredGradeLevelName: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: GenderType;
    status: ApplicationStatus;
    submittedDate: string;
    creationTime: string;
}

export interface AdmissionApplicationDetailDto extends AdmissionApplicationListDto {
    nationality: string;
    religion: string;
    previousSchoolName: string;
    previousSchoolBoard: string;
    previousGradeLevel: string;
    reviewedByUserId: number | undefined;
    reviewNotes: string;
    rejectionReason: string;
    guardians: ApplicationGuardianDto[];
    documents: ApplicationDocumentDto[];
    assessments: AdmissionAssessmentDto[];
}

export interface ApplicationDocumentDto {
    id: number;
    applicationId: number;
    documentType: DocumentType;
    fileName: string;
    fileUrl: string;
    fileSizeKb: number | undefined;
    uploadedDate: string;
    verificationStatus: VerificationStatus;
    verifiedByUserId: number | undefined;
    verifiedDate: string | undefined;
    rejectionNote: string;
}

export interface AdmissionAssessmentDto {
    id: number;
    applicationId: number;
    assessmentType: AssessmentType;
    scheduledDate: string | undefined;
    conductedByUserId: number | undefined;
    score: number | undefined;
    maxScore: number | undefined;
    remarks: string;
    result: AssessmentResult;
}

export interface CreateGuardianDto {
    fullName: string;
    relationship: GuardianRelationship;
    nationalIdNumber: string;
    email: string;
    phone: string;
    occupation: string;
    address: string;
    isPrimaryContact: boolean;
}

export interface CreateApplicationDocumentDto {
    documentType: DocumentType;
    fileName: string;
    fileUrl: string;
    fileSizeKb: number | undefined;
}

export interface VerifyDocumentDto {
    verificationStatus: VerificationStatus;
    rejectionNote: string;
}

export interface CreateAssessmentDto {
    assessmentType: AssessmentType;
    scheduledDate: string | undefined;
}

export interface RecordAssessmentResultDto {
    score: number | undefined;
    maxScore: number | undefined;
    remarks: string;
    result: AssessmentResult;
}

export interface CreateAdmissionApplicationDto {
    academicYearId: number;
    campusId: number;
    desiredGradeLevelId: number;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: GenderType;
    nationality: string;
    religion: string;
    previousSchoolName: string;
    previousSchoolBoard: string;
    previousGradeLevel: string;
}

export interface UpdateAdmissionApplicationDto extends CreateAdmissionApplicationDto {
    id: number;
}

export interface ChangeApplicationStatusDto {
    applicationId: number;
    newStatus: ApplicationStatus;
    notes: string;
    rejectionReason: string;
}

export interface ConvertToStudentDto {
    applicationId: number;
    registrationNumber: string;
    grNumber: string;
    campusId: number;
    currentGradeLevelId: number;
    currentSectionId: number | undefined;
    enrollmentDate: string;
    academicYearId: number;
}

// ── Student DTOs ──────────────────────────────────────────────────────────────

export interface StudentListDto {
    id: number;
    registrationNumber: string;
    grNumber: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: GenderType;
    campusId: number;
    campusName: string;
    currentGradeLevelId: number;
    currentGradeLevelName: string;
    currentSectionId: number | undefined;
    currentSectionName: string;
    enrollmentDate: string;
    status: StudentStatus;
}

export interface StudentDetailDto extends StudentListDto {
    applicationId: number | undefined;
    bloodGroup: string;
    nationality: string;
    religion: string;
    photoUrl: string;
    guardians: StudentGuardianDto[];
    enrollmentHistories: EnrollmentHistoryDto[];
}

export interface CreateStudentDto {
    applicationId: number | undefined;
    registrationNumber: string;
    grNumber: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: GenderType;
    bloodGroup: string;
    nationality: string;
    religion: string;
    campusId: number;
    currentGradeLevelId: number;
    currentSectionId: number | undefined;
    enrollmentDate: string;
    status: StudentStatus;
}

export interface EnrollmentHistoryDto {
    id: number;
    studentId: number;
    academicYearId: number;
    academicYearName: string;
    gradeLevelId: number;
    gradeLevelName: string;
    sectionId: number | undefined;
    sectionName: string;
    enrollmentDate: string;
    promotionStatus: PromotionStatus;
    remarks: string;
}

// ── Service Proxies ───────────────────────────────────────────────────────────

@Injectable()
export class AdmissionApplicationServiceProxy {
    private baseUrl: string;

    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? '';
    }

    getAll(
        keyword: string | undefined,
        status: ApplicationStatus | undefined,
        campusId: number | undefined,
        academicYearId: number | undefined,
        sorting: string | undefined,
        skipCount: number,
        maxResultCount: number
    ): Observable<PagedResult<AdmissionApplicationListDto>> {
        let url = `${this.baseUrl}/api/services/app/AdmissionApplication/GetAll?`;
        if (keyword != null) url += `Keyword=${encodeURIComponent(keyword)}&`;
        if (status != null) url += `Status=${status}&`;
        if (campusId != null) url += `CampusId=${campusId}&`;
        if (academicYearId != null) url += `AcademicYearId=${academicYearId}&`;
        if (sorting != null) url += `Sorting=${encodeURIComponent(sorting)}&`;
        url += `SkipCount=${skipCount}&MaxResultCount=${maxResultCount}`;
        return this.http.get<any>(url).pipe(map(unwrap));
    }

    get(id: number): Observable<AdmissionApplicationDetailDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/Get?id=${id}`).pipe(map(unwrap));
    }

    getDetail(id: number): Observable<AdmissionApplicationDetailDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/GetDetail?id=${id}`).pipe(map(unwrap));
    }

    create(body: CreateAdmissionApplicationDto): Observable<AdmissionApplicationListDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/Create`, body).pipe(map(unwrap));
    }

    update(body: UpdateAdmissionApplicationDto): Observable<AdmissionApplicationListDto> {
        return this.http.put<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/Update`, body).pipe(map(unwrap));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/services/app/AdmissionApplication/Delete?id=${id}`);
    }

    submitApplication(applicationId: number): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/api/services/app/AdmissionApplication/SubmitApplication?applicationId=${applicationId}`, null);
    }

    changeStatus(body: ChangeApplicationStatusDto): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/api/services/app/AdmissionApplication/ChangeStatus`, body);
    }

    convertToStudent(body: ConvertToStudentDto): Observable<StudentListDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/ConvertToStudent`, body).pipe(map(unwrap));
    }

    // ── Guardians ─────────────────────────────────────────────────────────────

    addGuardian(applicationId: number, body: CreateGuardianDto): Observable<ApplicationGuardianDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/AddGuardian?applicationId=${applicationId}`, body).pipe(map(unwrap));
    }

    removeGuardian(applicationGuardianId: number): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/api/services/app/AdmissionApplication/RemoveGuardian?applicationGuardianId=${applicationGuardianId}`, null);
    }

    getGuardians(applicationId: number): Observable<ApplicationGuardianDto[]> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/GetGuardians?applicationId=${applicationId}`).pipe(map(unwrap));
    }

    // ── Documents ─────────────────────────────────────────────────────────────

    addDocument(applicationId: number, body: CreateApplicationDocumentDto): Observable<ApplicationDocumentDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/AddDocument?applicationId=${applicationId}`, body).pipe(map(unwrap));
    }

    removeDocument(documentId: number): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/api/services/app/AdmissionApplication/RemoveDocument?documentId=${documentId}`, null);
    }

    verifyDocument(documentId: number, body: VerifyDocumentDto): Observable<ApplicationDocumentDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/VerifyDocument?documentId=${documentId}`, body).pipe(map(unwrap));
    }

    getDocuments(applicationId: number): Observable<ApplicationDocumentDto[]> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/GetDocuments?applicationId=${applicationId}`).pipe(map(unwrap));
    }

    // ── Assessments ───────────────────────────────────────────────────────────

    scheduleAssessment(applicationId: number, body: CreateAssessmentDto): Observable<AdmissionAssessmentDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/ScheduleAssessment?applicationId=${applicationId}`, body).pipe(map(unwrap));
    }

    recordAssessmentResult(assessmentId: number, body: RecordAssessmentResultDto): Observable<AdmissionAssessmentDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/RecordAssessmentResult?assessmentId=${assessmentId}`, body).pipe(map(unwrap));
    }

    getAssessments(applicationId: number): Observable<AdmissionAssessmentDto[]> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AdmissionApplication/GetAssessments?applicationId=${applicationId}`).pipe(map(unwrap));
    }
}

@Injectable()
export class StudentServiceProxy {
    private baseUrl: string;

    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? '';
    }

    getAll(
        keyword: string | undefined,
        status: StudentStatus | undefined,
        campusId: number | undefined,
        currentGradeLevelId: number | undefined,
        sorting: string | undefined,
        skipCount: number,
        maxResultCount: number
    ): Observable<PagedResult<StudentListDto>> {
        let url = `${this.baseUrl}/api/services/app/Student/GetAll?`;
        if (keyword != null) url += `Keyword=${encodeURIComponent(keyword)}&`;
        if (status != null) url += `Status=${status}&`;
        if (campusId != null) url += `CampusId=${campusId}&`;
        if (currentGradeLevelId != null) url += `CurrentGradeLevelId=${currentGradeLevelId}&`;
        if (sorting != null) url += `Sorting=${encodeURIComponent(sorting)}&`;
        url += `SkipCount=${skipCount}&MaxResultCount=${maxResultCount}`;
        return this.http.get<any>(url).pipe(map(unwrap));
    }

    getDetail(id: number): Observable<StudentDetailDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/Student/GetDetail?id=${id}`).pipe(map(unwrap));
    }

    create(body: CreateStudentDto): Observable<StudentListDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/Student/Create`, body).pipe(map(unwrap));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/services/app/Student/Delete?id=${id}`);
    }

    getEnrollmentHistory(studentId: number): Observable<EnrollmentHistoryDto[]> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/Student/GetEnrollmentHistory?studentId=${studentId}`).pipe(map(unwrap));
    }

    addGuardian(studentId: number, body: CreateGuardianDto): Observable<StudentGuardianDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/Student/AddGuardian?studentId=${studentId}`, body).pipe(map(unwrap));
    }

    removeGuardian(studentGuardianId: number): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/api/services/app/Student/RemoveGuardian?studentGuardianId=${studentGuardianId}`, null);
    }

    getGuardians(studentId: number): Observable<StudentGuardianDto[]> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/Student/GetGuardians?studentId=${studentId}`).pipe(map(unwrap));
    }
}

@Injectable()
export class AcademicYearServiceProxy {
    private baseUrl: string;
    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? '';
    }
    getAll(): Observable<PagedResult<AcademicYearDto>> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AcademicYear/GetAll?MaxResultCount=200`).pipe(map(unwrap));
    }
    get(id: number): Observable<AcademicYearDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/AcademicYear/Get?id=${id}`).pipe(map(unwrap));
    }
    create(dto: AcademicYearDto): Observable<AcademicYearDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/AcademicYear/Create`, dto).pipe(map(unwrap));
    }
    update(dto: AcademicYearDto): Observable<AcademicYearDto> {
        return this.http.put<any>(`${this.baseUrl}/api/services/app/AcademicYear/Update`, dto).pipe(map(unwrap));
    }
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/services/app/AcademicYear/Delete?id=${id}`);
    }
}

@Injectable()
export class CampusServiceProxy {
    private baseUrl: string;
    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? '';
    }
    getAll(): Observable<PagedResult<CampusDto>> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/Campus/GetAll?MaxResultCount=200`).pipe(map(unwrap));
    }
    get(id: number): Observable<CampusDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/Campus/Get?id=${id}`).pipe(map(unwrap));
    }
    create(dto: CampusDto): Observable<CampusDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/Campus/Create`, dto).pipe(map(unwrap));
    }
    update(dto: CampusDto): Observable<CampusDto> {
        return this.http.put<any>(`${this.baseUrl}/api/services/app/Campus/Update`, dto).pipe(map(unwrap));
    }
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/services/app/Campus/Delete?id=${id}`);
    }
}

@Injectable()
export class GradeLevelServiceProxy {
    private baseUrl: string;
    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? '';
    }
    getAll(campusId?: number): Observable<PagedResult<GradeLevelDto>> {
        let url = `${this.baseUrl}/api/services/app/GradeLevel/GetAll?MaxResultCount=200`;
        if (campusId != null) url += `&CampusId=${campusId}`;
        return this.http.get<any>(url).pipe(map(unwrap));
    }
    get(id: number): Observable<GradeLevelDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/GradeLevel/Get?id=${id}`).pipe(map(unwrap));
    }
    create(dto: GradeLevelDto): Observable<GradeLevelDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/GradeLevel/Create`, dto).pipe(map(unwrap));
    }
    update(dto: GradeLevelDto): Observable<GradeLevelDto> {
        return this.http.put<any>(`${this.baseUrl}/api/services/app/GradeLevel/Update`, dto).pipe(map(unwrap));
    }
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/services/app/GradeLevel/Delete?id=${id}`);
    }
}

@Injectable()
export class SectionServiceProxy {
    private baseUrl: string;
    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.baseUrl = baseUrl ?? '';
    }
    getAll(gradeLevelId?: number): Observable<PagedResult<SectionDto>> {
        let url = `${this.baseUrl}/api/services/app/Section/GetAll?MaxResultCount=200`;
        if (gradeLevelId != null) url += `&GradeLevelId=${gradeLevelId}`;
        return this.http.get<any>(url).pipe(map(unwrap));
    }
    get(id: number): Observable<SectionDto> {
        return this.http.get<any>(`${this.baseUrl}/api/services/app/Section/Get?id=${id}`).pipe(map(unwrap));
    }
    create(dto: SectionDto): Observable<SectionDto> {
        return this.http.post<any>(`${this.baseUrl}/api/services/app/Section/Create`, dto).pipe(map(unwrap));
    }
    update(dto: SectionDto): Observable<SectionDto> {
        return this.http.put<any>(`${this.baseUrl}/api/services/app/Section/Update`, dto).pipe(map(unwrap));
    }
    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/api/services/app/Section/Delete?id=${id}`);
    }
}
