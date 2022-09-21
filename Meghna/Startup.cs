using Deal.Helpers;
using Deal.Models;
using Deal.Repository;
using Deal.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal
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
            // GTR  DB context
            services.AddDbContext<GTRDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Connection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<GTRDBContext>()
                .AddDefaultTokenProviders();

            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    // Use the default property (Pascal) casing.
            //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //});

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // possible object cycle resolver 
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // Use the default property (Pascal) casing.
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowSpecificOrigin",
            //        builder => builder.WithOrigins("http://127.0.0.1:5500/"));
            //});

            services.AddSession();

            // login path
            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/SignIn";
            });

            // password configure
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.SignIn.RequireConfirmedEmail = false;
            });

            // Token life time 
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(1);
            });

            services.AddControllersWithViews();
            // services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // all servieces
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISetupRepository, SetupRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
            services.AddScoped<NPGCrypto>();

            // mail configure
            services.Configure<SMTPConfigModel>(Configuration.GetSection("SMTPConfig"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
