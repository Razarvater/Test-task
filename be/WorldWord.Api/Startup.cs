using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;
using WorldWord.Api.Filters;
using WorldWord.Api.Services;
using WorldWord.Config;
using WorldWord.Context;
using WorldWord.Context.Interfaces;
using WorldWord.Context.Models;
using WorldWord.Context.Repositories;
using WorldWord.DTO.Converters;

namespace WorldWord.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly WorldWordConfiguration _cfg;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _cfg = Configuration.Create<WorldWordConfiguration>(_configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            services.AddSingleton(_cfg);
            services.AddScoped<WorldContext>();
            services.AddScoped<IWordRepository<Word>, WordRepository>();
            services.AddScoped<WordService>();

            DateTime configuringTime = DateTime.Now;
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "World word API",
                    Description = $"<br/> Configure time: {configuringTime}<br/>"
                });
                opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "XmlDocumentation.xml"), true);
            });

            services.AddControllers(x =>
            {
                x.Filters.Add<ExceptionFilter>();
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.MaxDepth = 1000;
                opt.UseCamelCasing(true);
                opt.SerializerSettings.Converters.Add(new RegionConverter());
                opt.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseCors(x =>
            {
                IEnumerable<Regex> allow = _cfg.AllowedAccessToApi.Split(";").Select(x => new Regex(x));
                x.SetIsOriginAllowed(y => allow.Any(z => z.IsMatch(y))).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });

            app.UsePathBase("/api/v1");
            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");

                opt.RoutePrefix = string.Empty;
                opt.DisplayRequestDuration();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}