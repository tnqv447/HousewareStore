using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MvcClient.Authorization
{
    public class ItemOperations
    {
        public static readonly OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = "Create" };
        public static readonly OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = "Read" };
        public static readonly OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = "Update" };
        public static readonly OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = "Delete" };

        public static readonly OperationAuthorizationRequirement Approve = new OperationAuthorizationRequirement { Name = "Approve" };
        public static readonly OperationAuthorizationRequirement Reject = new OperationAuthorizationRequirement { Name = "Reject" };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string ManagersRole = "Managers";
        public static readonly string AdministratorsRole = "Administrators";
        public static readonly string SalesRole = "Sales";
        public static readonly string UsersRole = "Users";
    }
}