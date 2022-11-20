using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using newTolkuchka;
using newTolkuchka.Models;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using System.Globalization;
using System.Text;

string con = Secrets.dbConnection;
int accessLevels = 3;
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
     .WithOrigins("http://localhost:3000", "https://localhost:3000", "http://cms.tolkuchka.bar", "https://cms.tolkuchka.bar")
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
builder.Services.AddScoped<ICategory, CategoryService>();
builder.Services.AddScoped<IBrand, BrandService>();
builder.Services.AddScoped<ICrypto, CryptoService>();
builder.Services.AddScoped<IEmployee, EmployeeService>();
builder.Services.AddScoped<IEntry, EntryService>();
builder.Services.AddScoped<IImage, ImageService>();
builder.Services.AddScoped<IJwt, JwtService>();
builder.Services.AddScoped<ILine, LineService>();
builder.Services.AddScoped<ILogin, LoginService>();
builder.Services.AddScoped<IModel, ModelService>();
builder.Services.AddScoped<IPath, PathService>();
builder.Services.AddScoped<IPosition, PositionService>();
builder.Services.AddScoped<IProduct, ProductService>();
builder.Services.AddScoped<IType, TypeService>();
builder.Services.AddScoped<ISpec, SpecService>();
builder.Services.AddScoped<ISpecsValue, SpecsValueService>();
builder.Services.AddScoped<IWarranty, WarrantyService>();
builder.Services.AddScoped<ISlide, SlideService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IMail, MailService>();
builder.Services.AddScoped<IBreadcrumbs, BreadcrumbsService>();
builder.Services.AddScoped<IActionNoFile<Currency>, CurrencyService>();
builder.Services.AddScoped<ISpecsValueMod, SpecsValueModService>();
builder.Services.AddScoped<IOrder, OrderService>();
builder.Services.AddScoped<IInvoice, InvoiceService>();
builder.Services.AddScoped<IContent, ContentService>();
builder.Services.AddScoped<ISupplier, SupplierService>();
builder.Services.AddScoped<IPurchaseInvoice, PurchaseInvoiceService>();
builder.Services.AddScoped<IPurchase, PurchaseService>();
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
app.UseRequestLocalization();
app.UseCookiePolicy();
app.Use(async (context, next) =>
{
    if (context.Request.Cookies.ContainsKey(Secrets.adminCookie))
    {
        string t = context.Request.Cookies[Secrets.adminCookie];
        context.Request.Headers.Add("Authorization", "Bearer " + t);
    }
    else if (context.Request.Cookies.ContainsKey(Secrets.userCookie))
    {
        string t = context.Request.Cookies[Secrets.userCookie];
        context.Request.Headers.Add("Authorization", "Bearer " + t);
    }
    _ = context.RequestServices.GetService<IActionNoFile<Currency>>();
    await next();
});
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        // remove when upload
        if (ctx.Context.Request.Host.Value.Contains("localhost"))
            ctx.Context.Response.Headers.Add("Cache-Control", "no-cache");
        else
            ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=8640000");
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