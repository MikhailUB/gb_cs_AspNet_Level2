using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using WebStore.Clients.Employees;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.Services;
using WebStore.Services.Data;

namespace WebStore.ServiceHosting
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
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddDbContext<WebStoreContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConection")));
			services.AddTransient<WebStoreContextInitializer>();

			services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<WebStoreContext>()
				.AddDefaultTokenProviders();

			services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();

			services.AddScoped<IProductData, SqlProductData>();
			services.AddScoped<ICartService, CookieCartService>();
			services.AddScoped<IOrderService, SqlOrdersService>();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<ICartService, CookieCartService>();

			services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("v1", new Info { Title = "WebStore.API", Version = "v1" });
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				opt.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, WebStoreContextInitializer db)
		{
			db.InitializeAsync().Wait();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(opt =>
			{
				opt.SwaggerEndpoint("/swagger/v1/swagger.json", "WebStore.API");
				opt.RoutePrefix = "";
			});

			app.UseMvc();
		}
	}
}
