using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(ApplicationDocument))]
public class ApplicationDocumentDto : EntityDto<long>
{
    public long ApplicationId { get; set; }

    public DocumentType DocumentType { get; set; }

    public string FileName { get; set; }

    public string FileUrl { get; set; }

    public int? FileSizeKb { get; set; }

    public DateTime UploadedDate { get; set; }

    public VerificationStatus VerificationStatus { get; set; }

    public long? VerifiedByUserId { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public string RejectionNote { get; set; }
}
