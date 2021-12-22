using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SI_MicroServicos.Model;
using static SI_MicroServicos.Model._DbContext;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;

namespace SI_MicroServicos
{
    public class Startup
    {
        //readonly string CorsPolicy = "_myAllowSpecificOrigins";
        
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        
            // Documentação: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-5.0
            // services.AddCors(
            //     options => {
            //         options.AddPolicy(name: CorsPolicy, builder => builder
            //                             .AllowAnyOrigin() // uso permitido se não estiver presente .AllowCredentials() 
            //                             .AllowAnyHeader()
            //                             .AllowAnyMethod()
            //                             // .WithOrigins("http://localhost:8080") // O URL especificado não deve conter uma barra final (/). Se o URL termina com /, a comparação retorna false e nenhum cabeçalho é retornado.                                        
            //                             // .AllowCredentials() // uso permitido se não estiver presente .AllowOrigin()
            //                             // .SetPreflightMaxAge(TimeSpan.FromHours(8)) 
            //                             // .SetIsOriginAllowedToAllowWildcardSubdomains()
            //                         );
            //     }
            // );
            services.AddCors();
            services.AddControllers();  

            // Auth JWT
            // Authentication Bearer JWT
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.AddSingleton<IConfiguration>(Configuration);

            // Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "SI MicroServicos",
                        Version = "v1",
                        Description = "API para autenticação no domínio Algar, usando as credenciais do AD." +
                                        "Salvando log de quando e qual ferramenta solicitante, para devidas auditorias futuras.",
                        Contact = new OpenApiContact
                        {
                            Name = "Segurança da Informação Algartech",
                            Email = "si@algartech.com",
                            Url = new Uri("https://algartech.com")
                        }
                    });
                //Apontando o caminho do XML do Swagger
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            DatabaseConfiguration.ConnectionString = Configuration["ConnectionString"];
            //Entity
            services.AddDbContext<_DbContext>(opts =>
            opts.UseSqlServer(DatabaseConfiguration.ConnectionString));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}


            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SI MicroServicos");

            });


            app.UseHttpsRedirection();

            app.UseRouting();
            //app.UseCors("CorsPolicy");
            app.UseCors(
                options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}")
                    // .RequireCors("CorsPolicy")
                    ;
            });
        }
    }
}

