using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Nexora.Authorization;

public class NexoraAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

        var admissions = context.CreatePermission(PermissionNames.Pages_Admissions, L("Admissions"));

        var applications = admissions.CreateChildPermission(PermissionNames.Pages_Admissions_Applications, L("AdmissionApplications"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_Create, L("CreateAdmissionApplication"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_Edit, L("EditAdmissionApplication"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_Delete, L("DeleteAdmissionApplication"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_ChangeStatus, L("ChangeApplicationStatus"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_ManageGuardians, L("ManageApplicationGuardians"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_ManageDocuments, L("ManageApplicationDocuments"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_VerifyDocument, L("VerifyApplicationDocument"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_ManageAssessments, L("ManageApplicationAssessments"));
        applications.CreateChildPermission(PermissionNames.Pages_Admissions_Applications_RecordAssessment, L("RecordAssessmentResult"));

        var students = admissions.CreateChildPermission(PermissionNames.Pages_Admissions_Students, L("Students"));
        students.CreateChildPermission(PermissionNames.Pages_Admissions_Students_Create, L("CreateStudent"));
        students.CreateChildPermission(PermissionNames.Pages_Admissions_Students_Edit, L("EditStudent"));
        students.CreateChildPermission(PermissionNames.Pages_Admissions_Students_Delete, L("DeleteStudent"));
        students.CreateChildPermission(PermissionNames.Pages_Admissions_Students_ManageGuardians, L("ManageStudentGuardians"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, NexoraConsts.LocalizationSourceName);
    }
}
