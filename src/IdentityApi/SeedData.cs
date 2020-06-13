// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityApi.Data;
using IdentityApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Tasks;

namespace IdentityApi
{
    public class SeedData
    {
        //
        //old seed data funtion
        //
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var alice = userMgr.FindByNameAsync("alice").Result;
                    if (alice == null)
                    {
                        alice = new ApplicationUser
                        {
                            UserName = "alice"
                        };
                        var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("alice created");
                    }
                    else
                    {
                        Log.Debug("alice already exists");
                    }

                    var bob = userMgr.FindByNameAsync("bob").Result;
                    if (bob == null)
                    {
                        bob = new ApplicationUser
                        {
                            UserName = "bob"
                        };
                        var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("bob created");
                    }
                    else
                    {
                        Log.Debug("bob already exists");
                    }
                }
            }
        }


        //
        //in-use seed data funtions
        //
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            if (!context.Roles.Any())
            {
                await EnsureCreatedRole(services, "Users");
                await EnsureCreatedRole(services, "Sales");
                await EnsureCreatedRole(services, "Managers");
                await EnsureCreatedRole(services, "Administrators");
            }

            if (!context.Users.Any())
            {
                EnsureCreatedUser(services, "alice", "Users", new ApplicationUser
                {
                    Name = "Alice Smith",
                    GivenName = "Alice",
                    FamilyName = "Smith",
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true,
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                    PictureUrl = "default_avatar.png",
                    Website = "http://alice.com",
                    Address = new Address
                    {
                        StreetAddress = "One Hacker Way",
                        Locality = "Heidelberg",
                        City = "Heidelberg",
                        Country = "Germany",
                        PostalCode = "69118"
                    }
                });

                EnsureCreatedUser(services, "ben", "Sales", new ApplicationUser
                {
                    Name = "Ben Smith",
                    GivenName = "Ben",
                    FamilyName = "Smith",
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true,
                    Email = "BenSmith@email.com",
                    EmailConfirmed = true,
                    PictureUrl = "default_avatar.png",
                    Website = "http://ben.com",
                    Address = new Address
                    {
                        StreetAddress = "One Hacker Way",
                        Locality = "Heidelberg",
                        City = "Heidelberg",
                        Country = "Germany",
                        PostalCode = "69118"
                    }
                });

                EnsureCreatedUser(services, "bob", "Managers", new ApplicationUser
                {
                    Name = "Bob Smith",
                    GivenName = "Bob",
                    FamilyName = "Smith",
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true,
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true,
                    PictureUrl = "default_avatar.png",
                    Website = "http://bob.com",
                    Address = new Address
                    {
                        StreetAddress = "One Hacker Way",
                        Locality = "Heidelberg",
                        City = "Heidelberg",
                        Country = "Germany",
                        PostalCode = "69118"
                    }
                });

                EnsureCreatedUser(services, "windy", "Administrators", new ApplicationUser
                {
                    Name = "Windy Smith",
                    GivenName = "Windy",
                    FamilyName = "Smith",
                    PhoneNumber = "0123456789",
                    PhoneNumberConfirmed = true,
                    Email = "WindySmith@email.com",
                    EmailConfirmed = true,
                    PictureUrl = "default_avatar.png",
                    Website = "http://windy.com",
                    Address = new Address
                    {
                        StreetAddress = "One Hacker Way",
                        Locality = "Heidelberg",
                        City = "Heidelberg",
                        Country = "Germany",
                        PostalCode = "69118"
                    }
                });
            }
        }

        private static void EnsureCreatedUser(IServiceProvider services, string username, string role, ApplicationUser dto)
        {
            var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

            var user = userMgr.FindByNameAsync(username).Result;
            if (user == null)
            {
                user = dto;
                user.UserName = username;
                var result = userMgr.CreateAsync(user, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(user, role).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug($"{username} created");
            }
            else
            {
                Log.Debug($"{username} already exists");
            }
        }

        private static async Task EnsureCreatedRole(IServiceProvider services, string role)
        {
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
            await roleMgr.CreateAsync(new IdentityRole(role));
        }
    }
}
