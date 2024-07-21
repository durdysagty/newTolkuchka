using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using newTolkuchka;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using System.Globalization;
using System.Text;

string con = Secrets.dbConnection;
int accessLevels = 4;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(con));
builder.Services.AddCors(o => o.AddPolicy("Policy", p =>
{
    p.AllowAnyMethod()
     .AllowAnyHeader()
     //.WithOrigins("http://localhost:3000", "https://localhost:3000")
     .WithOrigins("http://localhost:3000", "https://localhost:3000", "http://cms.tolkuchka.bar", "https://cms.tolkuchka.bar", "http://www.cms.tolkuchka.bar", "https://www.cms.tolkuchka.bar")
     .AllowCredentials();
}));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(IJwt.jwtKey)),
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
builder.Services.AddAuthorization(options =>
{
    int max = accessLevels;
    while (accessLevels > 0)
    {
        int level = accessLevels;
        List<string> levels = new();
        while (max >= level)
        {
            levels.Add(level.ToString());
            level++;
        }
        options.AddPolicy($"Level{accessLevels}", policy => policy.RequireClaim("accesslevel", levels));
        accessLevels--;
    }
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
#region myservices
builder.Services.AddHostedService<WeeklyTaskService>();
builder.Services.AddSingleton<IPath, PathService>();
builder.Services.AddScoped<IArticle, ArticleService>();
builder.Services.AddScoped<IBrand, BrandService>();
builder.Services.AddScoped<IBreadcrumbs, BreadcrumbsService>();
builder.Services.AddScoped<ICategory, CategoryService>();
builder.Services.AddScoped<ICacheClean, CacheCleanService>();
builder.Services.AddScoped<IContent, ContentService>();
builder.Services.AddScoped<ICrypto, CryptoService>();
builder.Services.AddScoped<IActionNoFile<Currency, AdminCurrency>, CurrencyService>();
builder.Services.AddScoped<ICustomerGuid, CustomerGuidService>();
builder.Services.AddScoped<IEntry, EntryService>();
builder.Services.AddScoped<IEmployee, EmployeeService>();
builder.Services.AddScoped<IActionNoFile<Heading, Heading>, HeadingService>();
builder.Services.AddScoped<IInvoice, InvoiceService>();
builder.Services.AddScoped<IImage, ImageService>();
builder.Services.AddScoped<IJwt, JwtService>();
builder.Services.AddScoped<ILine, LineService>();
builder.Services.AddScoped<ILogin, LoginService>();
builder.Services.AddScoped<IMail, MailService>();
builder.Services.AddScoped<IModel, ModelService>();
builder.Services.AddScoped<IOrder, OrderService>();
builder.Services.AddScoped<IPosition, PositionService>();
builder.Services.AddScoped<IPurchase, PurchaseService>();
builder.Services.AddScoped<IPurchaseInvoice, PurchaseInvoiceService>();
builder.Services.AddScoped<IProduct, ProductService>();
builder.Services.AddScoped<IPromotion, PromotionService>();
builder.Services.AddScoped<IReport, ReportService>();
builder.Services.AddScoped<ISiteMap, SiteMapService>();
builder.Services.AddScoped<ISlide, SlideService>();
builder.Services.AddScoped<ISpec, SpecService>();
builder.Services.AddScoped<ISpecsValue, SpecsValueService>();
builder.Services.AddScoped<ISpecsValueMod, SpecsValueModService>();
builder.Services.AddScoped<ISupplier, SupplierService>();
builder.Services.AddScoped<IType, TypeService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IWarranty, WarrantyService>();
#endregion
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("ru"),
        new CultureInfo("en"),
        new CultureInfo("tk")
    };

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.AddInitialRequestCultureProvider(new CultureProvider());
});
builder.Services.AddLocalization(op => op.ResourcesPath = "Reces");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseRequestLocalization();
app.UseCookiePolicy();
app.Use(async (context, next) =>
{
    if (ConstantsService.Test > 1)
        context.Abort();
    // used for exclude file paths
    if (!context.Request.Path.Value.Contains('.'))
    {
        // for users
        if (!context.Request.Headers.Authorization.Any())
        {
            ICrypto _crypto = context.RequestServices.GetService<ICrypto>();
            string u = context.Request.Cookies[Secrets.userUniqCookie];
            if (string.IsNullOrEmpty(u))
                u = _crypto.CreateUserUniqCookie();
            context.Response.Cookies.Append(Secrets.userUniqCookie, u, new CookieOptions
            {
                MaxAge = new TimeSpan(365, 0, 0, 0),
                // remove on publish
#if !DEBUG
                SameSite = SameSiteMode.Strict,
                Domain = CultureProvider.Host,
                Secure = true
#endif
            });
            string t = context.Request.Cookies[Secrets.userTokenCookie];
            string h = context.Request.Cookies[Secrets.userHashCookie];
            if (!string.IsNullOrEmpty(t) && !string.IsNullOrEmpty(h))
            {
                IMemoryCache _memoryCache = context.RequestServices.GetService<IMemoryCache>();
                int userId = int.Parse(_crypto.DecryptString(h).Split(" ")[0]);
                _memoryCache.TryGetValue(ConstantsService.UserHashKey(userId), out string testHash);
                if (!string.IsNullOrEmpty(testHash))
                {
                    if (h == testHash)
                        context.Request.Headers.Append("Authorization", "Bearer " + t);
                    else
                    {
                        context.Response.Cookies.Delete(Secrets.userTokenCookie);
                        context.Response.Cookies.Delete(Secrets.userHashCookie);
                    }
                }
                else
                {
                    context.Response.Cookies.Delete(Secrets.userTokenCookie);
                    context.Response.Cookies.Delete(Secrets.userHashCookie);
                }
            }
        }
        // for employees
        else
        {
            string h = context.Request.Cookies[Secrets.empHashCookie];
            if (!string.IsNullOrEmpty(h))
            {
                IMemoryCache _memoryCache = context.RequestServices.GetService<IMemoryCache>();
                string[] value = h.Split("-");
                if (value.Length != 2)
                    _ = context.Request.Headers.Remove("Authorization");
                else
                {
                    int empId = int.Parse(value[0]);
                    _memoryCache.TryGetValue(ConstantsService.EmpHashKey(empId), out string testHash);
                    if (h != testHash)
                        _ = context.Request.Headers.Remove("Authorization");
                }
            }
            else
                _ = context.Request.Headers.Remove("Authorization");

        }
    }
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Request.Path = "/404";
        await next();
    }
});
//app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
    }
});
app.UseRouting();
app.UseCors("Policy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();