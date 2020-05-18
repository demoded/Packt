using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcMusicStore.Models;
using Strathweb.AspNetCore.AzureBlobFileProvider;

namespace MvcMusicStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

           services.AddSession(actions =>
                {
                    actions.IdleTimeout = TimeSpan.FromSeconds(10);
                    actions.Cookie.HttpOnly = true;
                });
            services.AddMvc();
            services.AddHttpContextAccessor();

            var sqlConnBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("MusicStoreEntities"));
            sqlConnBuilder.Password = Configuration["MUSICSTORE_DB_PASSWORD"];
            services.AddDbContext<MusicStoreEntities>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(sqlConnBuilder.ConnectionString));

            sqlConnBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("ASPNETDB"));
            sqlConnBuilder.Password = Configuration["MUSICSTORE_DB_PASSWORD"];
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(sqlConnBuilder.ConnectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddCookie();


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Logon";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            var blobOptions = new AzureBlobOptions
            {
                ConnectionString = Configuration.GetConnectionString("AzureStorageStaticFiles"),
                DocumentContainer = "wwwroot"
            };
            blobOptions.ConnectionString = blobOptions.ConnectionString.Replace("MUSICSTORE_ACCOUNTKEY", Configuration["MUSICSTORE_ACCOUNTKEY"]);
            var azureBlobFileProvider = new AzureBlobFileProvider(blobOptions);
            services.AddSingleton(azureBlobFileProvider);
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            var blobFileProvider = app.ApplicationServices.GetRequiredService<AzureBlobFileProvider>();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = blobFileProvider,
                RequestPath = ""
            });

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>    
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
