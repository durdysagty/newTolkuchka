using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using newTolkuchka;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
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
builder.Services.AddScoped<IBrand, BrandService>();
builder.Services.AddScoped<IBreadcrumbs, BreadcrumbsService>();
builder.Services.AddScoped<ICategory, CategoryService>();
builder.Services.AddScoped<IContent, ContentService>();
builder.Services.AddScoped<ICrypto, CryptoService>();
builder.Services.AddScoped<IActionNoFile<Currency, AdminCurrency>, CurrencyService>();
builder.Services.AddScoped<IEntry, EntryService>();
builder.Services.AddScoped<IEmployee, EmployeeService>();
builder.Services.AddScoped<IInvoice, InvoiceService>();
builder.Services.AddScoped<IImage, ImageService>();
builder.Services.AddScoped<IJwt, JwtService>();
builder.Services.AddScoped<ILine, LineService>();
builder.Services.AddScoped<ILogin, LoginService>();
builder.Services.AddScoped<IMail, MailService>();
builder.Services.AddScoped<IModel, ModelService>();
builder.Services.AddScoped<IOrder, OrderService>();
builder.Services.AddScoped<IPath, PathService>();
builder.Services.AddScoped<IPosition, PositionService>();
builder.Services.AddScoped<IPurchase, PurchaseService>();
builder.Services.AddScoped<IPurchaseInvoice, PurchaseInvoiceService>();
builder.Services.AddScoped<IProduct, ProductService>();
builder.Services.AddScoped<IReport, ReportService>();
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
    if (!context.Request.Headers.Authorization.Any())
    {
        string t = context.Request.Cookies[Secrets.userCookie];
        if (!string.IsNullOrEmpty(t))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + t);
        }
    }
    //if (CurrencyService.Currency == null)
    //    _ = context.RequestServices.GetService<IActionNoFile<Currency, AdminCurrency>>();
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Request.Path = "/404";
        await next();
    }
});
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        // comment when upload
        if (ctx.Context.Request.Host.Value.Contains("localhost"))
            ctx.Context.Response.Headers.Add("cache-control", "no-cache");
        else
            ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=86400");
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