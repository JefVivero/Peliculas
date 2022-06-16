using APIPeliculas.Data;
using APIPeliculas.helpers;
using APIPeliculas.PeliculasMapper;
using APIPeliculas.Repository;
using APIPeliculas.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APIPeliculas
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
            // Creamos la cadena de conexi�n
            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Inyecci�n de las dependencias
            // Pasamos la interfaz y la implementaci�n de la misma
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            //* Agregar dependencia del token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAutoMapper(typeof(PeliculasMappers));

            // De aqu� en adelante configuraci�n de documentaci�n de nuestra API
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ApiPeliculas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Pel�culas",
                    Version = "v1",
                    Description = "Backend pel�culas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "esdjl_asse15@hotmail.com",
                        Name = "Sa�l Laguna",
                        Url = new Uri("https://mipaginaweb.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                options.SwaggerDoc("ApiPeliculasCategorias", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Categor�as Pel�culas",
                    Version = "v1",
                    Description = "Backend pel�culas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "esdjl_asse15@hotmail.com",
                        Name = "Sa�l Laguna",
                        Url = new Uri("https://mipaginaweb.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                options.SwaggerDoc("ApiPeliculasUsuarios", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Usuarios Pel�culas",
                    Version = "v1",
                    Description = "Backend pel�culas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "esdjl_asse15@hotmail.com",
                        Name = "Sa�l Laguna",
                        Url = new Uri("https://mipaginaweb.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                var archivoXMLComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXMLComentarios);
                options.IncludeXmlComments(rutaApiComentarios);

                //* Primero definir el esquema de seguridad
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticacion JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
			});

            services.AddControllers();

            //* Damos soporte para CORS
            services.AddCors();
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
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            app.UseHttpsRedirection();

            // L�nea para documentaci�n  API
            app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				// Esto nos evitar� tener que navegar a .../Swagger/nombreApi/swagger.json
				options.SwaggerEndpoint("/swagger/ApiPeliculasCategorias/swagger.json", "API Categor�as Pel�culas");
                options.SwaggerEndpoint("/swagger/ApiPeliculas/swagger.json", "API Pel�culas");
                options.SwaggerEndpoint("/swagger/ApiPeliculasUsuarios/swagger.json", "API Usuarios Pel�culas");
                // Gracias a esta l�nea no tendremos que escribor la ruta .../Swagger/index.html, cargar� inmediatamente la documentaci�n de la API.
                options.RoutePrefix = "";
			});

			app.UseRouting();

            //* Estos dos son para la autenticaci�n y autorizaci�n
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //* Damos soporte para CORS
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
