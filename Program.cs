  using Microsoft.AspNetCore.Authentication.JwtBearer;
using Blog_App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Claims;

namespace Blog_App
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddControllers();

      builder.Services.AddCors(options =>
      {
        options.AddPolicy("AllowAngularApp",
            b => b.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
        
      });

      // ✅ JSON camelCase + circular reference fix
      builder.Services.AddControllers()
          .AddJsonOptions(options =>
          {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
          });

      // ✅ JWT setup
      builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              RoleClaimType=ClaimTypes.Role,
              ValidIssuer = "https://localhost:5000",
              ValidAudience = "https://localhost:5000",
              IssuerSigningKey = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes("ThisIsAReallyLongSuperSecretKey123456!")
               )
            };
          });
      builder.Services.AddAuthorization(options =>
      {
        // Admin only access
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        // Moderator or Admin can access
        options.AddPolicy("ModeratorOnly", policy =>
            policy.RequireRole("Moderator", "Admin"));
      });

      // ✅ Database
      builder.Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      var app = builder.Build();

      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();

      app.UseStaticFiles();            // ✅ Static files should be served early

      app.UseRouting();                // ✅ Needed before CORS, Auth, etc.

      app.UseCors("AllowAngularApp");  // ✅ Allow Angular frontend requests

      app.UseAuthentication();         // ✅ After CORS and before Authorization
      app.UseAuthorization();

      app.MapControllers();            // ✅ Or app.UseEndpoints(...) depending on version


    

      app.Run();
    }
  }
}
