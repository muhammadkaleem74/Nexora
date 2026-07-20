namespace Nexora.Domain.Admissions.Enums;

public enum ApplicationStatus
{
    Draft = 0,       // created but not yet formally submitted
    Submitted = 1,
    UnderReview = 2,
    TestScheduled = 3,
    Approved = 4,
    Rejected = 5,
    Enrolled = 6,
    Withdrawn = 7
}
