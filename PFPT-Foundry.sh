#!/usr/bin/env bash
set -euo pipefail

# ---- safety & env ----------------------------------------------------------------
# refuse to run as root (prevents sudo-caused permission mess)
if [ "$EUID" -eq 0 ]; then
  echo "✋ Do not run this script with sudo. Fix permissions instead."; exit 1
fi

# Use user-local caches if Homebrew SDK is detected (or if unset)
if dotnet --info 2>/dev/null | grep -qi 'homebrew'; then
  echo "⚠️  Homebrew-managed .NET detected; using user-local caches."
  export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-$HOME/.dotnet}"
  export NUGET_PACKAGES="${NUGET_PACKAGES:-$HOME/.nuget/packages}"
fi
# ------------------------------------------------------------------------------


# ---------------------------------------------
# Physically Fit PT – Blazor (MAUI) v4 scaffold (macOS)
# Clean architecture, services, CI, tests, seed, shared clinical libs
# Flags:
#   --create-migration   Generate Initial migration and update DB
#   --seed               Seed a local dev DB (uses PFP_DB_PATH or ./dev.physicallyfitpt.db)
# ---------------------------------------------

CREATE_MIGRATION=false
SEED_DATA=false
while [[ $# -gt 0 ]]; do
  case "$1" in
    --create-migration) CREATE_MIGRATION=true; shift ;;
    --seed)             SEED_DATA=true; shift ;;
    *) echo "Unknown arg: $1"; exit 1 ;;
  esac
done

SOLUTION="PhysicallyFitPT"
APP="$SOLUTION"                   # .NET MAUI Blazor app
DOMAIN="$SOLUTION.Domain"         # Pure POCOs (no EF attributes)
INFRA="$SOLUTION.Infrastructure"  # EF Core, DbContext, Services, PDF
SHARED="$SOLUTION.Shared"         # DTOs / shared clinical libs
TESTS="$SOLUTION.Tests"           # xUnit tests
SEEDER="$SOLUTION.Seeder"         # Console app to seed EF data

echo "🚀 Scaffolding $SOLUTION (MAUI Blazor + Clean Architecture)…"

# 0) Ensure MAUI workloads (safe/idempotent)
if ! dotnet workload list | grep -Eiq '(^|[[:space:]])maui([[:space:]]|$)'; then
  echo "• Installing .NET MAUI workloads…"
  dotnet workload install maui
else
  echo "• MAUI workloads already installed."
fi

# 1) Base files (SDK pin, ignores, analyzers, styles)
[[ -f global.json ]] || cat > global.json <<'EOF'
{ "sdk": { "version": "9.0.100", "rollForward": "latestFeature" } }
EOF

[[ -f .gitignore ]] || dotnet new gitignore >/dev/null

[[ -f .editorconfig ]] || cat > .editorconfig <<'EOF'
root = true

[*.{cs,razor}]
indent_style = space
indent_size = 2
charset = utf-8-bom
insert_final_newline = true
dotnet_style_qualification_for_field = false:suggestion
dotnet_style_qualification_for_property = false:suggestion
dotnet_style_qualification_for_method = false:suggestion
dotnet_style_predefined_type_for_locals_parameters_members = true:warning
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = true:suggestion

[*.sh]
charset = utf-8
end_of_line = lf
insert_final_newline = true
EOF

[[ -f Directory.Build.props ]] || cat > Directory.Build.props <<'EOF'
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
    <AnalysisLevel>latest</AnalysisLevel>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.507" PrivateAssets="all" />
    <PackageReference Include="Roslynator.Analyzers" Version="4.10.0" PrivateAssets="all" />
  </ItemGroup>
  <PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
</ItemGroup>
</Project>
EOF

# 2) Solution + projects
[[ -f "$SOLUTION.sln" ]] || dotnet new sln -n "$SOLUTION"
[[ -d "$APP"    ]] || dotnet new maui-blazor -n "$APP"
[[ -d "$DOMAIN" ]] || dotnet new classlib   -n "$DOMAIN"
[[ -d "$INFRA"  ]] || dotnet new classlib   -n "$INFRA"
[[ -d "$SHARED" ]] || dotnet new classlib   -n "$SHARED"
[[ -d "$TESTS"  ]] || dotnet new xunit      -n "$TESTS"
[[ -d "$SEEDER" ]] || dotnet new console    -n "$SEEDER"

# --- TFM Normalizer: enforce supported frameworks (macOS-safe) ----------------
echo "• Normalizing TargetFramework/TargetFrameworks to .NET 9…"

# helper: replace (or insert) a single-TFM <TargetFramework> in a csproj
ensure_single_tfm() {
  local file="$1"
  local tfm="$2"
  if grep -q "<TargetFrameworks>" "$file"; then
    # convert multi -> single
    sed -i '' -E "s|<TargetFrameworks>[^<]+</TargetFrameworks>|<TargetFramework>${tfm}</TargetFramework>|" "$file"
  elif grep -q "<TargetFramework>" "$file"; then
    sed -i '' -E "s|<TargetFramework>[^<]+</TargetFramework>|<TargetFramework>${tfm}</TargetFramework>|" "$file"
  else
    # insert under first <PropertyGroup>
    awk -v TFM="$tfm" '
      BEGIN{inserted=0}
      {print}
      /<PropertyGroup>/ && inserted==0 {print "    <TargetFramework>" TFM "</TargetFramework>"; inserted=1}
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
  fi
}

# helper: force MAUI multi-TFM
ensure_maui_tfms() {
  local file="$1"
  local tfms="net9.0-android;net9.0-ios;net9.0-maccatalyst"
  if grep -q "<TargetFrameworks>" "$file"; then
    sed -i '' -E "s|<TargetFrameworks>[^<]+</TargetFrameworks>|<TargetFrameworks>${tfms}</TargetFrameworks>|" "$file"
  elif grep -q "<TargetFramework>" "$file"; then
    sed -i '' -E "s|<TargetFramework>[^<]+</TargetFramework>|<TargetFrameworks>${tfms}</TargetFrameworks>|" "$file"
  else
    awk -v TFMS="$tfms" '
      BEGIN{inserted=0}
      {print}
      /<PropertyGroup>/ && inserted==0 {print "    <TargetFrameworks>" TFMS "</TargetFrameworks>"; inserted=1}
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
  fi
}

# 1) Normalize MAUI app to multi-target net9
ensure_maui_tfms "$APP/$APP.csproj"

# 2) Normalize libs/tests/tools to net9.0 single TFM
for proj in \
  "$DOMAIN/$DOMAIN.csproj" \
  "$INFRA/$INFRA.csproj" \
  "$SHARED/$SHARED.csproj" \
  "$TESTS/$TESTS.csproj" \
  "$SEEDER/$SEEDER.csproj"
do
  ensure_single_tfm "$proj" "net9.0"
done

# 3) Sanity report (shows any leftover 7/8)
FOUND_OLD=$(grep -R --line-number -E "<TargetFramework(s)?>.*net[78]\.0" . || true)
if [[ -n "$FOUND_OLD" ]]; then
  echo "⚠️  WARNING: Found residual net7.0/net8.0 references:"
  echo "$FOUND_OLD"
  echo "   Please review the files above; they may have custom configurations."
else
  echo "• All project TFMs set to .NET 9 successfully."
fi
# ------------------------------------------------------------------------------


# Add to solution (idempotent-ish)
dotnet sln add "$APP/$APP.csproj"       2>/dev/null || true
dotnet sln add "$DOMAIN/$DOMAIN.csproj" 2>/dev/null || true
dotnet sln add "$INFRA/$INFRA.csproj"   2>/dev/null || true
dotnet sln add "$SHARED/$SHARED.csproj" 2>/dev/null || true
dotnet sln add "$TESTS/$TESTS.csproj"   2>/dev/null || true
dotnet sln add "$SEEDER/$SEEDER.csproj" 2>/dev/null || true

# 3) Project references (clean layering)
dotnet add "$APP/$APP.csproj"        reference "$DOMAIN/$DOMAIN.csproj" "$INFRA/$INFRA.csproj" "$SHARED/$SHARED.csproj" 2>/dev/null || true
dotnet add "$INFRA/$INFRA.csproj"    reference "$DOMAIN/$DOMAIN.csproj" "$SHARED/$SHARED.csproj"                         2>/dev/null || true
dotnet add "$TESTS/$TESTS.csproj"    reference "$INFRA/$INFRA.csproj" "$DOMAIN/$DOMAIN.csproj"                            2>/dev/null || true
dotnet add "$SEEDER/$SEEDER.csproj"  reference "$INFRA/$INFRA.csproj" "$DOMAIN/$DOMAIN.csproj"                            2>/dev/null || true

# 4) NuGet packages
# Infrastructure: EF Core + Sqlite + Design + PDF + Skia (render)
dotnet add "$INFRA/$INFRA.csproj" \
 package Microsoft.EntityFrameworkCore
dotnet add "$INFRA/$INFRA.csproj" \
 package Microsoft.EntityFrameworkCore.Sqlite
dotnet add "$INFRA/$INFRA.csproj" \
 package Microsoft.EntityFrameworkCore.Design
dotnet add "$INFRA/$INFRA.csproj" \
 package QuestPDF
dotnet add "$INFRA/$INFRA.csproj" \
 package SkiaSharp
dotnet add "$INFRA/$INFRA.csproj" \
 package SQLitePCLRaw.bundle_e_sqlite3

# App: MAUI + Http + SQLite native assets for mobile
dotnet add "$APP/$APP.csproj" \
 package SQLitePCLRaw.bundle_e_sqlite3
dotnet add "$APP/$APP.csproj" \
 package SkiaSharp.Views.Maui.Controls

# Tests: assertions + EF Core bits
dotnet add "$TESTS/$TESTS.csproj" \
 package FluentAssertions
dotnet add "$TESTS/$TESTS.csproj" \
 package Microsoft.EntityFrameworkCore
dotnet add "$TESTS/$TESTS.csproj" \
 package Microsoft.EntityFrameworkCore.Sqlite


# ---- EF local tool & design-time factory ---------------------------------------
# install/update local dotnet-ef tool (checked into repo)
if [ ! -f .config/dotnet-tools.json ]; then
  dotnet new tool-manifest --force
fi
dotnet tool update dotnet-ef || dotnet tool install dotnet-ef

# add a design-time factory so migrations don't need the MAUI app to run
DTF="$INFRA/Data/DesignTimeDbContextFactory.cs"
if [ ! -f "$DTF" ]; then
  mkdir -p "$INFRA/Data"
  cat > "$DTF" <<'CS'
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PhysicallyFitPT.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source={ResolveDbPath()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    static string ResolveDbPath()
    {
        var root = FindRepoRoot() ?? Directory.GetCurrentDirectory();
        return Path.Combine(root, "pfpt.design.sqlite");
    }

    static string? FindRepoRoot()
    {
        var d = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (d != null &&
               !File.Exists(Path.Combine(d.FullName, ".gitignore")) &&
               !File.Exists(Path.Combine(d.FullName, ".editorconfig")))
            d = d.Parent;
        return d?.FullName;
    }
}
CS
fi
# ------------------------------------------------------------------------------

# 5) Domain models (no DataAnnotations)
mkdir -p "$DOMAIN"
cat > "$DOMAIN/Common.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public abstract class Entity
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public string? CreatedBy { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }
  public string? UpdatedBy { get; set; }
  public bool IsDeleted { get; set; }
}

public enum VisitType { Eval = 0, Daily = 1, Progress = 2, Discharge = 3 }
public enum Side { NA = 0, Left = 1, Right = 2, Bilateral = 3 }
public enum SpecialTestResult { NotPerformed = 0, Negative = 1, Positive = 2, Inconclusive = 3 }
public enum DeliveryMethod { SMS = 0, Email = 1 }
public enum QuestionnaireType { Eval = 0, Daily = 1, Progress = 2, Discharge = 3, BodyPartSpecific = 4 }
public enum GoalStatus { Active = 0, Met = 1, PartiallyMet = 2, NotMet = 3, Deferred = 4 }
EOF

cat > "$DOMAIN/Patient.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class Patient : Entity
{
  public string? MRN { get; set; }
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public DateTime? DateOfBirth { get; set; }
  public string? Sex { get; set; }
  public string? Email { get; set; }
  public string? MobilePhone { get; set; }
  public string? MedicationsCsv { get; set; }
  public string? ComorbiditiesCsv { get; set; }
  public string? AssistiveDevicesCsv { get; set; }
  public string? LivingSituation { get; set; }
  public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
EOF

cat > "$DOMAIN/Appointment.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class Appointment : Entity
{
  public Guid PatientId { get; set; }
  public Patient Patient { get; set; } = null!;
  public VisitType VisitType { get; set; }
  public DateTimeOffset ScheduledStart { get; set; }
  public DateTimeOffset? ScheduledEnd { get; set; }
  public string? Location { get; set; }
  public string? ClinicianNpi { get; set; }
  public string? ClinicianName { get; set; }
  public DateTimeOffset? QuestionnaireSentAt { get; set; }
  public DateTimeOffset? QuestionnaireCompletedAt { get; set; }
  public bool IsCheckedIn { get; set; }
  public Note? Note { get; set; }
}
EOF

cat > "$DOMAIN/NoteAndSections.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class Note : Entity
{
  public Guid PatientId { get; set; }
  public Patient Patient { get; set; } = null!;
  public Guid AppointmentId { get; set; }
  public Appointment Appointment { get; set; } = null!;
  public VisitType VisitType { get; set; }

  public SubjectiveSection Subjective { get; set; } = new();
  public ObjectiveSection Objective { get; set; } = new();
  public AssessmentSection Assessment { get; set; } = new();
  public PlanSection Plan { get; set; } = new();

  public bool IsSigned { get; set; }
  public DateTimeOffset? SignedAt { get; set; }
  public string? SignedBy { get; set; }
}

public class SubjectiveSection
{
  public string? ChiefComplaint { get; set; }
  public string? HistoryOfPresentIllness { get; set; }
  public string? PainLocationsCsv { get; set; }
  public string? PainSeverity0to10 { get; set; }
  public string? AggravatingFactors { get; set; }
  public string? EasingFactors { get; set; }
  public string? FunctionalLimitations { get; set; }
  public string? PatientGoalsNarrative { get; set; }
}

public class ObjectiveSection
{
  public List<RomMeasure> Rom { get; set; } = new();
  public List<MmtMeasure> Mmt { get; set; } = new();
  public List<SpecialTest> SpecialTests { get; set; } = new();
  public List<OutcomeMeasureScore> OutcomeMeasures { get; set; } = new();
  public List<ProvidedIntervention> ProvidedInterventions { get; set; } = new();
}

public class RomMeasure
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Joint { get; set; } = "Knee";
  public string Movement { get; set; } = "Flexion";
  public Side Side { get; set; } = Side.NA;
  public int? MeasuredDegrees { get; set; }
  public int? NormalDegrees { get; set; }
  public bool WithPain { get; set; }
  public string? Notes { get; set; }
}

public class MmtMeasure
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string MuscleGroup { get; set; } = "Quadriceps";
  public Side Side { get; set; } = Side.NA;
  public string Grade { get; set; } = "4+";
  public bool WithPain { get; set; }
  public string? Notes { get; set; }
}

public class SpecialTest
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = "Lachman";
  public Side Side { get; set; } = Side.NA;
  public SpecialTestResult Result { get; set; } = SpecialTestResult.NotPerformed;
  public string? Notes { get; set; }
}

public class OutcomeMeasureScore
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Instrument { get; set; } = "LEFS";
  public int? RawScore { get; set; }
  public double? Percent { get; set; }
  public DateTime? CollectedOn { get; set; }
}

public class ProvidedIntervention
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string CptCode { get; set; } = null!;
  public string? Description { get; set; }
  public int Units { get; set; }
  public int? Minutes { get; set; }
}

public class AssessmentSection
{
  public string? ClinicalImpression { get; set; }
  public string? RehabPotential { get; set; }
  public List<Icd10Link> Icd10Codes { get; set; } = new();
  public List<Goal> Goals { get; set; } = new();
}

public class Icd10Link
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Code { get; set; } = null!;
  public string? Description { get; set; }
}

public class PlanSection
{
  public string? Frequency { get; set; }
  public string? Duration { get; set; }
  public string? PlannedInterventionsCsv { get; set; }
  public List<ExercisePrescription> Hep { get; set; } = new();
  public string? NextVisitFocus { get; set; }
}

public class ExercisePrescription
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = null!;
  public string? Dosage { get; set; }
  public string? Notes { get; set; }
}
EOF

cat > "$DOMAIN/Goal.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class Goal
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public bool IsLongTerm { get; set; }
  public string Description { get; set; } = null!;
  public string? MeasureType { get; set; }
  public string? BaselineValue { get; set; }
  public string? TargetValue { get; set; }
  public DateTime? TargetDate { get; set; }
  public GoalStatus Status { get; set; } = GoalStatus.Active;
}
EOF

cat > "$DOMAIN/Codes.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class CptCode : Entity
{
  public string Code { get; set; } = null!;
  public string Description { get; set; } = null!;
}

public class Icd10Code : Entity
{
  public string Code { get; set; } = null!;
  public string Description { get; set; } = null!;
}
EOF

cat > "$DOMAIN/Questionnaires.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class QuestionnaireDefinition : Entity
{
  public string Name { get; set; } = null!;
  public QuestionnaireType Type { get; set; }
  public string? BodyRegion { get; set; }
  public int Version { get; set; } = 1;
  public string JsonSchema { get; set; } = "{}";
}

public class QuestionnaireResponse : Entity
{
  public Guid PatientId { get; set; }
  public Guid AppointmentId { get; set; }
  public Guid QuestionnaireDefinitionId { get; set; }
  public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
  public string AnswersJson { get; set; } = "{}";
}
EOF

cat > "$DOMAIN/Messaging.cs" <<'EOF'
namespace PhysicallyFitPT.Domain;

public class CheckInMessageLog : Entity
{
  public Guid PatientId { get; set; }
  public Guid AppointmentId { get; set; }
  public VisitType VisitType { get; set; }
  public QuestionnaireType QuestionnaireType { get; set; }
  public DeliveryMethod Method { get; set; } = DeliveryMethod.SMS;
  public DateTimeOffset ScheduledSendAt { get; set; }
  public DateTimeOffset? AttemptedAt { get; set; }
  public DateTimeOffset? SentAt { get; set; }
  public string Status { get; set; } = "Pending";
  public string? FailureReason { get; set; }
  public string LinkTokenHash { get; set; } = Guid.NewGuid().ToString("N");
  public DateTimeOffset? QuestionnaireCompletedAt { get; set; }
}
EOF

# 6) Infrastructure: DbContext + PDF + Services
mkdir -p "$INFRA"/{Data,Services,Services/Interfaces}
cat > "$INFRA/Data/ApplicationDbContext.cs" <<'EOF'
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;

namespace PhysicallyFitPT.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<Patient> Patients => Set<Patient>();
  public DbSet<Appointment> Appointments => Set<Appointment>();
  public DbSet<Note> Notes => Set<Note>();
  public DbSet<CptCode> CptCodes => Set<CptCode>();
  public DbSet<Icd10Code> Icd10Codes => Set<Icd10Code>();
  public DbSet<QuestionnaireDefinition> QuestionnaireDefinitions => Set<QuestionnaireDefinition>();
  public DbSet<QuestionnaireResponse> QuestionnaireResponses => Set<QuestionnaireResponse>();
  public DbSet<CheckInMessageLog> CheckInMessageLogs => Set<CheckInMessageLog>();

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    var now = DateTimeOffset.UtcNow;
    foreach (var e in ChangeTracker.Entries<Entity>())
    {
      if (e.State == EntityState.Added)   e.Entity.CreatedAt = now;
      if (e.State == EntityState.Modified) e.Entity.UpdatedAt = now;
    }
    return base.SaveChangesAsync(cancellationToken);
  }

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
EOF

# Service interfaces
cat > "$INFRA/Services/Interfaces/IPatientService.cs" <<'EOF'
using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;
public interface IPatientService { Task<IEnumerable<Patient>> SearchAsync(string query, int take = 50); }
EOF

cat > "$INFRA/Services/Interfaces/IAppointmentService.cs" <<'EOF'
using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

public interface IAppointmentService
{
    Task<Appointment> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null);
    Task<bool> CancelAsync(Guid appointmentId);
    Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50);
}
EOF

cat > "$INFRA/Services/Interfaces/IAutoMessagingService.cs" <<'EOF'
using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

public interface IAutoMessagingService
{
    Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc);
    Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100);
}
EOF

cat > "$INFRA/Services/Interfaces/INoteBuilderService.cs" <<'EOF'
using PhysicallyFitPT.Domain;
namespace PhysicallyFitPT.Infrastructure.Services.Interfaces;

public interface INoteBuilderService
{
    Task<Note> CreateEvalNoteAsync(Guid patientId, Guid appointmentId);
    Task<Note?> GetAsync(Guid noteId);
    Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasure>? rom = null, IEnumerable<MmtMeasure>? mmt = null, IEnumerable<SpecialTest>? specialTests = null, IEnumerable<OutcomeMeasureScore>? outcomes = null, IEnumerable<ProvidedIntervention>? interventions = null);
    Task<bool> SignAsync(Guid noteId, string signedBy);
}
EOF

# Service implementations
cat > "$INFRA/Services/PatientService.cs" <<'EOF'
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;
public class PatientService : IPatientService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    public PatientService(IDbContextFactory<ApplicationDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<IEnumerable<Patient>> SearchAsync(string query, int take = 50)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var q = (query ?? string.Empty).Trim().ToLower();
        var like = $"%{q}%";
        return await db.Patients.AsNoTracking()
            .Where(p => EF.Functions.Like((p.FirstName + " " + p.LastName).ToLower(), like))
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Take(take).ToListAsync();
    }
}
EOF

cat > "$INFRA/Services/AppointmentService.cs" <<'EOF'
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public AppointmentService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<Appointment> ScheduleAsync(Guid patientId, DateTimeOffset start, DateTimeOffset? end, VisitType visitType, string? location = null, string? clinicianName = null, string? clinicianNpi = null)
    {
        using var db = await _factory.CreateDbContextAsync();
        var appt = new Appointment
        {
            PatientId = patientId,
            VisitType = visitType,
            ScheduledStart = start,
            ScheduledEnd = end,
            Location = location,
            ClinicianName = clinicianName,
            ClinicianNpi = clinicianNpi
        };
        db.Appointments.Add(appt);
        await db.SaveChangesAsync();
        return appt;
    }

    public async Task<bool> CancelAsync(Guid appointmentId)
    {
        using var db = await _factory.CreateDbContextAsync();
        var appt = await db.Appointments.FindAsync(appointmentId);
        if (appt is null) return false;
        db.Appointments.Remove(appt);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(Guid patientId, DateTimeOffset fromUtc, int take = 50)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Appointments.AsNoTracking()
            .Where(a => a.PatientId == patientId && a.ScheduledStart >= fromUtc)
            .OrderBy(a => a.ScheduledStart)
            .Take(take)
            .ToListAsync();
    }
}
EOF

cat > "$INFRA/Services/AutoMessagingService.cs" <<'EOF'
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class AutoMessagingService : IAutoMessagingService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public AutoMessagingService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<CheckInMessageLog> EnqueueCheckInAsync(Guid patientId, Guid appointmentId, VisitType visitType, QuestionnaireType questionnaireType, DeliveryMethod method, DateTimeOffset scheduledSendAtUtc)
    {
        using var db = await _factory.CreateDbContextAsync();
        var log = new CheckInMessageLog
        {
            PatientId = patientId,
            AppointmentId = appointmentId,
            VisitType = visitType,
            QuestionnaireType = questionnaireType,
            Method = method,
            ScheduledSendAt = scheduledSendAtUtc,
            Status = "Pending"
        };
        db.CheckInMessageLogs.Add(log);
        await db.SaveChangesAsync();
        return log;
    }

    public async Task<IReadOnlyList<CheckInMessageLog>> GetLogAsync(Guid? patientId = null, int take = 100)
    {
        using var db = await _factory.CreateDbContextAsync();
        var q = db.CheckInMessageLogs.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        if (patientId.HasValue) q = q.Where(x => x.PatientId == patientId.Value).OrderByDescending(x => x.CreatedAt);
        return await q.Take(take).ToListAsync();
    }
}
EOF

cat > "$INFRA/Services/NoteBuilderService.cs" <<'EOF'
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT.Infrastructure.Services;

public class NoteBuilderService : INoteBuilderService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public NoteBuilderService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<Note> CreateEvalNoteAsync(Guid patientId, Guid appointmentId)
    {
        using var db = await _factory.CreateDbContextAsync();
        var note = new Note
        {
            PatientId = patientId,
            AppointmentId = appointmentId,
            VisitType = VisitType.Eval,
            Subjective = new SubjectiveSection(),
            Objective  = new ObjectiveSection(),
            Assessment = new AssessmentSection(),
            Plan       = new PlanSection()
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }

    public async Task<Note?> GetAsync(Guid noteId)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == noteId);
    }

    public async Task<bool> UpdateObjectiveAsync(Guid noteId, IEnumerable<RomMeasure>? rom = null, IEnumerable<MmtMeasure>? mmt = null, IEnumerable<SpecialTest>? specialTests = null, IEnumerable<OutcomeMeasureScore>? outcomes = null, IEnumerable<ProvidedIntervention>? interventions = null)
    {
        using var db = await _factory.CreateDbContextAsync();
        var note = await db.Notes.Include(n => n.Objective).FirstOrDefaultAsync(n => n.Id == noteId);
        if (note is null) return false;

        note.Objective.Rom = rom?.ToList() ?? note.Objective.Rom;
        note.Objective.Mmt = mmt?.ToList() ?? note.Objective.Mmt;
        note.Objective.SpecialTests = specialTests?.ToList() ?? note.Objective.SpecialTests;
        note.Objective.OutcomeMeasures = outcomes?.ToList() ?? note.Objective.OutcomeMeasures;
        note.Objective.ProvidedInterventions = interventions?.ToList() ?? note.Objective.ProvidedInterventions;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SignAsync(Guid noteId, string signedBy)
    {
        using var db = await _factory.CreateDbContextAsync();
        var note = await db.Notes.FindAsync(noteId);
        if (note is null) return false;
        note.IsSigned = true;
        note.SignedBy = signedBy;
        note.SignedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }
}
EOF

cat > "$INFRA/Services/PdfRenderer.cs" <<'EOF'
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace PhysicallyFitPT.Infrastructure.Services;

public interface IPdfRenderer
{
  byte[] RenderSimple(string title, string body);
}

public class PdfRenderer : IPdfRenderer
{
  public byte[] RenderSimple(string title, string body)
  {
    return Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.DefaultTextStyle(t => t.FontSize(11));
        page.Content().Column(col =>
        {
          col.Item().Text(title).FontSize(18).SemiBold();
          col.Item().Text(body);
        });
      });
    }).GeneratePdf();
  }
}
EOF

# 7) Shared: placeholders + clinical libraries (Goals, Interventions, Outcome Measures)
mkdir -p "$SHARED"
cat > "$SHARED/Placeholders.cs" <<'EOF'
namespace PhysicallyFitPT.Shared;
public class DtoPlaceholder { }
EOF

cat > "$SHARED/GoalsLibrary.cs" <<'EOF'
namespace PhysicallyFitPT.Shared;

public static class GoalTemplates
{
    public static readonly Dictionary<string, List<string>> BodyRegionGoals = new()
    {
        ["Neck"] = new()
        {
            "Patient will perform chin tucks 2x/day with correct form within 2 weeks.",
            "Patient will turn head ≥60° bilaterally without pain in 3 weeks.",
            "Patient will don overhead shirt without discomfort in 4 weeks."
        },
        ["LowBack"] = new()
        {
            "Patient will sit for 30 minutes without low back pain in 3 weeks.",
            "Patient will transition sit-to-stand with minimal discomfort within 2 weeks.",
            "Patient will sleep through the night without waking due to back pain within 4 weeks."
        },
        ["LE"] = new()
        {
            "Patient will ascend 1 flight of stairs without rail support in 4 weeks.",
            "Patient will perform 5 sit-to-stands in 30 seconds without assist.",
            "Patient will walk 500 feet pain-free in community setting within 6 weeks."
        },
        ["UE"] = new()
        {
            "Patient will reach overhead with <2/10 pain for 5 reps within 3 weeks.",
            "Patient will carry 10 lbs for 50 ft without upper extremity fatigue in 4 weeks.",
            "Patient will brush or style hair independently without compensation in 2 weeks."
        },
        ["Pelvic"] = new()
        {
            "Patient will sit-to-stand from toilet with minimal pelvic discomfort in 3 weeks.",
            "Patient will stand for 15 minutes without flare-up by week 4.",
            "Patient will report 50% reduction in urgency episodes with HEP adherence by week 6."
        }
    };
}
EOF

cat > "$SHARED/InterventionsLibrary.cs" <<'EOF'
namespace PhysicallyFitPT.Shared;

public static class InterventionsLibrary
{
    public static readonly List<string> TreatmentCategories = new()
    {
        "ROM – Active, Passive, AAROM",
        "Joint Mobilization – Grades I–V",
        "Soft Tissue Mobilization",
        "Muscle Strengthening (Isolated or Functional)",
        "Core Stabilization",
        "Neuromuscular Re-education",
        "Postural Training",
        "Gait & Balance Training",
        "Manual Therapy",
        "Pelvic Floor Therapy",
        "Pain Neuroscience Education",
        "HEP Instruction"
    };

    public static readonly Dictionary<string, List<string>> ExerciseLibrary = new()
    {
        ["Neck"] = new() { "Chin tucks", "Cervical isometrics", "Scapular retraction with band" },
        ["Shoulder"] = new() { "Wall slides", "Pendulum swings", "External rotation with band" },
        ["Lumbar"] = new() { "Bird dog", "Prone press-ups", "Bridges" },
        ["Hip"] = new() { "Clamshells", "Hip flexor stretch", "Glute bridges" },
        ["Knee"] = new() { "Step-ups", "Terminal knee extension", "Wall sits" },
        ["Ankle"] = new() { "Calf raises", "Single-leg balance", "Towel scrunches" },
        ["Pelvic"] = new() { "Pelvic tilts", "Core-lumbopelvic coordination", "Hip adduction squeeze" }
    };
}
EOF

cat > "$SHARED/OutcomeMeasures.cs" <<'EOF'
namespace PhysicallyFitPT.Shared;

public static class OutcomeMeasures
{
    public static readonly Dictionary<string, List<string>> RegionMeasures = new()
    {
        ["Neck"] = new() { "NDI", "PSFS", "VAS" },
        ["Shoulder"] = new() { "DASH", "SPADI", "QuickDASH" },
        ["LowBack"] = new() { "ODI", "Roland-Morris", "PSFS" },
        ["Hip"] = new() { "LEFS", "HOOS" },
        ["Knee"] = new() { "LEFS", "KOOS", "Lysholm" },
        ["Ankle"] = new() { "FAAM", "LEFS" },
        ["General Balance"] = new() { "TUG", "5xSTS", "BBS", "ABC" },
        ["Whole Body"] = new() { "PSFS", "SF-36", "NPRS" }
    };
}
EOF

# 8) App wiring (DbContextFactory, SQLite init, DI, pages, docs)
mkdir -p "$APP"/{Components/Pages,Shared,wwwroot/css,docs}
mkdir -p "$APP"/Components/Pages/{Patients,Appointments,Notes,Admin,Reports,Notes/Sections}

cat > "$APP/GlobalUsings.cs" <<'EOF'
global using Microsoft.EntityFrameworkCore;
global using PhysicallyFitPT.Domain;
global using PhysicallyFitPT.Shared;
global using PhysicallyFitPT.Infrastructure.Data;
global using PhysicallyFitPT.Infrastructure.Services.Interfaces;
global using PhysicallyFitPT.Infrastructure.Services;
EOF

# MauiProgram: supports PFP_DB_PATH
awk '1' "$APP/MauiProgram.cs" > "$APP/MauiProgram.cs.bak" || true
cat > "$APP/MauiProgram.cs" <<'EOF'
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;

namespace PhysicallyFitPT;
public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

#if DEBUG
    builder.Services.AddBlazorWebViewDeveloperTools();
    builder.Logging.AddDebug();
#endif

    SQLitePCL.Batteries_V2.Init();

    var envPath = Environment.GetEnvironmentVariable("PFP_DB_PATH");
    string dbPath = !string.IsNullOrWhiteSpace(envPath)
      ? envPath!
      : Path.Combine(FileSystem.AppDataDirectory, "physicallyfitpt.db");

    builder.Services.AddDbContextFactory<ApplicationDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

    // DI: services
    builder.Services.AddScoped<IPatientService, PatientService>();
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IAutoMessagingService, AutoMessagingService>();
    builder.Services.AddScoped<INoteBuilderService, NoteBuilderService>();
    builder.Services.AddSingleton<IPdfRenderer, PdfRenderer>();

    builder.Services.AddHttpClient("integrations");
    builder.Services.AddMauiBlazorWebView();

    return builder.Build();
  }
}
EOF

# CSS tokens
cat > "$APP/wwwroot/css/design-tokens.css" <<'EOF'
:root{
  --pfp-color-lime:#9BE000;
  --pfp-color-black:#000000;
  --pfp-color-gray:#6B7280;
  --pfp-color-white:#FFFFFF;
  --pfp-color-navy:#0B2545;
  --pfp-radius:12px;
  --pfp-shadow:0 8px 20px rgba(0,0,0,.08);
  --pfp-font:'Inter', system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif;
}
body{ font-family: var(--pfp-font); color:#111; background:#fafafa; }
.pfp-card{ background:var(--pfp-color-white); border-radius:var(--pfp-radius); box-shadow:var(--pfp-shadow); padding:1rem; }
.pfp-btn{ background:var(--pfp-color-lime); color:var(--pfp-color-black); border:none; padding:.5rem .75rem; border-radius:10px; cursor:pointer; }
.mt-3{ margin-top:1rem }
EOF

# Basic pages
cat > "$APP/Components/Pages/Index.razor" <<'EOF'
@page "/"
<h2>Physically Fit PT — Dashboard</h2>
<p class="pfp-card">Pre‑Figma shell. Hook up services; skin later.</p>
EOF

cat > "$APP/Shared/NavMenu.razor" <<'EOF'
<nav class="p-3">
  <ul>
    <li><a href="/">Dashboard</a></li>
    <li><a href="/patients">Patients</a></li>
    <li><a href="/appointments">Appointments</a></li>
    <li><a href="/notes/eval">Eval Note</a></li>
    <li><a href="/admin/automessaging">Auto Messaging</a></li>
    <li><a href="/ui-kit">UI Kit</a></li>
  </ul>
</nav>
EOF

cat > "$APP/Components/Pages/Patients/Index.razor" <<'EOF'
@page "/patients"
@inject IPatientService Patients

<h3>Patients</h3>
<input @bind="search" placeholder="Search by name…" />
<button class="pfp-btn mt-3" @onclick="DoSearch">Search</button>

@if (results is null)
{
  <p class="mt-3">Enter a name to search.</p>
}
else if (results.Count == 0)
{
  <p class="mt-3">No matches.</p>
}
else
{
  <div class="mt-3">
    @foreach (var p in results)
    {
      <div class="pfp-card" style="margin-bottom:.5rem">
        <strong>@p.LastName, @p.FirstName</strong>
        <div style="color:#6B7280">MRN: @p.MRN</div>
      </div>
    }
  </div>
}

@code {
  string? search;
  List<PhysicallyFitPT.Domain.Patient> results = new();

  async Task DoSearch()
  {
    results = (await Patients.SearchAsync(search ?? "", 100)).ToList();
  }
}
EOF

cat > "$APP/Components/Pages/Appointments/Index.razor" <<'EOF'
@page "/appointments"
<h3>Appointments</h3>
<p class="pfp-card">Wire to AppointmentService later.</p>
EOF

cat > "$APP/Components/Pages/UiKit.razor" <<'EOF'
@page "/ui-kit"
<h3>UI Kit</h3>
<section class="mt-3">
  <h4>Buttons</h4>
  <button class="pfp-btn">Primary</button>
</section>
<section class="mt-3">
  <h4>Patient Card</h4>
  <div class="pfp-card"><strong>Jane Doe</strong><div style="color:#6B7280">MRN: A1234</div></div>
</section>
EOF

# _Imports.razor
IMPORTS="$APP/_Imports.razor"
if [[ ! -f "$IMPORTS" ]]; then
cat > "$IMPORTS" <<'EOF'
@using System.Net.Http
@using Microsoft.AspNetCore.Components
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.JSInterop
@using PhysicallyFitPT
@using PhysicallyFitPT.Domain
@using PhysicallyFitPT.Shared
@using PhysicallyFitPT.Infrastructure.Data
@using PhysicallyFitPT.Infrastructure.Services
@using PhysicallyFitPT.Infrastructure.Services.Interfaces
EOF
else
  for ns in "PhysicallyFitPT.Domain" "PhysicallyFitPT.Shared" "PhysicallyFitPT.Infrastructure.Data" "PhysicallyFitPT.Infrastructure.Services" "PhysicallyFitPT.Infrastructure.Services.Interfaces"; do
    grep -q "^@using $ns$" "$IMPORTS" || echo "@using $ns" >> "$IMPORTS"
  done
fi

# 9) Tests
mkdir -p "$TESTS"
cat > "$TESTS/SmokeTests.cs" <<'EOF'
using FluentAssertions;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class SmokeTests
{
  [Fact]
  public void Sanity() => true.Should().BeTrue();
}
EOF

cat > "$TESTS/PatientServiceTests.cs" <<'EOF'
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class PatientServiceTests
{
    [Fact]
    public async Task SearchAsync_Returns_Empty_On_Empty_DB()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var db = new ApplicationDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        IPatientService svc = new PatientService(factory);

        var results = await svc.SearchAsync("john", 10);
        results.Should().BeEmpty();
    }
}
EOF

cat > "$TESTS/AppointmentServiceTests.cs" <<'EOF'
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class AppointmentServiceTests
{
    [Fact]
    public async Task ScheduleAndCancel_Works_With_EmptyDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var db = new ApplicationDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        IAppointmentService svc = new AppointmentService(factory);

        var patient = new Patient { FirstName = "A", LastName = "B" };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var appt = await svc.ScheduleAsync(patient.Id, DateTimeOffset.UtcNow.AddDays(1), null, VisitType.Eval, "Room 1", "PT Jane", "1234");
        appt.Id.Should().NotBeEmpty();

        (await svc.CancelAsync(appt.Id)).Should().BeTrue();
    }
}
EOF

cat > "$TESTS/AutoMessagingServiceTests.cs" <<'EOF'
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class AutoMessagingServiceTests
{
    [Fact]
    public async Task Enqueue_Adds_Log_Row()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var db = new ApplicationDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        IAutoMessagingService svc = new AutoMessagingService(factory);

        var log = await svc.EnqueueCheckInAsync(Guid.NewGuid(), Guid.NewGuid(), VisitType.Eval, QuestionnaireType.Eval, DeliveryMethod.SMS, DateTimeOffset.UtcNow.AddHours(1));
        log.Id.Should().NotBeEmpty();
        (await svc.GetLogAsync(null, 10)).Should().ContainSingle();
    }
}
EOF

cat > "$TESTS/NoteBuilderServiceTests.cs" <<'EOF'
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;
using PhysicallyFitPT.Infrastructure.Services;
using PhysicallyFitPT.Infrastructure.Services.Interfaces;
using Xunit;

namespace PhysicallyFitPT.Tests;

public class NoteBuilderServiceTests
{
    [Fact]
    public async Task CreateEvalNote_Then_Sign_Works()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var db = new ApplicationDbContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);
        INoteBuilderService svc = new NoteBuilderService(factory);

        var patient = new Patient { FirstName = "Eval", LastName = "Patient" };
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var appt = new Appointment { PatientId = patient.Id, VisitType = VisitType.Eval, ScheduledStart = DateTimeOffset.UtcNow };
        db.Appointments.Add(appt);
        await db.SaveChangesAsync();

        var note = await svc.CreateEvalNoteAsync(patient.Id, appt.Id);
        note.VisitType.Should().Be(VisitType.Eval);

        var ok = await svc.SignAsync(note.Id, "PT Jane");
        ok.Should().BeTrue();
    }
}
EOF

# 10) Seeder console app (uses PFP_DB_PATH or ./dev.physicallyfitpt.db)
cat > "$SEEDER/Program.cs" <<'EOF'
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhysicallyFitPT.Domain;
using PhysicallyFitPT.Infrastructure.Data;

var envPath = Environment.GetEnvironmentVariable("PFP_DB_PATH");
var dbPath = string.IsNullOrWhiteSpace(envPath)
  ? Path.Combine(Directory.GetCurrentDirectory(), "dev.physicallyfitpt.db")
  : envPath!;
Console.WriteLine($"[Seeder] Using DB: {dbPath}");

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
  .UseSqlite($"Data Source={dbPath}")
  .Options;

await using var db = new ApplicationDbContext(options);
await db.Database.EnsureCreatedAsync();

if (!await db.CptCodes.AnyAsync())
{
  db.CptCodes.AddRange(
    new CptCode { Code = "97110", Description = "Therapeutic exercise" },
    new CptCode { Code = "97140", Description = "Manual therapy" },
    new CptCode { Code = "97530", Description = "Therapeutic activities" }
  );
}

if (!await db.Icd10Codes.AnyAsync())
{
  db.Icd10Codes.AddRange(
    new Icd10Code { Code = "M25.561", Description = "Pain in right knee" },
    new Icd10Code { Code = "M25.562", Description = "Pain in left knee" }
  );
}

if (!await db.Patients.AnyAsync())
{
  db.Patients.AddRange(
    new Patient { MRN = "A1001", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" },
    new Patient { MRN = "A1002", FirstName = "John", LastName = "Smith", Email = "john@example.com" }
  );
}

await db.SaveChangesAsync();
Console.WriteLine("[Seeder] Seed complete.");
EOF

# 11) Docs: Figma → .razor mapping (starter)
mkdir -p "$APP/docs"
cat > "$APP/docs/ui-mapping.md" <<'EOF'
# Figma → .razor Mapping (Starter)

| Figma component            | Razor component/snippet           | Notes                                  |
|---------------------------|-----------------------------------|----------------------------------------|
| Primary Button            | `<button class="pfp-btn">`        | Uses design tokens in `/wwwroot/css`.  |
| Card / Panel              | `<div class="pfp-card">`          | Rounded + shadow from tokens.          |
| Grid (3-col)              | utility CSS or Tailwind later     | Consider Blazor component wrapper.     |
| Text Field                | `<input />` / `<textarea>`        | Add validation + a11y states later.    |
| Patient List Item         | `<div class="pfp-card">…</div>`   | See `/Patients/Index.razor`.           |
| Note Section Container    | `<div class="pfp-card">…</div>`   | Subjective/Objective/Plan shells.      |

> Expand this table as Figma evolves; prefer shared Razor components as you identify repetition.
EOF

# 12) CI (GitHub Actions): includes MAUI workload
mkdir -p .github/workflows
[[ -f .github/workflows/build.yml ]] || cat > .github/workflows/build.yml <<'EOF'
name: ci

on:
  push:
    branches: [ main ]
  pull_request:

jobs:
  build_test:
    runs-on: macos-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Install .NET 9 SDK (with NuGet cache)
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
          include-prerelease: true
          cache: true

      - name: Install MAUI workload
        run: dotnet workload install maui

      - name: Restore
        run: dotnet restore

      - name: Enforce Code Formatting
        run: |
          dotnet tool update -g dotnet-format || dotnet tool install -g dotnet-format
          dotnet format --verify-no-changes --no-restore
          
      - name: Build & Test
        run: dotnet test --nologo --configuration Release
EOF

# 13) Optional: create initial migration & seed
if $CREATE_MIGRATION; then
  echo "• Creating initial EF Core migration…"
  dotnet tool install --global dotnet-ef >/dev/null 2>&1 || true
  dotnet ef migrations add Initial --project "$INFRA" --startup-project "$APP" || true
  dotnet ef database update      --project "$INFRA" --startup-project "$APP" || true
fi

if $SEED_DATA; then
  DB_FILE="$(pwd)/dev.physicallyfitpt.db"
  export PFP_DB_PATH="$DB_FILE"
  echo "• Seeding DB at: $PFP_DB_PATH"
  dotnet run --project "$SEEDER/$SEEDER.csproj"
  echo "• Seeded. App will use this DB when PFP_DB_PATH is set."
fi

# ---- Optional: create/apply the initial migration -------------------------------
if [ "${PFPT_RUN_MIGRATIONS:-0}" = "1" ]; then
  echo "• Running EF migrations (Infrastructure as startup)..."
  if ! ls "$INFRA"/Data/Migrations/*_Initial*.cs >/dev/null 2>&1; then
    dotnet tool run dotnet-ef migrations add Initial \
      --project "$INFRA" \
      --startup-project "$INFRA" \
      --output-dir Data/Migrations
  else
    echo "• Initial migration already exists; skipping add."
  fi

  dotnet tool run dotnet-ef database update \
    --project "$INFRA" \
    --startup-project "$INFRA"
else
  echo "• Skipping migrations (set PFPT_RUN_MIGRATIONS=1 to enable)."
fi
# ------------------------------------------------------------------------------


echo
echo "✅ Scaffold complete."
echo
echo "Next steps:"
echo "1) Open the solution in VS Code:"
echo "   code $SOLUTION.sln"
echo
echo "2) (Optional) Create migration & update DB:"
echo "   ./scaffold_physicallyfitpt_v4_mac.sh --create-migration"
echo
echo "3) (Optional) Seed data (and use at runtime):"
echo "   ./scaffold_physicallyfitpt_v4_mac.sh --seed"
echo "   export PFP_DB_PATH=\"$(pwd)/dev.physicallyfitpt.db\""
echo
echo "4) Run MAUI (Mac Catalyst):"
echo "   dotnet build -t:Run -f net9.0-maccatalyst $APP/$APP.csproj"

