using EAMIS.Common.DTO;
using EAMIS.Core.BusinessLogic;
using EAMIS.Core.BusinessLogic.Masterfiles;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Ais;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.LogicRepository;
using EAMIS.Core.LogicRepository.Ais;
using EAMIS.Core.LogicRepository.Classification;
using EAMIS.Core.LogicRepository.Masterfiles;
using EAMIS.Core.Response;
using EAMIS.Core.Response.DTO;
using EAMIS.Core.TokenServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _config;
       
        public Startup(IConfiguration config)
        {
            _config = config;
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
           
            
           // DataList dataList = new Core.Response.DTO.DataList;
            _config.Bind("Authentication", authenticationConfiguration);
            services.AddSingleton(authenticationConfiguration);
            //services.AddSingleton(pageConfig);
           // services.AddSingleton(dataList)
            services.AddScoped<AccessTokenGenerator>();
            services.AddScoped<TokenGenerator>();
            services.AddScoped<RefreshTokenGenerator>();
            services.AddScoped<RefreshTokenValidator>();
            services.AddScoped<IEamisRegionRepository, EamisRegionRepository>();
            services.AddScoped<IEamisProvinceRepository, EamisProvinceRepository>();
            services.AddScoped<IEamisMunicipalityRepository, EamisMunicipalityRepository>();
            services.AddScoped<IEamisBarangayReporsitory, EamisBarangayRepository>();
            services.AddScoped<Authenticator>();
            //services.AddScoped<DataList>();
           // services.AddScoped<PageConfig>();
            services.AddScoped<IEamisUsersRepository, EamisUsersRepository>();
            services.AddScoped<IEamisUserloginRepository, EamisUserloginRepository>();
            services.AddScoped<IRefreshTokenRepositories, RefreshTokenRepository>();
            services.AddScoped<IEamisRolesRepository, EamisRolesRepository>();
            services.AddScoped<IEamisUserRolesRepository, EamisUserRolesRepository>();
			services.AddScoped<IEamisAttachmentsRepository, EamisAttachmentsRepository>();
            services.AddScoped<IEamisAttachmentTypeRepository,EamisAttachmentTypeRepository>();
            services.AddScoped<IEamisWarehouseRepository, EamisWarehouseRepository>();
            services.AddScoped<IEamisPropertySuppliesRepository, EamisPropertySuppliesRepository>();
            services.AddScoped<IEamisPrecondtionsRepository, EamisPreconditionsRepository>();
            services.AddScoped<IEamisUnitofMeasureRepository, EamisUnitofMeasureRepository>();
            services.AddScoped<IEamisProcurementCategoryRepository, EamisProcurementCategoryRepository>();
            services.AddScoped<IEamisPropertyItemsRepository, EamisPropertyItemsRepository>();
            services.AddScoped<IEamisEnvironmentalImpactsRepository, EamisEnvironmentalImpactsRepository>();
            services.AddScoped<IEamisEnvironmentalAspectsRepository, EamisEnvironmentalAspectsRepository>();
            services.AddScoped<IEamisItemCategoryRepository, EamisItemCategoryRepository>();
            services.AddScoped<IEamisItemSubCategoryRepository, EamisItemSubCategoryRepository>();
            services.AddScoped<IEamisSupplierRepository, EamisSupplierRepository>();
            services.AddScoped<IEamisFundSourceRepository, EamisFundSourceRepository>();
            services.AddScoped<IEamisGeneralFundSourceRepository, EamisGeneralFundSourceRepository>();
            services.AddScoped<IEamisChartofAccountsRepository, EamisChartofAccountsRepository>();
            services.AddScoped<IEamisGeneralChartofAccountsRepository, EamisGeneralChartofAccountsRepository>();
            services.AddScoped<IEamisClassificationRepository, EamisClassificationRepository>();
            services.AddScoped<IEamisSubClassificationRepository, EamisSubClassificationRepository>();
            services.AddScoped<IEamisGroupClassificationRepository, EamisGroupClassificationRepository>();
            services.AddScoped<IEamisAuthorizationRepository, EamisAuthorizationRepository>();
            services.AddScoped<IEamisFinancingSourceRepository, EamisFinancingSourceRepository>();

            //AIS
            services.AddScoped<IAisPersonnelRepository, AisPersonnelRepository>();
            services.AddScoped<IAisOfficeRepository, AisOfficeRepository>();
            services.AddScoped<IAisCodeListValueRepository, AisCodeListValueRepository>();
            services.AddDbContext<EAMISContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DenrEamis"));
            });
            services.AddDbContext<AISContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DenrAis"));
            });
            services.AddControllersWithViews()
                .AddViewComponentsAsServices();
            services.AddControllers(x => x.AllowEmptyInputInBodyModelBinding = true);
            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    LifetimeValidator = CustomLifetimeValidator,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                };

            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EAMIS.WebApi", Version = "v1" });
               
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EAMIS.WebApi v1"));

            }

            app.UseHttpsRedirection();
            app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "http://eamiswebapp.s3-website-ap-southeast-1.amazonaws.com"));

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeveloperExceptionPage();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }
    }
}
