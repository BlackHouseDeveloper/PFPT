// <copyright file="20250909004336_Initial.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#nullable disable

namespace PhysicallyFitPT.Infrastructure.Migrations
{
  using System;
  using Microsoft.EntityFrameworkCore.Migrations;

  /// <inheritdoc />
  public partial class Initial : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "CheckInMessageLogs",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
            AppointmentId = table.Column<Guid>(type: "TEXT", nullable: false),
            VisitType = table.Column<int>(type: "INTEGER", nullable: false),
            QuestionnaireType = table.Column<int>(type: "INTEGER", nullable: false),
            Method = table.Column<int>(type: "INTEGER", nullable: false),
            ScheduledSendAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            AttemptedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            SentAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            Status = table.Column<string>(type: "TEXT", nullable: false),
            FailureReason = table.Column<string>(type: "TEXT", nullable: true),
            LinkTokenHash = table.Column<string>(type: "TEXT", nullable: false),
            QuestionnaireCompletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CheckInMessageLogs", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "CptCodes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
            Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_CptCodes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Icd10Codes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
            Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Icd10Codes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Patients",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            MRN = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
            FirstName = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
            LastName = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
            DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
            Sex = table.Column<string>(type: "TEXT", nullable: true),
            Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
            MobilePhone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
            MedicationsCsv = table.Column<string>(type: "TEXT", nullable: true),
            ComorbiditiesCsv = table.Column<string>(type: "TEXT", nullable: true),
            AssistiveDevicesCsv = table.Column<string>(type: "TEXT", nullable: true),
            LivingSituation = table.Column<string>(type: "TEXT", nullable: true),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Patients", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "QuestionnaireDefinitions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: false),
            Type = table.Column<int>(type: "INTEGER", nullable: false),
            BodyRegion = table.Column<string>(type: "TEXT", nullable: true),
            Version = table.Column<int>(type: "INTEGER", nullable: false),
            JsonSchema = table.Column<string>(type: "TEXT", nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_QuestionnaireDefinitions", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "QuestionnaireResponses",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
            AppointmentId = table.Column<Guid>(type: "TEXT", nullable: false),
            QuestionnaireDefinitionId = table.Column<Guid>(type: "TEXT", nullable: false),
            SubmittedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            AnswersJson = table.Column<string>(type: "TEXT", nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_QuestionnaireResponses", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Appointments",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
            VisitType = table.Column<int>(type: "INTEGER", nullable: false),
            ScheduledStart = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            ScheduledEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            Location = table.Column<string>(type: "TEXT", nullable: true),
            ClinicianNpi = table.Column<string>(type: "TEXT", nullable: true),
            ClinicianName = table.Column<string>(type: "TEXT", nullable: true),
            QuestionnaireSentAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            QuestionnaireCompletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            IsCheckedIn = table.Column<bool>(type: "INTEGER", nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Appointments", x => x.Id);
            table.ForeignKey(
                      name: "FK_Appointments_Patients_PatientId",
                      column: x => x.PatientId,
                      principalTable: "Patients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Notes",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
            AppointmentId = table.Column<Guid>(type: "TEXT", nullable: false),
            VisitType = table.Column<int>(type: "INTEGER", nullable: false),
            Subjective_ChiefComplaint = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_HistoryOfPresentIllness = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_PainLocationsCsv = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_PainSeverity0to10 = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_AggravatingFactors = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_EasingFactors = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_FunctionalLimitations = table.Column<string>(type: "TEXT", nullable: true),
            Subjective_PatientGoalsNarrative = table.Column<string>(type: "TEXT", nullable: true),
            Assessment_ClinicalImpression = table.Column<string>(type: "TEXT", nullable: true),
            Assessment_RehabPotential = table.Column<string>(type: "TEXT", nullable: true),
            Plan_Frequency = table.Column<string>(type: "TEXT", nullable: true),
            Plan_Duration = table.Column<string>(type: "TEXT", nullable: true),
            Plan_PlannedInterventionsCsv = table.Column<string>(type: "TEXT", nullable: true),
            Plan_NextVisitFocus = table.Column<string>(type: "TEXT", nullable: true),
            IsSigned = table.Column<bool>(type: "INTEGER", nullable: false),
            SignedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            SignedBy = table.Column<string>(type: "TEXT", nullable: true),
            CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
            CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
            UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
            UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
            IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Notes", x => x.Id);
            table.ForeignKey(
                      name: "FK_Notes_Appointments_AppointmentId",
                      column: x => x.AppointmentId,
                      principalTable: "Appointments",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Notes_Patients_PatientId",
                      column: x => x.PatientId,
                      principalTable: "Patients",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteAssessmentGoals",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            IsLongTerm = table.Column<bool>(type: "INTEGER", nullable: false),
            Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
            MeasureType = table.Column<string>(type: "TEXT", nullable: true),
            BaselineValue = table.Column<string>(type: "TEXT", nullable: true),
            TargetValue = table.Column<string>(type: "TEXT", nullable: true),
            TargetDate = table.Column<DateTime>(type: "TEXT", nullable: true),
            Status = table.Column<int>(type: "INTEGER", nullable: false),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteAssessmentGoals", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteAssessmentGoals_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteAssessmentIcd10",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Code = table.Column<string>(type: "TEXT", nullable: false),
            Description = table.Column<string>(type: "TEXT", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteAssessmentIcd10", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteAssessmentIcd10_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteObjectiveMmt",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            MuscleGroup = table.Column<string>(type: "TEXT", nullable: false),
            Side = table.Column<int>(type: "INTEGER", nullable: false),
            Grade = table.Column<string>(type: "TEXT", nullable: false),
            WithPain = table.Column<bool>(type: "INTEGER", nullable: false),
            Notes = table.Column<string>(type: "TEXT", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteObjectiveMmt", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteObjectiveMmt_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteObjectiveOutcomeMeasures",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Instrument = table.Column<string>(type: "TEXT", nullable: false),
            RawScore = table.Column<int>(type: "INTEGER", nullable: true),
            Percent = table.Column<double>(type: "REAL", nullable: true),
            CollectedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteObjectiveOutcomeMeasures", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteObjectiveOutcomeMeasures_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteObjectiveRom",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Joint = table.Column<string>(type: "TEXT", nullable: false),
            Movement = table.Column<string>(type: "TEXT", nullable: false),
            Side = table.Column<int>(type: "INTEGER", nullable: false),
            MeasuredDegrees = table.Column<int>(type: "INTEGER", nullable: true),
            NormalDegrees = table.Column<int>(type: "INTEGER", nullable: true),
            WithPain = table.Column<bool>(type: "INTEGER", nullable: false),
            Notes = table.Column<string>(type: "TEXT", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteObjectiveRom", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteObjectiveRom_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteObjectiveSpecialTests",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: false),
            Side = table.Column<int>(type: "INTEGER", nullable: false),
            Result = table.Column<int>(type: "INTEGER", nullable: false),
            Notes = table.Column<string>(type: "TEXT", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteObjectiveSpecialTests", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteObjectiveSpecialTests_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NotePlanHep",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            Name = table.Column<string>(type: "TEXT", nullable: false),
            Dosage = table.Column<string>(type: "TEXT", nullable: true),
            Notes = table.Column<string>(type: "TEXT", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NotePlanHep", x => x.Id);
            table.ForeignKey(
                      name: "FK_NotePlanHep_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "NoteProvidedInterventions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            CptCode = table.Column<string>(type: "TEXT", nullable: false),
            Description = table.Column<string>(type: "TEXT", nullable: true),
            Units = table.Column<int>(type: "INTEGER", nullable: false),
            Minutes = table.Column<int>(type: "INTEGER", nullable: true),
            NoteId = table.Column<Guid>(type: "TEXT", nullable: false),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_NoteProvidedInterventions", x => x.Id);
            table.ForeignKey(
                      name: "FK_NoteProvidedInterventions_Notes_NoteId",
                      column: x => x.NoteId,
                      principalTable: "Notes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Appointments_PatientId_ScheduledStart",
          table: "Appointments",
          columns: new[] { "PatientId", "ScheduledStart" });

      migrationBuilder.CreateIndex(
          name: "IX_CptCodes_Code",
          table: "CptCodes",
          column: "Code",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Icd10Codes_Code",
          table: "Icd10Codes",
          column: "Code");

      migrationBuilder.CreateIndex(
          name: "IX_NoteAssessmentGoals_NoteId",
          table: "NoteAssessmentGoals",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NoteAssessmentIcd10_NoteId",
          table: "NoteAssessmentIcd10",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NoteObjectiveMmt_NoteId",
          table: "NoteObjectiveMmt",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NoteObjectiveOutcomeMeasures_NoteId",
          table: "NoteObjectiveOutcomeMeasures",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NoteObjectiveRom_NoteId",
          table: "NoteObjectiveRom",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NoteObjectiveSpecialTests_NoteId",
          table: "NoteObjectiveSpecialTests",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NotePlanHep_NoteId",
          table: "NotePlanHep",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_NoteProvidedInterventions_NoteId",
          table: "NoteProvidedInterventions",
          column: "NoteId");

      migrationBuilder.CreateIndex(
          name: "IX_Notes_AppointmentId",
          table: "Notes",
          column: "AppointmentId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Notes_PatientId",
          table: "Notes",
          column: "PatientId");

      migrationBuilder.CreateIndex(
          name: "IX_Patients_LastName_FirstName",
          table: "Patients",
          columns: new[] { "LastName", "FirstName" });

      migrationBuilder.CreateIndex(
          name: "IX_QuestionnaireResponses_AppointmentId",
          table: "QuestionnaireResponses",
          column: "AppointmentId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "CheckInMessageLogs");

      migrationBuilder.DropTable(
          name: "CptCodes");

      migrationBuilder.DropTable(
          name: "Icd10Codes");

      migrationBuilder.DropTable(
          name: "NoteAssessmentGoals");

      migrationBuilder.DropTable(
          name: "NoteAssessmentIcd10");

      migrationBuilder.DropTable(
          name: "NoteObjectiveMmt");

      migrationBuilder.DropTable(
          name: "NoteObjectiveOutcomeMeasures");

      migrationBuilder.DropTable(
          name: "NoteObjectiveRom");

      migrationBuilder.DropTable(
          name: "NoteObjectiveSpecialTests");

      migrationBuilder.DropTable(
          name: "NotePlanHep");

      migrationBuilder.DropTable(
          name: "NoteProvidedInterventions");

      migrationBuilder.DropTable(
          name: "QuestionnaireDefinitions");

      migrationBuilder.DropTable(
          name: "QuestionnaireResponses");

      migrationBuilder.DropTable(
          name: "Notes");

      migrationBuilder.DropTable(
          name: "Appointments");

      migrationBuilder.DropTable(
          name: "Patients");
    }
  }
}
