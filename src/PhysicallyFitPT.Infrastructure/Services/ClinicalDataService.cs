// <copyright file="ClinicalDataService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using PhysicallyFitPT.Core;

namespace PhysicallyFitPT.Infrastructure.Services;

/// <summary>
/// Service for managing clinical assessment data including outcome measures, ICD-10/CPT codes,
/// and standardized assessment findings.
/// </summary>
public class ClinicalDataService
{
    private static readonly List<OutcomeMeasure> OutcomeMeasures = new()
    {
        new OutcomeMeasure
        {
            Name = "Lower Extremity Functional Scale",
            Abbreviation = "LEFS",
            BodyPart = new List<string> { "knee", "hip", "ankle", "back" },
            MinScore = 0,
            MaxScore = 80,
            Interpretation = "Higher scores = better function. MCID: 9 points",
            Mcid = 9
        },
        new OutcomeMeasure
        {
            Name = "Disabilities of Arm, Shoulder and Hand",
            Abbreviation = "DASH",
            BodyPart = new List<string> { "shoulder", "elbow", "wrist" },
            MinScore = 0,
            MaxScore = 100,
            Interpretation = "Lower scores = better function. MCID: 10 points",
            Mcid = 10
        },
        new OutcomeMeasure
        {
            Name = "QuickDASH",
            Abbreviation = "QuickDASH",
            BodyPart = new List<string> { "shoulder", "elbow", "wrist" },
            MinScore = 0,
            MaxScore = 100,
            Interpretation = "Lower scores = better function. MCID: 8 points",
            Mcid = 8
        },
        new OutcomeMeasure
        {
            Name = "Oswestry Disability Index",
            Abbreviation = "ODI",
            BodyPart = new List<string> { "back" },
            MinScore = 0,
            MaxScore = 100,
            Interpretation = "0-20%: minimal, 21-40%: moderate, 41-60%: severe, 61-80%: crippling, 81-100%: bed-bound. MCID: 10 points",
            Mcid = 10
        },
        new OutcomeMeasure
        {
            Name = "Neck Disability Index",
            Abbreviation = "NDI",
            BodyPart = new List<string> { "neck" },
            MinScore = 0,
            MaxScore = 100,
            Interpretation = "0-8%: no disability, 10-28%: mild, 30-48%: moderate, 50-68%: severe, 70-100%: complete. MCID: 7 points",
            Mcid = 7
        },
        new OutcomeMeasure
        {
            Name = "Foot and Ankle Ability Measure",
            Abbreviation = "FAAM",
            BodyPart = new List<string> { "ankle" },
            MinScore = 0,
            MaxScore = 100,
            Interpretation = "Higher scores = better function. MCID: 8 points",
            Mcid = 8
        },
        new OutcomeMeasure
        {
            Name = "Shoulder Pain and Disability Index",
            Abbreviation = "SPADI",
            BodyPart = new List<string> { "shoulder" },
            MinScore = 0,
            MaxScore = 100,
            Interpretation = "Lower scores = better function. MCID: 13 points",
            Mcid = 13
        },
        new OutcomeMeasure
        {
            Name = "Western Ontario and McMaster Universities Osteoarthritis Index",
            Abbreviation = "WOMAC",
            BodyPart = new List<string> { "knee", "hip" },
            MinScore = 0,
            MaxScore = 96,
            Interpretation = "Lower scores = better function. MCID: 12 points",
            Mcid = 12
        },
        new OutcomeMeasure
        {
            Name = "Functional Gait Assessment",
            Abbreviation = "FGA",
            BodyPart = new List<string> { "knee", "hip", "ankle", "back" },
            MinScore = 0,
            MaxScore = 30,
            Interpretation = "≥23 = low fall risk, <23 = increased fall risk",
            Mcid = null
        },
        new OutcomeMeasure
        {
            Name = "Timed Up and Go",
            Abbreviation = "TUG",
            BodyPart = new List<string> { "knee", "hip", "ankle", "back" },
            MinScore = 0,
            MaxScore = 999,
            Interpretation = "<10s: normal, 11-20s: low fall risk, >20s: high fall risk",
            Mcid = null
        }
    };

    private static readonly List<Icd10Code> Icd10Codes = new()
    {
        // Shoulder
        new Icd10Code { Code = "M25.511", Description = "Pain in right shoulder", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "shoulder", "pain", "right" } },
        new Icd10Code { Code = "M25.512", Description = "Pain in left shoulder", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "shoulder", "pain", "left" } },
        new Icd10Code { Code = "M75.100", Description = "Rotator cuff syndrome, unspecified shoulder", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "rotator", "cuff", "impingement" } },
        new Icd10Code { Code = "M75.101", Description = "Rotator cuff syndrome, right shoulder", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "rotator", "cuff", "right" } },
        new Icd10Code { Code = "M75.102", Description = "Rotator cuff syndrome, left shoulder", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "rotator", "cuff", "left" } },
        new Icd10Code { Code = "M75.50", Description = "Bursitis of shoulder, unspecified", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "bursitis", "bursa" } },
        new Icd10Code { Code = "M75.80", Description = "Frozen shoulder", BodyPart = new List<string> { "shoulder" }, Keywords = new List<string> { "frozen", "adhesive", "capsulitis" } },

        // Back/Spine
        new Icd10Code { Code = "M54.5", Description = "Low back pain", BodyPart = new List<string> { "back" }, Keywords = new List<string> { "back", "lumbar", "lbp", "lower" } },
        new Icd10Code { Code = "M54.2", Description = "Cervicalgia (neck pain)", BodyPart = new List<string> { "neck" }, Keywords = new List<string> { "neck", "cervical", "pain" } },
        new Icd10Code { Code = "M54.6", Description = "Pain in thoracic spine", BodyPart = new List<string> { "back" }, Keywords = new List<string> { "thoracic", "mid", "back" } },
        new Icd10Code { Code = "M51.26", Description = "Lumbar disc displacement", BodyPart = new List<string> { "back" }, Keywords = new List<string> { "disc", "herniation", "lumbar" } },
        new Icd10Code { Code = "M47.26", Description = "Spondylosis with radiculopathy, lumbar", BodyPart = new List<string> { "back" }, Keywords = new List<string> { "spondylosis", "radiculopathy", "lumbar" } },
        new Icd10Code { Code = "M48.06", Description = "Spinal stenosis, lumbar", BodyPart = new List<string> { "back" }, Keywords = new List<string> { "stenosis", "lumbar", "narrowing" } },

        // Knee
        new Icd10Code { Code = "M25.561", Description = "Pain in right knee", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "knee", "pain", "right" } },
        new Icd10Code { Code = "M25.562", Description = "Pain in left knee", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "knee", "pain", "left" } },
        new Icd10Code { Code = "M17.11", Description = "Osteoarthritis of right knee", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "osteoarthritis", "knee", "right", "oa" } },
        new Icd10Code { Code = "M17.12", Description = "Osteoarthritis of left knee", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "osteoarthritis", "knee", "left", "oa" } },
        new Icd10Code { Code = "M22.2X1", Description = "Patellofemoral disorders, right knee", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "patellofemoral", "pfps", "right" } },
        new Icd10Code { Code = "M23.91", Description = "Meniscus disorder, right knee", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "meniscus", "tear", "right" } },
        new Icd10Code { Code = "S83.511A", Description = "ACL sprain, right knee, initial", BodyPart = new List<string> { "knee" }, Keywords = new List<string> { "acl", "anterior", "cruciate", "right" } },

        // Hip
        new Icd10Code { Code = "M25.551", Description = "Pain in right hip", BodyPart = new List<string> { "hip" }, Keywords = new List<string> { "hip", "pain", "right" } },
        new Icd10Code { Code = "M25.552", Description = "Pain in left hip", BodyPart = new List<string> { "hip" }, Keywords = new List<string> { "hip", "pain", "left" } },
        new Icd10Code { Code = "M16.11", Description = "Osteoarthritis of right hip", BodyPart = new List<string> { "hip" }, Keywords = new List<string> { "osteoarthritis", "hip", "right" } },
        new Icd10Code { Code = "M70.60", Description = "Trochanteric bursitis", BodyPart = new List<string> { "hip" }, Keywords = new List<string> { "trochanteric", "bursitis", "gtps" } },

        // Ankle/Foot
        new Icd10Code { Code = "M25.571", Description = "Pain in right ankle", BodyPart = new List<string> { "ankle" }, Keywords = new List<string> { "ankle", "pain", "right" } },
        new Icd10Code { Code = "M25.572", Description = "Pain in left ankle", BodyPart = new List<string> { "ankle" }, Keywords = new List<string> { "ankle", "pain", "left" } },
        new Icd10Code { Code = "S93.401A", Description = "Ankle sprain, right, initial", BodyPart = new List<string> { "ankle" }, Keywords = new List<string> { "ankle", "sprain", "right" } },
        new Icd10Code { Code = "M72.2", Description = "Plantar fasciitis", BodyPart = new List<string> { "ankle" }, Keywords = new List<string> { "plantar", "fasciitis", "heel" } },
        new Icd10Code { Code = "M76.891", Description = "Achilles tendinitis, right", BodyPart = new List<string> { "ankle" }, Keywords = new List<string> { "achilles", "tendinitis", "right" } },

        // Elbow
        new Icd10Code { Code = "M25.521", Description = "Pain in right elbow", BodyPart = new List<string> { "elbow" }, Keywords = new List<string> { "elbow", "pain", "right" } },
        new Icd10Code { Code = "M77.10", Description = "Lateral epicondylitis (tennis elbow)", BodyPart = new List<string> { "elbow" }, Keywords = new List<string> { "lateral", "epicondylitis", "tennis" } },
        new Icd10Code { Code = "M77.00", Description = "Medial epicondylitis (golfer's elbow)", BodyPart = new List<string> { "elbow" }, Keywords = new List<string> { "medial", "epicondylitis", "golfer" } },

        // Wrist/Hand
        new Icd10Code { Code = "M25.531", Description = "Pain in right wrist", BodyPart = new List<string> { "wrist" }, Keywords = new List<string> { "wrist", "pain", "right" } },
        new Icd10Code { Code = "G56.00", Description = "Carpal tunnel syndrome", BodyPart = new List<string> { "wrist" }, Keywords = new List<string> { "carpal", "tunnel", "cts" } },
        new Icd10Code { Code = "M65.4", Description = "De Quervain's tenosynovitis", BodyPart = new List<string> { "wrist" }, Keywords = new List<string> { "quervain", "tenosynovitis", "thumb" } }
    };

    private static readonly List<CptCodeDetail> CptCodesDatabase = new()
    {
        new CptCodeDetail { Code = "97110", Description = "Therapeutic Exercise", TimeMin = 15, Category = "Exercise", Keywords = new List<string> { "exercise", "strengthen", "stretch", "rom" } },
        new CptCodeDetail { Code = "97112", Description = "Neuromuscular Re-education", TimeMin = 15, Category = "Neuro", Keywords = new List<string> { "balance", "proprioception", "coordination", "neuro" } },
        new CptCodeDetail { Code = "97116", Description = "Gait Training", TimeMin = 15, Category = "Gait", Keywords = new List<string> { "gait", "walking", "ambulation" } },
        new CptCodeDetail { Code = "97140", Description = "Manual Therapy", TimeMin = 15, Category = "Manual", Keywords = new List<string> { "manual", "mobilization", "manipulation", "massage" } },
        new CptCodeDetail { Code = "97530", Description = "Therapeutic Activities", TimeMin = 15, Category = "Functional", Keywords = new List<string> { "functional", "adl", "task", "activity" } },
        new CptCodeDetail { Code = "97535", Description = "Self-care/ADL Training", TimeMin = 15, Category = "ADL", Keywords = new List<string> { "adl", "self-care", "daily", "living" } },
        new CptCodeDetail { Code = "97150", Description = "Group Therapy", TimeMin = 15, Category = "Group", Keywords = new List<string> { "group", "class" } },
        new CptCodeDetail { Code = "97161", Description = "PT Evaluation - Low Complexity", TimeMin = null, Category = "Eval", Keywords = new List<string> { "evaluation", "initial", "low" } },
        new CptCodeDetail { Code = "97162", Description = "PT Evaluation - Moderate Complexity", TimeMin = null, Category = "Eval", Keywords = new List<string> { "evaluation", "initial", "moderate" } },
        new CptCodeDetail { Code = "97163", Description = "PT Evaluation - High Complexity", TimeMin = null, Category = "Eval", Keywords = new List<string> { "evaluation", "initial", "high" } },
        new CptCodeDetail { Code = "97164", Description = "PT Re-evaluation", TimeMin = null, Category = "Eval", Keywords = new List<string> { "re-evaluation", "reassessment" } },
        new CptCodeDetail { Code = "97010", Description = "Hot/Cold Packs", TimeMin = 15, Category = "Modality", Keywords = new List<string> { "hot", "cold", "ice", "heat" } },
        new CptCodeDetail { Code = "97035", Description = "Ultrasound", TimeMin = 15, Category = "Modality", Keywords = new List<string> { "ultrasound", "therapeutic" } },
        new CptCodeDetail { Code = "97032", Description = "Electrical Stimulation", TimeMin = 15, Category = "Modality", Keywords = new List<string> { "estim", "electrical", "tens", "nmes" } },
        new CptCodeDetail { Code = "97012", Description = "Mechanical Traction", TimeMin = 15, Category = "Modality", Keywords = new List<string> { "traction", "mechanical" } },
        new CptCodeDetail { Code = "97039", Description = "Unlisted Modality", TimeMin = 15, Category = "Modality", Keywords = new List<string> { "modality", "other" } },
        new CptCodeDetail { Code = "20560", Description = "Dry Needling 1-2 Muscles", TimeMin = null, Category = "Dry Needling", Keywords = new List<string> { "dry", "needling", "trigger" } },
        new CptCodeDetail { Code = "20561", Description = "Dry Needling 3+ Muscles", TimeMin = null, Category = "Dry Needling", Keywords = new List<string> { "dry", "needling", "trigger" } }
    };

    /// <summary>
    /// Searches ICD-10 codes by query text and optionally filters by body part.
    /// </summary>
    /// <param name="query">Search query (matches code, description, or keywords).</param>
    /// <param name="bodyPart">Optional body part filter.</param>
    /// <returns>List of matching ICD-10 codes (max 10 results).</returns>
    public List<Icd10Code> SearchIcd10(string query, string? bodyPart = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<Icd10Code>();
        }

        var lowerQuery = query.ToLowerInvariant();

        var results = Icd10Codes.Where(code =>
            code.Description.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase) ||
            code.Code.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase) ||
            (code.Keywords?.Any(keyword => keyword.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase)) ?? false));

        if (!string.IsNullOrWhiteSpace(bodyPart))
        {
            results = results.Where(code => code.BodyPart?.Contains(bodyPart, StringComparer.OrdinalIgnoreCase) ?? false);
        }

        return results.Take(10).ToList();
    }

    /// <summary>
    /// Searches CPT codes by query text.
    /// </summary>
    /// <param name="query">Search query (matches code, description, or keywords).</param>
    /// <returns>List of matching CPT codes (max 10 results).</returns>
    public List<CptCodeDetail> SearchCpt(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<CptCodeDetail>();
        }

        var lowerQuery = query.ToLowerInvariant();

        return CptCodesDatabase
            .Where(code =>
                code.Description.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase) ||
                code.Code.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase) ||
                code.Keywords.Any(keyword => keyword.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase)))
            .Take(10)
            .ToList();
    }

    /// <summary>
    /// Gets recommended outcome measures for a specific body part.
    /// </summary>
    /// <param name="bodyPart">The body part to get measures for.</param>
    /// <returns>List of applicable outcome measures.</returns>
    public List<OutcomeMeasure> GetRecommendedOutcomeMeasures(string bodyPart)
    {
        if (string.IsNullOrWhiteSpace(bodyPart))
        {
            return new List<OutcomeMeasure>();
        }

        return OutcomeMeasures
            .Where(measure => measure.BodyPart.Contains(bodyPart, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Gets all available outcome measures.
    /// </summary>
    /// <returns>List of all outcome measures.</returns>
    public List<OutcomeMeasure> GetAllOutcomeMeasures()
    {
        return OutcomeMeasures;
    }

    /// <summary>
    /// Gets all available ICD-10 codes.
    /// </summary>
    /// <returns>List of all ICD-10 codes.</returns>
    public List<Icd10Code> GetAllIcd10Codes()
    {
        return Icd10Codes;
    }

    /// <summary>
    /// Gets all available CPT codes.
    /// </summary>
    /// <returns>List of all CPT codes.</returns>
    public List<CptCodeDetail> GetAllCptCodes()
    {
        return CptCodesDatabase;
    }

    /// <summary>
    /// Gets ICD-10 codes filtered by body part.
    /// </summary>
    /// <param name="bodyPart">The body part to filter by.</param>
    /// <returns>List of ICD-10 codes for the specified body part.</returns>
    public List<Icd10Code> GetIcd10CodesByBodyPart(string bodyPart)
    {
        if (string.IsNullOrWhiteSpace(bodyPart))
        {
            return new List<Icd10Code>();
        }

        return Icd10Codes
            .Where(code => code.BodyPart?.Contains(bodyPart, StringComparer.OrdinalIgnoreCase) ?? false)
            .ToList();
    }

    /// <summary>
    /// Gets CPT codes filtered by category.
    /// </summary>
    /// <param name="category">The category to filter by (e.g., "Exercise", "Manual", "Eval").</param>
    /// <returns>List of CPT codes in the specified category.</returns>
    public List<CptCodeDetail> GetCptCodesByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return new List<CptCodeDetail>();
        }

        return CptCodesDatabase
            .Where(code => code.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
