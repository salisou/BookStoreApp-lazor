namespace bookstoreApp.Api.Data;

public partial class BookStoreDbContext : IdentityDbContext<ApiUser>
{
    public BookStoreDbContext()
    {
    }

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; } 

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\sqlexpress;Database=BookStoreDb;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC07E0AB59FA");

            entity.Property(e => e.Bio).HasMaxLength(250);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC07F43ED3AA");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA75AA5BA3").IsUnique();

            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.Isbn)
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Summary).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Books_ToTable");
        });


        // Configura l'inizializzazione dei dati per l'entità IdentityRole
        modelBuilder.Entity<IdentityRole>().HasData
            (
                // Definisci i dati per il ruolo "User"
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER", // Nome normalizzato del ruolo
                    Id = "cfdf272a-5bec-405f-a6eb-2ee80dc0ae4b" // Identificatore univoco del ruolo
                },


                // Definisci i dati per il ruolo "Administrator"
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR", // Nome normalizzato del ruolo
                    Id = "4043d151-7160-4208-be8d-260607a5f812" // Identificatore univoco del ruolo
                }

            );


        // Oggetto PasswordHasher per l'entità ApiUser
        var hasher = new PasswordHasher<ApiUser>();

        // Configura l'inizializzazione dei dati per l'entità ApiUser
        modelBuilder.Entity<ApiUser>().HasData
            (
                // Definisci i dati per l'utente "admin"
                new ApiUser
                {
                    Id = "072b9fc5-1b92-4441-b3a6-27cadc442cb9",  // Identificatore univoco dell'utente
                    Email = "admin@bookstre.com",
                    NormalizedEmail = "ADMIN@BOOKSTORE.COM",
                    UserName = "admin@bookstre.com",
                    NormalizedUserName = "ADMIN@BOOKSTORE.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    PasswordHash = hasher.HashPassword(null, "P@ssword1")  // Hash della password
                },


                // Definisci i dati per l'utente "user"
                new ApiUser
                {
                    Id = "5e75c0d7-5127-406e-aa70-b28f0cd57d83",
                    Email = "user@bookstre.com",
                    NormalizedEmail = "USER@BOOKSTORE.COM",
                    UserName = "user@bookstre.com",
                    NormalizedUserName = "USER@BOOKSTORE.COM",
                    FirstName = "System",
                    LastName = "User",
                    PasswordHash = hasher.HashPassword(null, "P@ssword1")
                }

            );


        // Configura l'inizializzazione dei dati per l'entità IdentityUserRole<string>
        modelBuilder.Entity<IdentityUserRole<string>>().HasData
        (
            // Associa il ruolo "User" all'utente "user"
            new IdentityUserRole<string>
            {
                RoleId = "cfdf272a-5bec-405f-a6eb-2ee80dc0ae4b", // Identificatore del ruolo "User"
                UserId = "5e75c0d7-5127-406e-aa70-b28f0cd57d83" // Identificatore dell'utente "user"
            },

            // Associa il ruolo "Administrator" all'utente "admin"
            new IdentityUserRole<string>
            {
                RoleId = "4043d151-7160-4208-be8d-260607a5f812", // Identificatore del ruolo "Administrator"
                UserId = "072b9fc5-1b92-4441-b3a6-27cadc442cb9" // Identificatore dell'utente "admin"
            }
        );


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
