// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authorization.Infrastructure;
// using MvcClient.Models;
// using MvcClient.Services;

// namespace MvcClient.Authorization
// {
//     public class OwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Item>
//     {
//         private readonly IIdentityService<Buyer> _identityService;

//         public OwnerAuthorizationHandler(IIdentityService<Buyer> identityService)
//         {
//             _identityService = identityService;
//         }

//         protected override Task HandleRequirementAsync(
//             AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Item resource)
//         {
//             if (context.User == null || resource == null)
//             {
//                 return Task.CompletedTask;
//             }

//             if (requirement.Name != Constants.CreateOperationName &&
//                 requirement.Name != Constants.ReadOperationName &&
//                 requirement.Name != Constants.UpdateOperationName &&
//                 requirement.Name != Constants.DeleteOperationName)
//             {
//                 return Task.CompletedTask;
//             }

//             var user = _identityService.Get(context.User);
//             if (user.Id == resource.OwnerId)
//             {
//                 context.Succeed(requirement);
//             }

//             return Task.CompletedTask;
//         }
//     }
// }