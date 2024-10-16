using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Runpay.UserIdentification.DataAccess.IdentifyDoc;

public partial class IdentifyDocContext : DbContext
{
    public readonly IConfiguration _configuration;

    public IdentifyDocContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IdentifyDocContext(DbContextOptions<IdentifyDocContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("IdentifyDocConnectionString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Documents");

            entity.HasIndex(e => e.Id, "IX_ID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AddressResidence).HasMaxLength(255);
            entity.Property(e => e.AfilliatedParties).HasMaxLength(500);
            entity.Property(e => e.AssociatedMembers).HasMaxLength(255);
            entity.Property(e => e.Beneficiar).HasMaxLength(255);
            entity.Property(e => e.BirthPlace).HasMaxLength(255);
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.FamilyMembers).HasMaxLength(255);
            entity.Property(e => e.Inn)
                .HasMaxLength(255)
                .HasColumnName("INN");
            entity.Property(e => e.IsPul).HasColumnName("IsPUL");
            entity.Property(e => e.IssuedBy).HasMaxLength(255);
            entity.Property(e => e.OperationTypeOther).HasMaxLength(255);
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Snils)
                .HasMaxLength(255)
                .HasColumnName("SNILS");
            entity.Property(e => e.WorkPlace).HasMaxLength(255);
            entity.Property(e => e.WorkPosition).HasMaxLength(255);
            entity.Property(e => e.WorkTypeOther).HasMaxLength(255);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Document)
                .HasForeignKey<Document>(d => d.Id)
                .HasConstraintName("FK_dbo.Documents_dbo.Users_ID");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PK_Requests_1");

            entity.HasIndex(e => e.Guid, "UNIQ_Request_Guid").IsUnique();

            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CodeData)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.DateAdd).HasColumnType("datetime");
            entity.Property(e => e.DateEdit).HasColumnType("datetime");
            entity.Property(e => e.DateRegistration).HasColumnType("datetime");
            entity.Property(e => e.DocType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FromIp)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("FromIP");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Phone)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PosCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SiaDocumentNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SiadocumentId).HasColumnName("SIADocumentId");
            entity.Property(e => e.SyncDateWithEmoney).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Requests__UserId__29221CFB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC2760357B7C");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Bills).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Idnp)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IDNP");
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(4)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
