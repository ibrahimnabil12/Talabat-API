using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Repository.Repositories;
using Talabat.Service;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(); // for api 
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
            });
            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });



            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var erros = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                            .SelectMany(p => p.Value.Errors)
                                                            .Select(E => E.ErrorMessage).ToArray();
                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = erros
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>().
                AddEntityFrameworkStores<AppIdentityDbContext>();    // to allow dependency injections for stores
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))

                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyHeader();
                    config.AllowAnyMethod();
                    config.WithOrigins(builder.Configuration["FrontEndBaseUrl"]);
                }
           );
            });
        
           



            var app = builder.Build();
            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _context = services.GetRequiredService<StoreDbContext>();
            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _context.Database.MigrateAsync(); // updata db business
                await StoreDbContextSeed.SeedAsync(_context);
                await _IdentityDbContext.Database.MigrateAsync(); //Updata database > identity
                var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(_userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error has occured while applying Migration");
            }
            app.UseMiddleware<ExceptionMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

           app.UseCors("MyPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
