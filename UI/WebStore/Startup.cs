using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebStore.Clients.Employees;
using WebStore.Clients.Orders;
using WebStore.Clients.Products;
using WebStore.Clients.Users;
using WebStore.Clients.Values;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Domain.Models;
using WebStore.Interfaces.Api;
using WebStore.Interfaces.Services;
using WebStore.Services;
using WebStore.Services.Data;

namespace WebStore
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddDbContext<WebStoreContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConection")));
			//services.AddTransient<WebStoreContextInitializer>();

			services.AddSingleton<IEmployeesData, EmployeesClient>();
			services.AddScoped<IProductData, ProductsClient>();
			services.AddScoped<ICartService, CookieCartService>();
			services.AddScoped<IOrderService, OrdersClient>();

			services.AddTransient<IValuesService, ValuesClient>();

			/*
			IUserRoleStore<User>,
			IUserClaimStore<User>,
			IUserPasswordStore<User>,
			IUserTwoFactorStore<User>,
			IUserEmailStore<User>,
			IUserPhoneNumberStore<User>,
			IUserLoginStore<User>,
			IUserLockoutStore<User>
			*/

			services.AddIdentity<User, IdentityRole>(options =>
				{
					// тут можно сконфигурировать cookies
				})
				//.AddEntityFrameworkStores<WebStoreContext>()
				.AddDefaultTokenProviders();

			#region Identity - собственная реализация хранилищ данных на основе WebAPI

			services.AddTransient<IUserStore<User>, UserClient>();
			services.AddTransient<IUserRoleStore<User>, UserClient>();
			services.AddTransient<IUserClaimStore<User>, UserClient>();
			services.AddTransient<IUserPasswordStore<User>, UserClient>();
			services.AddTransient<IUserTwoFactorStore<User>, UserClient>();
			services.AddTransient<IUserEmailStore<User>, UserClient>();
			services.AddTransient<IUserPhoneNumberStore<User>, UserClient>();
			services.AddTransient<IUserLoginStore<User>, UserClient>();
			services.AddTransient<IUserLockoutStore<User>, UserClient>();

			services.AddTransient<IRoleStore<IdentityRole>, RoleClient>();
			#endregion

			services.Configure<IdentityOptions>(config =>
			{
				config.Password.RequiredLength = 3;
				config.Password.RequireDigit = false;
				config.Password.RequireLowercase = false;
				config.Password.RequireUppercase = false;
				config.Password.RequireNonAlphanumeric = false;
				config.Password.RequiredUniqueChars = 3;

				config.Lockout.MaxFailedAccessAttempts = 10;
				config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
				config.Lockout.AllowedForNewUsers = true;
			});

			services.ConfigureApplicationCookie(config =>
			{
				config.Cookie.HttpOnly = true;
				config.Cookie.Expiration = TimeSpan.FromDays(100);
				config.Cookie.MaxAge = TimeSpan.FromDays(100);

				config.LoginPath = "/Account/Login";
				config.LogoutPath = "/Account/Logout";
				config.AccessDeniedPath = "/Account/AccessDenied";

				config.SlidingExpiration = true;
			});

			services.AddMvc();

			services.AddAutoMapper(options =>
			{
				options.CreateMap<Employee, Employee>();
			});

			/* или можно так
			AutoMapper.Mapper.Initialize(options =>
			{
				options.CreateMap<Employee, Employee>();
			});
			*/
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env/*, WebStoreContextInitializer db*/)
		{
			//db.InitializeAsync().Wait();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}

			app.UseStaticFiles();
			app.UseDefaultFiles();

			app.UseAuthentication();

			app.UseMvc(route => 
			{
				route.MapRoute(
					name: "areas",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

				route.MapRoute(
					name:"default",
					template:"{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
