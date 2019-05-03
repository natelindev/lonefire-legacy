using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Authorization;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models;
using lonefire.Services;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown;

namespace lonefire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //数据库连接
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //修改密码加密方式为MD5
            services.AddScoped<IPasswordHasher<ApplicationUser>, LF_PasswordHasher>();

            //取消用户名格式验证以允许中文用户名
            services.AddTransient<IUserValidator<ApplicationUser>, LF_UsernameValidator<ApplicationUser>>();

            //创建identity身份信息
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //identity设置
            services.Configure<IdentityOptions>(options =>
            {
                //至少有数字和字母，至少6位，2个不同字符
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;

                //5次错误密码，锁定15分钟，新用户也可以被锁定
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                //Email验证关闭
                options.User.RequireUniqueEmail = false;
            });

            //Configure Cookies
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie save for 7 days
                options.Cookie.HttpOnly = true; //Prevent JS from accessing cookies
                options.Cookie.Expiration = TimeSpan.FromDays(30);

                // Login Path
                options.LoginPath = "/Account/Login";

                // Access Denied Path
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IFileIOHelper, FileIOHelper>();
            services.AddTransient<IToaster, Toaster>();

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddMvcOptions(options =>
            {
                //Error Message Localization
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                    (x) => "所填值 '{0}' 不符合要求");
                options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(
                    () => "请求内容不能为空");
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
                    (x) => "{0} 必须为数字");
                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(
                    (x) => "'{0}' 为必填项");
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
                    (x, y) => "所填的值'{0}' 不符合{1}的要求");
                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
                    () => "该项为必填项");
                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(
                    (x) => "所填的值不符合{0}的要求");
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                    (x) => "该值不能为空");
                options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(
                    (x) => "所填值 '{0}' 不符合要求");
                options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(
                    () => "所填值不符合要求");
                options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(
                    () => "该项必须为数字");
            })
            .AddControllersAsServices()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //Register Authorization handlers
            services.AddScoped<IAuthorizationHandler,
                          ArticleIsOwnerAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                      CommentAdministratorAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                                  ArticleAdministratorsAuthorizationHandler>();

            //Add Markdown support
            services.AddMarkdown(config =>
            {
                // Create custom MarkdigPipeline 
                // using MarkDig; for extension methods
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
                        .UsePipeTables()
                        .UseGridTables()
                        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                        .UseAutoLinks() // URLs are parsed into anchors
                        .UseAbbreviations()
                        .UseYamlFrontMatter()
                        .UseEmojiAndSmiley(true)
                        .UseListExtras()
                        .UseFigures()
                        .UseMathematics()
                        .UseTaskLists()
                        .UseCustomContainers()
                        .UseGenericAttributes();
                        //.DisableHtml();
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMarkdown();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseStatusCodePagesWithReExecute("/Home/Error/", "?statusCode={0}");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
