using AutoMapper;
using Nexora.Domain.Admissions;

namespace Nexora.Admissions.Dto;

public class AdmissionsMapProfile : Profile
{
    public AdmissionsMapProfile()
    {
        // Reference tables (bidirectional, handled by [AutoMap] attributes on the DTOs)
        CreateMap<AcademicYear, AcademicYearDto>().ReverseMap();
        CreateMap<Campus, CampusDto>().ReverseMap();

        CreateMap<GradeLevel, GradeLevelDto>()
            .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.Name : null))
            .ReverseMap()
            .ForMember(d => d.Campus, opt => opt.Ignore());

        CreateMap<Section, SectionDto>()
            .ForMember(d => d.GradeLevelName, opt => opt.MapFrom(s => s.GradeLevel != null ? s.GradeLevel.Name : null))
            .ReverseMap()
            .ForMember(d => d.GradeLevel, opt => opt.Ignore());

        // Guardian
        CreateMap<Guardian, GuardianDto>().ReverseMap();
        CreateMap<CreateGuardianDto, Guardian>();

        // ApplicationGuardian (flattened)
        CreateMap<ApplicationGuardian, ApplicationGuardianDto>()
            .ForMember(d => d.GuardianId, opt => opt.MapFrom(s => s.GuardianId))
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Guardian.FullName))
            .ForMember(d => d.Relationship, opt => opt.MapFrom(s => s.Guardian.Relationship))
            .ForMember(d => d.NationalIdNumber, opt => opt.MapFrom(s => s.Guardian.NationalIdNumber))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Guardian.Email))
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Guardian.Phone))
            .ForMember(d => d.Occupation, opt => opt.MapFrom(s => s.Guardian.Occupation));

        // StudentGuardian (flattened)
        CreateMap<StudentGuardian, StudentGuardianDto>()
            .ForMember(d => d.GuardianId, opt => opt.MapFrom(s => s.GuardianId))
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Guardian.FullName))
            .ForMember(d => d.Relationship, opt => opt.MapFrom(s => s.Guardian.Relationship))
            .ForMember(d => d.NationalIdNumber, opt => opt.MapFrom(s => s.Guardian.NationalIdNumber))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Guardian.Email))
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Guardian.Phone))
            .ForMember(d => d.Occupation, opt => opt.MapFrom(s => s.Guardian.Occupation));

        // ApplicationDocument and Assessment
        CreateMap<ApplicationDocument, ApplicationDocumentDto>();
        CreateMap<AdmissionAssessment, AdmissionAssessmentDto>();

        // AdmissionApplication → list DTO (flat nav properties)
        CreateMap<AdmissionApplication, AdmissionApplicationListDto>()
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
            .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.Name : null))
            .ForMember(d => d.DesiredGradeLevelName, opt => opt.MapFrom(s => s.DesiredGradeLevel != null ? s.DesiredGradeLevel.Name : null));

        // AdmissionApplication → detail DTO (includes collections)
        CreateMap<AdmissionApplication, AdmissionApplicationDetailDto>()
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
            .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.Name : null))
            .ForMember(d => d.DesiredGradeLevelName, opt => opt.MapFrom(s => s.DesiredGradeLevel != null ? s.DesiredGradeLevel.Name : null))
            .ForMember(d => d.Guardians, opt => opt.MapFrom(s => s.ApplicationGuardians))
            .ForMember(d => d.Documents, opt => opt.MapFrom(s => s.Documents))
            .ForMember(d => d.Assessments, opt => opt.MapFrom(s => s.Assessments));

        // Create/Update → Entity
        CreateMap<CreateAdmissionApplicationDto, AdmissionApplication>()
            .ForMember(d => d.ApplicationNumber, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.SubmittedDate, opt => opt.Ignore());

        CreateMap<UpdateAdmissionApplicationDto, AdmissionApplication>();

        // Student → list DTO
        CreateMap<Student, StudentListDto>()
            .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.Name : null))
            .ForMember(d => d.CurrentGradeLevelName, opt => opt.MapFrom(s => s.CurrentGradeLevel != null ? s.CurrentGradeLevel.Name : null))
            .ForMember(d => d.CurrentSectionName, opt => opt.MapFrom(s => s.CurrentSection != null ? s.CurrentSection.Name : null));

        // Student → detail DTO
        CreateMap<Student, StudentDetailDto>()
            .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.Name : null))
            .ForMember(d => d.CurrentGradeLevelName, opt => opt.MapFrom(s => s.CurrentGradeLevel != null ? s.CurrentGradeLevel.Name : null))
            .ForMember(d => d.CurrentSectionName, opt => opt.MapFrom(s => s.CurrentSection != null ? s.CurrentSection.Name : null))
            .ForMember(d => d.Guardians, opt => opt.MapFrom(s => s.StudentGuardians))
            .ForMember(d => d.EnrollmentHistories, opt => opt.MapFrom(s => s.EnrollmentHistories));

        // Create/Update → Entity
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>()
            .ForMember(d => d.RegistrationNumber, opt => opt.Ignore())
            .ForMember(d => d.GRNumber, opt => opt.Ignore())
            .ForMember(d => d.ApplicationId, opt => opt.Ignore())
            .ForMember(d => d.EnrollmentDate, opt => opt.Ignore());

        // EnrollmentHistory
        CreateMap<EnrollmentHistory, EnrollmentHistoryDto>()
            .ForMember(d => d.AcademicYearName, opt => opt.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
            .ForMember(d => d.GradeLevelName, opt => opt.MapFrom(s => s.GradeLevel != null ? s.GradeLevel.Name : null))
            .ForMember(d => d.SectionName, opt => opt.MapFrom(s => s.Section != null ? s.Section.Name : null));
    }
}
