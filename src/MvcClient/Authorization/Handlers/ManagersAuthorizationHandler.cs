// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authorization.Infrastructure;
// using MvcClient.Models;

// namespace MvcClient.Authorization
// {
//     public class ManagersAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Item>
//     {
//         protected override Task HandleRequirementAsync(
//             AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Item resource)
//         {
//             if (context.User == null || resource == null)
//             {
//                 return Task.CompletedTask;
//             }

//             if (requirement.Name != Constants.ReadOperationName &&
//                 requirement.Name != Constants.ApproveOperationName &&
//                 requirement.Name != Constants.RejectOperationName)
//             {
//                 return Task.CompletedTask;
//             }

//             if (context.User.IsInRole(Constants.ManagersRole))
//             {
//                 context.Succeed(requirement);
//             }

//             return Task.CompletedTask;
//         }
//     }
// }