using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MvcClient.Models;
using MvcClient.Services;

namespace MvcClient.Authorization
{
    public class ItemsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Item>
    {
        private readonly IIdentityService<Buyer> _identityService;

        public ItemsAuthorizationHandler(IIdentityService<Buyer> identityService)
        {
            _identityService = identityService;
        }

        //cap quyen: neu return task.complete task ma khong co context.succeed(requirement) tuc la khong cap quyen)
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Item resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }
            var role = this.GetUserRole(context);
            switch (role)
            {
                //quyen admin ======================================================
                case (AuthorizeRole.Administrators):
                    {
                        context.Succeed(requirement); break;
                    }

                //quyen manager ====================================================
                case (AuthorizeRole.Managers):
                    {
                        //neu quyen truen vao ko thuoc quyen nao trong day thi ko cap phep
                        if (requirement.Name != Constants.ReadOperationName &&
                            requirement.Name != Constants.ApproveOperationName &&
                            requirement.Name != Constants.RejectOperationName)
                        {
                            break;
                        }
                        context.Succeed(requirement); break;
                    }

                //quyen sales =======================================================
                case (AuthorizeRole.Sales):
                    {
                        //neu quyen truen vao ko thuoc quyen nao trong day thi ko cap phep
                        if (requirement.Name != Constants.CreateOperationName &&
                            requirement.Name != Constants.ReadOperationName &&
                            requirement.Name != Constants.UpdateOperationName &&
                            requirement.Name != Constants.DeleteOperationName)
                        {
                            break;
                        }

                        var user = _identityService.Get(context.User);
                        if (user.Id == resource.OwnerId)
                        {
                            context.Succeed(requirement);
                        }
                        break;
                    }

                //quyen user =======================================================
                case (AuthorizeRole.Users):
                    {
                        break;
                    }
                //Mac dinh khong cap quyen ============================================
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        //ham tra ve role dang enum
        private AuthorizeRole GetUserRole(AuthorizationHandlerContext context)
        {
            if (context.User.IsInRole(Constants.AdministratorsRole))
                return AuthorizeRole.Administrators;
            if (context.User.IsInRole(Constants.ManagersRole))
                return AuthorizeRole.Managers;
            if (context.User.IsInRole(Constants.SalesRole))
                return AuthorizeRole.Sales;
            //khong thuoc role nao thi mac dinh la user
            return AuthorizeRole.Users;
        }
    }

}