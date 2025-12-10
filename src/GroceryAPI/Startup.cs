using System.Text;
using Application.Commands;
using Application.Interfaces;
using Application.Queries;
using Domain.Interfaces;
using GroceryAPI.Middleware;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GroceryAPI;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		// Add Controllers
		services.AddControllers();

		// Configure Database
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

		// Configure JWT Authentication
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = Configuration["Jwt:Issuer"],
					ValidAudience = Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!))
				};
			});

		// Register Repositories
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IStoreRepository, StoreRepository>();
		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddScoped<ICartRepository, CartRepository>();
		services.AddScoped<IOrderRepository, OrderRepository>();

		// Register Services
		services.AddScoped<IPaymentService, PaymentService>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<TokenService>();

		// Register Command Handlers
		services.AddScoped<RegisterUserCommandHandler>();
		services.AddScoped<AddToCartCommandHandler>();
		services.AddScoped<CreateOrderCommandHandler>();

		// Register Query Handlers
		services.AddScoped<GetStoresQueryHandler>();
		services.AddScoped<GetProductsQueryHandler>();
		services.AddScoped<GetOrderHistoryQueryHandler>();

		// Configure Swagger
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Grocery Shopping API",
				Version = "v1",
				Description = "A comprehensive API for grocery shopping application with authentication, cart management, and order processing",
				Contact = new OpenApiContact
				{
					Name = "Grocery API Team",
					Email = "support@groceryapi.com"
				}
			});

			// Add JWT Authentication to Swagger
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJhbGc...'",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});

		// Configure CORS
		services.AddCors(options =>
		{
			options.AddPolicy("AllowAll", builder =>
			{
				builder.AllowAnyOrigin()
					   .AllowAnyMethod()
					   .AllowAnyHeader();
			});
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		// Development Environment Configuration
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Grocery Shopping API v1");
				c.RoutePrefix = string.Empty; // Swagger at root URL
				c.DocumentTitle = "Grocery Shopping API";
			});
		}

		// Exception Handling Middleware
		app.UseMiddleware<ExceptionHandlingMiddleware>();

		app.UseHttpsRedirection();

		app.UseRouting();

		app.UseCors("AllowAll");

		// Authentication & Authorization
		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});

		// Initialize Database with Seed Data
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			DbInitializer.InitializeAsync(context).Wait();
		}
	}
}