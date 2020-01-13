using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RecipeManager.Data;
using RecipeManager.DataInterfaces;
using RecipeManager.Helpers;
using RecipeManager.Services;
using RecipeManager.ServicesInterfaces;
using RecipeManager.Settings;

namespace RecipeManager
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
            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(option =>
                {
                    option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
			services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
				builder
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials()
				.WithOrigins("http://localhost:4200");
			}));
			services.AddSignalR();
			services.AddAutoMapper();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
			services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGenericRepository, GenericRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IShoppingRepository, ShoppingRepository>();
            services.AddScoped<IFridgeRepository, FridgeRepository>();
			services.AddScoped<IPhotoService, PhotoService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IRecipeService, RecipeService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
                {
					options.RequireHttpsMetadata = false;
					options.SaveToken = true;
					options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];

							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notify")))
							{
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						}
					};
				});
        }

		public void ConfigureDevelopmentServices(IServiceCollection services)
		{
			services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddJsonOptions(option =>
				{
					option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
				});
			services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
				builder
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials()
				.WithOrigins("http://localhost:4200");
			}));
			services.AddSignalR();
			services.AddAutoMapper();
			services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
			services.AddScoped<IAuthRepository, AuthRepository>();
			services.AddScoped<IGenericRepository, GenericRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IRecipeRepository, RecipeRepository>();
			services.AddScoped<IShoppingRepository, ShoppingRepository>();
			services.AddScoped<IFridgeRepository, FridgeRepository>();
			services.AddScoped<IPhotoService, PhotoService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IRecipeService, RecipeService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = false;
					options.SaveToken = true;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
						ValidateIssuer = false,
						ValidateAudience = false
					};
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];

							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notify")))
							{
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						}
					};
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

			app.UseCors("CorsPolicy");
			app.UseSignalR(routes =>
			{
				routes.MapHub<NotifyHub>("/notify");
			});
			app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(routes =>
			{
				routes.MapSpaFallbackRoute(
					name: "spa",
					defaults: new { controller = "Fallback", action = "Index" }
				);
			});
        }
    }
}
