using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using EmployeeManagment.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmployeeManagment
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        options.Password.RequiredLength = 4;
                        options.Password.RequiredUniqueChars = 1;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                    })
                    .AddEntityFrameworkStores<AppDbContext>();

            services.AddMvc(options => 
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));

                options.EnableEndpointRouting = false;
            });

            services.AddAuthentication().AddGoogle(options => 
            {
                options.ClientId = "2571592995-1d5sh0t9p2ao8s6is3r2fa46tmj9s3vg.apps.googleusercontent.com";
                options.ClientSecret = "zx-o7uE7KXennazj_9HN7-Yj";
            });

            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRolePolicy", policy => 
                    policy.RequireAssertion(context => AuthorizeAdminAccess(context)));

                options.AddPolicy("DeleteRolePolicy", policy =>
                    policy.RequireAssertion(context => AuthorizeAccess(context, "Delete Role")));

                options.AddPolicy("EditRolePolicy", policy =>
                     policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                options.AddPolicy("CreateRolePolicy", policy =>
                    policy.RequireAssertion(context => AuthorizeAccess(context, "Create Role")));
            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            services.AddSingleton<IAuthorizationHandler, EditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseExceptionHandler("/Error");

            app.UseRouting();

            app.UseFileServer();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }

        private bool AuthorizeAccess(AuthorizationHandlerContext context, string role)
        {
            return context.User.IsInRole("Admin") &&
                    context.User.HasClaim(claim => claim.Type == role && claim.Value == "true") ||
                    context.User.IsInRole("Super Admin");
        }

        private bool AuthorizeAdminAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") ||
                    context.User.IsInRole("Super Admin");
        }
    }
}
