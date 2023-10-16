
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Ottieni la stringa di connessione al database dalla configurazione
var conString = builder.Configuration.GetConnectionString("BookStoreAppDbConnection");

// Registra il contesto del database BookStoreDbContext come servizio nell'applicazione
// Configura il contesto del database per utilizzare il provider SQL Server con la stringa di connessione ottenuta
builder.Services.AddDbContext<BookStoreDbContext>(options => options.UseSqlServer(conString));

// Configura l'infrastruttura di autenticazione e autorizzazione con Identity
builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()    // Aggiungi il supporto per i ruoli di Identity
    .AddEntityFrameworkStores<BookStoreDbContext>();// Configura l'utilizzo di Entity Framework per la memorizzazione dei dati di Identity


// Registra il servizio AutoMapper nell'applicazione
// AutoMapper semplifica la mappatura tra oggetti di dominio e oggetti di trasferimento dati (DTO)
// `typeof(MapperConfig)` specifica il tipo che contiene le configurazioni di mappatura
builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura l'utilizzo di Serilog per il logging all'interno dell'host
builder.Host.UseSerilog((contextConfigutation, loggingConfiguration) => 
    loggingConfiguration.WriteTo.Console()  // Scrivi i log sulla console
    .ReadFrom.Configuration(contextConfigutation.Configuration) // Leggi le configurazioni di logging dalla configurazione dell'host
);

// Aggiungi la gestione delle politiche di CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAll", // Definisce una politica chiamata "AllowAll"
        b =>  b.AllowAnyMethod()  // Consente qualsiasi metodo HTTP
        .AllowAnyHeader()         // Consente qualsiasi intestazione HTTP
        .AllowAnyOrigin());       // Consente qualsiasi origine (qualsiasi dominio)
});

// Configura l'autenticazione basata su token JWT (JSON Web Token) 
// Configura il servizio di autenticazione
builder.Services.AddAuthentication(options =>
{
    // Imposta il schema di autenticazione predefinito sia per l'autenticazione che per le sfide come JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(_options =>
{
    // Configura i parametri di convalida del token JWT
    _options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Abilita la convalida della chiave del mittente (Issuer)
        ValidateIssuer = true, // Abilita la convalida dell'emittente (Issuer)
        ValidateAudience = true, // Abilita la convalida del pubblico (Audience)
        ValidateLifetime = true, // Abilita la convalida della durata di validità del token
        ClockSkew = TimeSpan.Zero, // Imposta la tolleranza per l'orologio a zero
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // Specifica il nome dell'emittente (Issuer) valido dal file di configurazione
        ValidAudience = builder.Configuration["JwtSettings:Audience"], // Specifica il pubblico (Audience) valido dal file di configurazione
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))  // Specifica la chiave di firma del token da utilizzare per la convalida "appsettings.json"
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
