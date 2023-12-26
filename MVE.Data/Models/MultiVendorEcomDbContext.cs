using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MVE.Admin.Models;


namespace MVE.Data.Models;

public partial class MultiVendorEcomDbContext : DbContext
{
    public MultiVendorEcomDbContext()
    {
    }

    public MultiVendorEcomDbContext(DbContextOptions<MultiVendorEcomDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccommodationType> AccommodationTypes { get; set; }

    public virtual DbSet<ActivityIncExcMaster> ActivityIncExcMasters { get; set; }

    public virtual DbSet<AdminUser> AdminUsers { get; set; }

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Categories> Categories { get; set; }
    public virtual DbSet<CountryMaster> CountryMasters { get; set; }
    public virtual DbSet<GeneralSiteSetting> GeneralSiteSettings { get; set; }
    public virtual DbSet<MenuItem> MenuItems { get; set; }
    public virtual DbSet<NotificationType> NotificationTypes { get; set; }
    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Region> Regions { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<RolePage> RolePages { get; set; }

    public virtual DbSet<RolePagePermission> RolePagePermissions { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<StaticContentBanner> StaticContentBanners { get; set; }

    public virtual DbSet<StaticPage> StaticPages { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransfersType> TransfersTypes { get; set; }

  

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VisaGuide> VisaGuides { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//    => optionsBuilder.UseSqlServer("Server=14.194.98.244,1436;Database=TravelCustomPackages;User=TravelCustomPackages;Password=fgfghfhfhfhffhh;Trusted_Connection=False;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccommodationType>(entity =>
        {
            entity.ToTable("AccommodationType");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ActivityIncExcMaster>(entity =>
        {
            entity.ToTable("ActivityIncExcMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(60);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdminUse__3214EC07FC5BB7A9");

            entity.ToTable("AdminUser");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EncryptedPassword).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.ForgotPasswordLink).HasMaxLength(256);
            entity.Property(e => e.ForgotPasswordLinkExpired).HasColumnType("datetime");
            entity.Property(e => e.ImageName).HasMaxLength(1000);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobilePhone).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SaltKey).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__AdminUser__Creat__4222D4EF");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InverseModifiedByNavigation)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK__AdminUser__Modif__4316F928");

            entity.HasOne(d => d.Role).WithMany(p => p.AdminUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdminUser__RoleI__440B1D61");
        });

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.ToTable("Billing");

            entity.Property(e => e.Address1).HasMaxLength(200);
            entity.Property(e => e.Address2).HasMaxLength(200);
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Town).HasMaxLength(100);
            entity.Property(e => e.Zipcode).HasMaxLength(50);

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.Billings)
                .HasForeignKey(d => d.Country)
                .HasConstraintName("FK__Billing__Country__12BEA5E7");

            entity.HasOne(d => d.State).WithMany(p => p.Billings)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK__Billing__StateId__11CA81AE");

            entity.HasOne(d => d.User).WithMany(p => p.Billings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__UserId__10D65D75");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.AppliedDiscountPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AppliedFixedDiscunt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApplyTaxPercent1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApplyTaxPercent2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApplyTaxPercentHeading1).HasMaxLength(50);
            entity.Property(e => e.ApplyTaxPercentHeading2).HasMaxLength(50);
            entity.Property(e => e.BookingFor).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.InfantsCount).HasDefaultValueSql("((0))");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobilePhone).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PassportNo).HasMaxLength(30);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ZipCode).HasMaxLength(10);

           

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<Categories>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(60);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });



        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CountryM__3214EC07DFD1A351");

            entity.ToTable("CountryMaster");

            entity.Property(e => e.Code)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.ShortUrl).HasMaxLength(100);
        });

        modelBuilder.Entity<GeneralSiteSetting>(entity =>
        {
            entity.ToTable("GeneralSiteSetting");

            entity.Property(e => e.ApplyTaxPercent1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApplyTaxPercent2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApplyTaxPercentHeading1).HasMaxLength(50);
            entity.Property(e => e.ApplyTaxPercentHeading2).HasMaxLength(50);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountFix).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.IsApplyDiscountFix).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsApplyDiscountPercent).HasDefaultValueSql("((0))");
            entity.Property(e => e.KeyName).HasMaxLength(50);
            entity.Property(e => e.LogoImageExtension).HasMaxLength(20);
            entity.Property(e => e.LogoImageExtensionDark).HasMaxLength(20);
            entity.Property(e => e.LogoImageName).HasMaxLength(255);
            entity.Property(e => e.LogoImageNameDark).HasMaxLength(255);
            entity.Property(e => e.LogoOriginalImageName).HasMaxLength(255);
            entity.Property(e => e.LogoOriginalImageNameDark).HasMaxLength(255);
            entity.Property(e => e.ModifiedBy).HasDefaultValueSql("((0))");
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SiteName).HasMaxLength(80);
            entity.Property(e => e.SupportEmail).HasMaxLength(200);
            entity.Property(e => e.SupportMobile).HasMaxLength(20);
        });

           modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MenuItem__3214EC075EB9E2B3");

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SubMenuIcon).HasMaxLength(100);
            entity.Property(e => e.SubMenuName).HasMaxLength(50);
        });


        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.ToTable("NotificationType");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(50);
        });


        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ratings__3214EC0753EC5D2A");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.RatingOn).HasColumnType("datetime");
            entity.Property(e => e.UserType).HasDefaultValueSql("((0))");

           

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__UserId__2A962F78");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Region__3214EC07106352D1");

            entity.ToTable("Region");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(60);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });


        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE8040F66B");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ReviewOn).HasColumnType("datetime");

            

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__2E66C05C");
        });

        modelBuilder.Entity<RolePage>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PK__RolePage__3214EC076972A431");

            entity.ToTable("RolePage");

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("Is_Active");
            entity.Property(e => e.PageName).HasMaxLength(100);
            entity.Property(e => e.PageUrl).HasMaxLength(250);
        });

        modelBuilder.Entity<RolePagePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolePage__3214EC07BD206F86");

            entity.ToTable("RolePagePermission");

            entity.HasOne(d => d.Page).WithMany(p => p.RolePagePermissions)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePagePermission_RolePage");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePagePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePageP__RoleI__3D89085B");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SessionExpiry).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_Users");
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_State");

            entity.ToTable("StateMaster");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.StateMaster)
                .HasForeignKey<StateMaster>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StateMaster_CountryMaster");
        });

        modelBuilder.Entity<StaticContentBanner>(entity =>
        {
            entity.ToTable("StaticContentBanner");

            entity.Property(e => e.ImageExtension).HasMaxLength(20);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.OriginalImageName).HasMaxLength(255);
        });

        modelBuilder.Entity<StaticPage>(entity =>
        {
            entity.ToTable("StaticPage");

            entity.Property(e => e.AddedDate).HasColumnType("datetime");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(20)
                .HasColumnName("IPAddress");
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaKeyword).HasMaxLength(500);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PageTitle).HasMaxLength(100);
            entity.Property(e => e.Url).HasMaxLength(300);
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Theme__3214EC07612C29A1");

            entity.ToTable("Theme");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(60);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ShortName).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentAmt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDateTime).HasColumnType("datetime");
            entity.Property(e => e.PaymentType).HasMaxLength(20);

            entity.HasOne(d => d.Booking).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Bookings");
        });

        modelBuilder.Entity<TransfersType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transfer__3214EC075EF7C6A3");

            entity.ToTable("TransfersType");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CreationOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EmailOtp)
                .HasMaxLength(150)
                .HasColumnName("EmailOTP");
            entity.Property(e => e.EmailVerificationOtpExpired).HasColumnType("datetime");
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(256);
            entity.Property(e => e.EncryptedPassword).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.ForgotPasswordLink).HasMaxLength(256);
            entity.Property(e => e.ForgotPasswordLinkExpired).HasColumnType("datetime");
            entity.Property(e => e.ForgotPasswordLinkUsed).HasDefaultValueSql("((0))");
            entity.Property(e => e.ImageName).HasMaxLength(1000);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobilePhone).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SaltKey).HasMaxLength(200);
            entity.Property(e => e.StateOrCounty).HasMaxLength(50);
            entity.Property(e => e.ZipCode).HasMaxLength(10);

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK__Users__CountryId__4AD81681");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Notifications");

            entity.Property(e => e.ImageExtension).HasMaxLength(20);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.IsImageAdded).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsIncludeInNotification).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsVisited).HasDefaultValueSql("((0))");
            entity.Property(e => e.OriginalImageName).HasMaxLength(255);
            entity.Property(e => e.Subject).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("UserPermission");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07AC5AEF26");

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("Is_Active");
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VisaGuide>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_DynamicContent");

            entity.ToTable("VisaGuide");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaKeyword).HasMaxLength(500);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PageTitle).HasMaxLength(100);
            entity.Property(e => e.Url).HasMaxLength(300);

            entity.HasOne(d => d.Country).WithMany(p => p.VisaGuides)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisaGuide_CountryMaster");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
