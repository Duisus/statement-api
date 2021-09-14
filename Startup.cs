using System.Reflection;
using AutoMapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Server.Repository;
using Server.Repository.Interfaces;
using Server.Managers;
using Server.Managers.Interfaces;
using Server.Profiles;
using Server.Settings;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(Configuration.GetValue<string>("ConnectionString"))
                    .ScanIn(Assembly.GetExecutingAssembly()).For.All())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

             services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Server", Version = "v1"}); });


            //Репозитории
            services
                .AddTransient<IStudentRepository, StudentRepository>()
                .AddTransient<ISpecializationRepository, SpecializationRepository>()
                .AddTransient<ICourseRepository, CourseRepository>()
                .AddTransient<IAssignmentRepository, AssignmentRepository>()
                .AddTransient<IFileRepository, FileRepository>();

            //Managers
            services
                .AddTransient<ICsvParserManager, CsvParserManager>()
                .AddTransient<IUploadManager, UploadManager>();
            
            //Настройки
            services
                .AddTransient<IAppSettings, AppSettings>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MainProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSerilogRequestLogging();

            using var scope = app.ApplicationServices.CreateScope();
            var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();
            migrator?.MigrateUp();
        }
    }
}