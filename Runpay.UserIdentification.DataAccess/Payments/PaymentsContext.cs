using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Runpay.UserIdentification.DataAccess.Payments;

public partial class PaymentsContext : DbContext
{
    public readonly IConfiguration _configuration;

    public PaymentsContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public PaymentsContext(DbContextOptions<PaymentsContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Certificate> Certificates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("PaymentsConnectionString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AI");

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.ToTable("CERTIFICATES");

            entity.HasIndex(e => e.SerialNumber, "SERIAL_NUMBER")
                .IsUnique()
                .HasFillFactor(80);

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateAdd)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Дата добавления записи")
                .HasColumnType("datetime")
                .HasColumnName("DATE_ADD");
            entity.Property(e => e.DateExpire)
                .HasComment("Дата окончания срока действия сертификата")
                .HasColumnType("datetime")
                .HasColumnName("DATE_EXPIRE");
            entity.Property(e => e.DateGiven)
                .HasComment("Дата выдачи сертификата")
                .HasColumnType("datetime")
                .HasColumnName("DATE_GIVEN");
            entity.Property(e => e.InternetPayments).HasComment("0 -платежи только через payment.asp, 1 платежи и через web и через payment.asp");
            entity.Property(e => e.Issuer)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasComment("Поле Issuer CN (кем выдан)")
                .HasColumnName("ISSUER");
            entity.Property(e => e.NewCertificateId)
                .HasComment("Это ссылка на эту же таблицу. Предназначена для того, чтобы когда сертификат закончится, в БД можно было бы указать сертификат, который должен его заменить. Новый сертификат надо заранее подготовить и экспортировать в файл PFX в нужную директорию. Файл должен именоваться SERIAL.pfx")
                .HasColumnName("NEW_CERTIFICATE_ID");
            entity.Property(e => e.PublicKey)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("PUBLIC_KEY");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Серийный номер сертификата")
                .HasColumnName("SERIAL_NUMBER");
            entity.Property(e => e.State)
                .HasDefaultValue(true)
                .HasComment("Состояние 1 активный, 0 неактивный")
                .HasColumnName("STATE");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasComment("поле Subject CN  (кому выдан сертификат)")
                .HasColumnName("SUBJECT");
            entity.Property(e => e.UserId)
                .HasDefaultValue(-1)
                .HasColumnName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
