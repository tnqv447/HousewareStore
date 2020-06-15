using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityApi.Models;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        /// <summary>
        /// Entry point into the register workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            var vm = await BuildRegisterViewModelAsync(returnUrl);
            return View(vm);
        }

        /// <summary>
        /// Handle register action
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInputModel model, string button, string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            //if the user clicked "cancel" button
            if (button != "register")
            {
                if (context != null)
                {
                    if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            //begin register
            if (ModelState.IsValid)
            {

                var result = true;
                if (model.Password.Equals(model.ConfirmPassword))
                {

                    // Console.WriteLine("\ngg\n");
                    // Console.WriteLine(model.Username);
                    // Console.WriteLine(model.Password);
                    // Console.WriteLine(returnUrl);
                    // Console.WriteLine(model.RememberMe);
                    // Console.WriteLine(model.UserProfile.Name);
                    //var temp = this.CreateClaims(model.UserProfile);

                    result = this.CreateUser(model.Username, model.Password, model.Role, model.UserProfile);
                }
                else
                {
                    result = false;
                }
                //if success
                if (result)
                {
                    var loginModel = new LoginInputModel
                    {
                        Username = model.Username,
                        Password = model.Password,
                        RememberLogin = model.RememberMe,
                        ReturnUrl = returnUrl
                    };
                    return await this.Login(loginModel, "login");
                }
                //if fail
                else
                {
                    ModelState.AddModelError(string.Empty, AccountOptions.RegisterErrorMessage);
                }
            }
            //show errors
            var vm = await BuildRegisterViewModelAsync(model);

            return View(vm);
        }

        //ham tao list Claims tu UserProfileInputModel
        // private Claim[] CreateClaims(UserProfileInputModel profile)
        // {
        //     return new Claim[]{
        //             new Claim(JwtClaimTypes.Name, profile.Name),
        //             new Claim(JwtClaimTypes.GivenName, profile.GivenName),
        //             new Claim(JwtClaimTypes.FamilyName, profile.FamilyName),
        //             new Claim(JwtClaimTypes.PhoneNumber, profile.PhoneNumber),
        //             new Claim(JwtClaimTypes.PhoneNumberVerified, "true", ClaimValueTypes.Boolean),
        //             new Claim(JwtClaimTypes.Email, profile.Email),
        //             new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
        //             new Claim(JwtClaimTypes.Picture, profile.PictureUrl.IsNullOrEmpty()?"default_avatar.png":profile.PictureUrl),
        //             new Claim(JwtClaimTypes.WebSite, profile.Website),
        //             new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(profile.Address), IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
        //         };
        // }
        private ApplicationUser TransferDataToUser(UserProfileInputModel profile)
        {
            var user = new ApplicationUser
            {
                //UserName = profile.UserName,     --------// username khong ton tai trong profile
                Name = profile.Name,
                GivenName = profile.GivenName,
                FamilyName = profile.FamilyName,
                Email = profile.Email,
                EmailConfirmed = true,
                PhoneNumber = profile.PhoneNumber,
                PhoneNumberConfirmed = true,
                PictureUrl = profile.PictureUrl,
                Website = profile.Website,
                Address = profile.Address
            };
            return user;
        }

        //ham tao user tra ve boolean la ket qua
        private bool CreateUser(string username, string password, string role, UserProfileInputModel profile)
        {
            var user = _userManager.FindByNameAsync(username).Result;
            var success = true;
            if (user == null)
            {
                user = this.TransferDataToUser(profile);
                user.UserName = username;
                var result = _userManager.CreateAsync(user, password).Result;
                if (!result.Succeeded)
                {
                    success = false;
                    throw new Exception(result.Errors.First().Description);
                }

                result = _userManager.AddToRoleAsync(user, role).Result;
                if (!result.Succeeded)
                {
                    success = false;
                    throw new Exception(result.Errors.First().Description);
                }
                // result = _userManager.AddClaimsAsync(user, claims).Result;
                // if (!result.Succeeded)
                // {
                //     success = false;
                //     throw new Exception(result.Errors.First().Description);
                // }
                Log.Debug($"{username} created");
                return success;
            }
            else
            {
                Log.Debug($"{username} already exists");
                return false;
            }
        }
        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("http://localhost:5002");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("http://localhost:5002");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                   (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<RegisterViewModel> BuildRegisterViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            return CreateTestUserInput("testuser1", returnUrl);

            // return new RegisterViewModel
            // {
            //     AllowRememberLogin = AccountOptions.AllowRememberLogin,
            //     EnableLocalRegister = AccountOptions.AllowLocalRegister,
            //     ReturnUrl = returnUrl
            // };
        }
        private RegisterViewModel CreateTestUserInput(string username, string returnUrl)
        {
            var address = new Address
            {
                StreetAddress = "123/45",
                Locality = "etc",
                City = "Some thing",
                Country = "Earth"
            };
            var profile = new UserProfileInputModel
            {
                Name = "Test User 1",
                GivenName = "User 1",
                FamilyName = "Test",
                PhoneNumber = "0123456789",
                Email = "user1@gmail.com",
                Website = "www.user1.com",
            };
            profile.Address = address;

            var vm = new RegisterViewModel
            {
                Username = username,
                Password = "12345",
                ConfirmPassword = "12345",
                Role = "Customer",
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalRegister = AccountOptions.AllowLocalRegister,
                ReturnUrl = returnUrl
            };
            vm.UserProfile = profile;

            return vm;
        }
        private async Task<RegisterViewModel> BuildRegisterViewModelAsync(RegisterInputModel model)
        {
            var vm = await BuildRegisterViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.Password = null;
            vm.ConfirmPassword = null;
            vm.RememberMe = model.RememberMe;
            vm.UserProfile = model.UserProfile;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}