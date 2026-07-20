using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Domain.Admissions;

public class ApplicationDocument : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long ApplicationId { get; set; }

    public DocumentType DocumentType { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; }

    [StringLength(500)]
    public string FileUrl { get; set; }

    public int? FileSizeKb { get; set; }

    public DateTime UploadedDate { get; set; }

    public VerificationStatus VerificationStatus { get; set; }

    public long? VerifiedByUserId { get; set; }

    public DateTime? VerifiedDate { get; set; }

    [StringLength(300)]
    public string RejectionNote { get; set; }

    public virtual AdmissionApplication Application { get; set; }
}
