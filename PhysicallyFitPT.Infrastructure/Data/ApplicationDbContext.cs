// <copyright file="ApplicationDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  public DbSet<Patient> Patients => this.Set<Patient>();

  public DbSet<Appointment> Appointments => this.Set<Appointment>();

  public DbSet<Note> Notes => this.Set<Note>();

  public DbSet<CptCode> CptCodes => this.Set<CptCode>();

  public DbSet<Icd10Code> Icd10Codes => this.Set<Icd10Code>();

  public DbSet<QuestionnaireDefinition> QuestionnaireDefinitions => this.Set<QuestionnaireDefinition>();

  public DbSet<QuestionnaireResponse> QuestionnaireResponses => this.Set<QuestionnaireResponse>();

  public DbSet<CheckInMessageLog> CheckInMessageLogs => this.Set<CheckInMessageLog>();

  /// <inheritdoc/>
  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    var now = DateTimeOffset.UtcNow;
    foreach (var e in this.ChangeTracker.Entries<Entity>())
    {
      if (e.State == EntityState.Added)
      {
        e.Entity.CreatedAt = now;
      }

      if (e.State == EntityState.Modified)
      {
        e.Entity.UpdatedAt = now;
      }
    }

    return base.SaveChangesAsync(cancellationToken);
  }

  /// <inheritdoc/>
  protected override void OnModelCreating(ModelBuilder b)
  {
    base.OnModelCreating(b);

    // Global soft-delete filter
    b.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);

    // Indexes
    b.Entity<Patient>().HasIndex(p => new { p.LastName, p.FirstName });
    b.Entity<Appointment>().HasIndex(a => new { a.PatientId, a.ScheduledStart });
    b.Entity<QuestionnaireResponse>().HasIndex(q => q.AppointmentId);

    // Patient constraints
    b.Entity<Patient>(e =>
    {
      e.Property(p => p.FirstName).HasMaxLength(60).IsRequired();
      e.Property(p => p.LastName).HasMaxLength(60).IsRequired();
      e.Property(p => p.Email).HasMaxLength(120);
      e.Property(p => p.MobilePhone).HasMaxLength(30);
      e.Property(p => p.MRN).HasMaxLength(40);
    });

    // Codes
    b.Entity<CptCode>(e =>
    {
      e.Property(x => x.Code).HasMaxLength(10).IsRequired();
      e.Property(x => x.Description).HasMaxLength(200).IsRequired();
      e.HasIndex(x => x.Code).IsUnique();
    });
    b.Entity<Icd10Code>(e =>
    {
      e.Property(x => x.Code).HasMaxLength(10).IsRequired();
      e.Property(x => x.Description).HasMaxLength(200).IsRequired();
      e.HasIndex(x => x.Code);
    });

    // Note aggregates
    b.Entity<Note>().OwnsOne(n => n.Subjective);

    b.Entity<Note>().OwnsOne(n => n.Objective, o =>
    {
      o.OwnsMany(x => x.Rom, r =>
      {
        r.ToTable("NoteObjectiveRom");
        r.WithOwner().HasForeignKey("NoteId");
        r.HasKey(x => x.Id);
        r.HasIndex("NoteId");
      });
      o.OwnsMany(x => x.Mmt, m =>
      {
        m.ToTable("NoteObjectiveMmt");
        m.WithOwner().HasForeignKey("NoteId");
        m.HasKey(x => x.Id);
        m.HasIndex("NoteId");
      });
      o.OwnsMany(x => x.SpecialTests, s =>
      {
        s.ToTable("NoteObjectiveSpecialTests");
        s.WithOwner().HasForeignKey("NoteId");
        s.HasKey(x => x.Id);
        s.HasIndex("NoteId");
      });
      o.OwnsMany(x => x.OutcomeMeasures, om =>
      {
        om.ToTable("NoteObjectiveOutcomeMeasures");
        om.WithOwner().HasForeignKey("NoteId");
        om.HasKey(x => x.Id);
        om.HasIndex("NoteId");
      });
      o.OwnsMany(x => x.ProvidedInterventions, pi =>
      {
        pi.ToTable("NoteProvidedInterventions");
        pi.WithOwner().HasForeignKey("NoteId");
        pi.HasKey(x => x.Id);
        pi.HasIndex("NoteId");
      });
    });

    b.Entity<Note>().OwnsOne(n => n.Assessment, a =>
    {
      a.OwnsMany(x => x.Icd10Codes, i =>
      {
        i.ToTable("NoteAssessmentIcd10");
        i.WithOwner().HasForeignKey("NoteId");
        i.HasKey(x => x.Id);
      });
      a.OwnsMany(x => x.Goals, g =>
      {
        g.ToTable("NoteAssessmentGoals");
        g.WithOwner().HasForeignKey("NoteId");
        g.HasKey(x => x.Id);
      });
    });

    b.Entity<Note>().OwnsOne(n => n.Plan, p =>
    {
      p.OwnsMany(x => x.Hep, h =>
      {
        h.ToTable("NotePlanHep");
        h.WithOwner().HasForeignKey("NoteId");
        h.HasKey(x => x.Id);
      });
    });

    b.Entity<Appointment>()
      .HasOne(a => a.Note)
      .WithOne(n => n.Appointment)
      .HasForeignKey<Note>(n => n.AppointmentId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);
  }
}
