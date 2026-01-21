// <copyright file="BodyPartDetectionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using PhysicallyFitPT.Core;

namespace PhysicallyFitPT.Infrastructure.Services;

/// <summary>
/// Service for detecting body parts from text and providing clinical data specific to each body part.
/// </summary>
public class BodyPartDetectionService
{
    private static readonly Dictionary<BodyPartRegion, BodyPartData> BodyPartDatabase = new()
    {
        [BodyPartRegion.Shoulder] = new BodyPartData
        {
            Name = "Shoulder",
            PainLocations = new List<string>
            {
                "Anterior shoulder",
                "Posterior shoulder",
                "Lateral shoulder",
                "Superior shoulder",
                "AC joint",
                "Radiating to upper arm",
                "Radiating to neck",
                "Glenohumeral joint"
            },
            FunctionalLimitations = new List<string>
            {
                "Overhead reaching",
                "Reaching behind back",
                "Lifting objects",
                "Carrying items",
                "Dressing (putting on shirt/coat)",
                "Hair care/grooming",
                "Sleeping on affected side",
                "Driving",
                "Work-related activities",
                "Sports activities"
            },
            RomMeasurements = new List<string>
            {
                "Flexion: 0-180°",
                "Extension: 0-60°",
                "Abduction: 0-180°",
                "Internal rotation: 0-70°",
                "External rotation: 0-90°",
                "Horizontal adduction",
                "Horizontal abduction"
            },
            MmtMuscles = new List<string>
            {
                "Deltoid (anterior/middle/posterior)",
                "Rotator cuff (SITS)",
                "Supraspinatus",
                "Infraspinatus",
                "Teres minor",
                "Subscapularis",
                "Biceps brachii",
                "Triceps brachii",
                "Serratus anterior",
                "Trapezius (upper/middle/lower)",
                "Rhomboids",
                "Pectoralis major"
            },
            SpecialTests = new List<string>
            {
                "Empty can test (Supraspinatus)",
                "Drop arm test",
                "Hawkins-Kennedy (impingement)",
                "Neer test (impingement)",
                "Speeds test (biceps)",
                "Yergason test (biceps)",
                "Apprehension test (instability)",
                "Cross-body adduction (AC joint)",
                "Lift-off test (subscapularis)",
                "External rotation lag sign"
            }
        },
        [BodyPartRegion.Knee] = new BodyPartData
        {
            Name = "Knee",
            PainLocations = new List<string>
            {
                "Anterior knee",
                "Posterior knee",
                "Medial knee",
                "Lateral knee",
                "Patella",
                "Patellar tendon",
                "Medial joint line",
                "Lateral joint line",
                "Popliteal fossa"
            },
            FunctionalLimitations = new List<string>
            {
                "Stair climbing",
                "Stair descending",
                "Squatting",
                "Kneeling",
                "Prolonged sitting",
                "Prolonged standing",
                "Walking on uneven surfaces",
                "Running/jogging",
                "Jumping",
                "Getting up from chair",
                "Getting in/out of car"
            },
            RomMeasurements = new List<string>
            {
                "Flexion: 0-135°",
                "Extension: 0-5°",
                "Passive flexion",
                "Active flexion",
                "Extension lag"
            },
            MmtMuscles = new List<string>
            {
                "Quadriceps",
                "Hamstrings",
                "Hip flexors",
                "Hip extensors",
                "Hip abductors",
                "Hip adductors",
                "Gastrocnemius",
                "Tibialis anterior"
            },
            SpecialTests = new List<string>
            {
                "Lachmans test (ACL)",
                "Anterior drawer (ACL)",
                "Posterior drawer (PCL)",
                "Valgus stress (MCL)",
                "Varus stress (LCL)",
                "McMurray test (meniscus)",
                "Thessaly test (meniscus)",
                "Patellar apprehension",
                "Clarkes test (patellofemoral)",
                "Noble compression (IT band)"
            }
        },
        [BodyPartRegion.Hip] = new BodyPartData
        {
            Name = "Hip",
            PainLocations = new List<string>
            {
                "Anterior hip/groin",
                "Lateral hip",
                "Posterior hip",
                "Greater trochanter",
                "Buttock",
                "Radiating to knee",
                "Deep hip joint"
            },
            FunctionalLimitations = new List<string>
            {
                "Walking",
                "Stair climbing",
                "Standing from sitting",
                "Squatting",
                "Getting in/out of car",
                "Putting on shoes/socks",
                "Hip flexion activities",
                "Crossing legs",
                "Sleeping on side",
                "Prolonged sitting",
                "Running"
            },
            RomMeasurements = new List<string>
            {
                "Flexion: 0-120°",
                "Extension: 0-20°",
                "Abduction: 0-45°",
                "Adduction: 0-30°",
                "Internal rotation: 0-45°",
                "External rotation: 0-45°"
            },
            MmtMuscles = new List<string>
            {
                "Hip flexors (iliopsoas)",
                "Hip extensors (glutes)",
                "Hip abductors (glute med/min)",
                "Hip adductors",
                "Hip internal rotators",
                "Hip external rotators",
                "Quadriceps",
                "Hamstrings",
                "TFL"
            },
            SpecialTests = new List<string>
            {
                "FABER test (Patrick test)",
                "FADIR test",
                "Scour test",
                "Trendelenburg test",
                "Thomas test (hip flexor)",
                "Ober test (IT band)",
                "Log roll test",
                "Stinchfield test",
                "Straight leg raise"
            }
        },
        [BodyPartRegion.Back] = new BodyPartData
        {
            Name = "Lower Back",
            PainLocations = new List<string>
            {
                "Central lumbar spine",
                "Left lumbar paraspinals",
                "Right lumbar paraspinals",
                "Sacroiliac joint",
                "Radiating to buttock",
                "Radiating to posterior thigh",
                "Radiating to calf",
                "Radiating to foot"
            },
            FunctionalLimitations = new List<string>
            {
                "Bending forward",
                "Bending backward",
                "Twisting/rotation",
                "Lifting objects",
                "Prolonged sitting",
                "Prolonged standing",
                "Walking",
                "Getting up from chair",
                "Reaching to floor",
                "Sleeping positions",
                "Driving"
            },
            RomMeasurements = new List<string>
            {
                "Flexion (finger-to-floor distance)",
                "Extension",
                "Right lateral flexion",
                "Left lateral flexion",
                "Right rotation",
                "Left rotation",
                "Combined movements"
            },
            MmtMuscles = new List<string>
            {
                "Erector spinae",
                "Multifidus",
                "Quadratus lumborum",
                "Abdominals",
                "Hip flexors",
                "Hip extensors",
                "Gluteus maximus",
                "Core stabilizers"
            },
            SpecialTests = new List<string>
            {
                "Straight leg raise (SLR)",
                "Slump test",
                "FABER test",
                "Gaenslen test (SI joint)",
                "Thigh thrust (SI joint)",
                "Compression test (SI joint)",
                "Distraction test (SI joint)",
                "Valsalva maneuver",
                "Quadrant test"
            }
        },
        [BodyPartRegion.Neck] = new BodyPartData
        {
            Name = "Neck",
            PainLocations = new List<string>
            {
                "Central cervical spine",
                "Left cervical paraspinals",
                "Right cervical paraspinals",
                "Upper trapezius",
                "Radiating to shoulder",
                "Radiating to arm",
                "Radiating to hand",
                "Occipital region",
                "Interscapular region"
            },
            FunctionalLimitations = new List<string>
            {
                "Looking over shoulder (driving)",
                "Overhead activities",
                "Computer/desk work",
                "Reading",
                "Sleeping",
                "Turning head",
                "Looking up/down",
                "Carrying bags",
                "Phone use",
                "Hair care/grooming"
            },
            RomMeasurements = new List<string>
            {
                "Flexion: 0-50°",
                "Extension: 0-60°",
                "Right rotation: 0-80°",
                "Left rotation: 0-80°",
                "Right lateral flexion: 0-45°",
                "Left lateral flexion: 0-45°"
            },
            MmtMuscles = new List<string>
            {
                "Cervical paraspinals",
                "Upper trapezius",
                "Middle trapezius",
                "Lower trapezius",
                "Levator scapulae",
                "Scalenes",
                "SCM (sternocleidomastoid)",
                "Deep neck flexors",
                "Rhomboids"
            },
            SpecialTests = new List<string>
            {
                "Spurlings test",
                "Distraction test",
                "Vertebral artery test",
                "Upper limb tension test",
                "Deep neck flexor endurance",
                "Cervical compression",
                "Shoulder abduction relief",
                "Adson test"
            }
        },
        [BodyPartRegion.Ankle] = new BodyPartData
        {
            Name = "Ankle/Foot",
            PainLocations = new List<string>
            {
                "Anterior ankle",
                "Posterior ankle",
                "Medial ankle",
                "Lateral ankle",
                "Achilles tendon",
                "Plantar foot",
                "Heel",
                "Forefoot",
                "Toes"
            },
            FunctionalLimitations = new List<string>
            {
                "Walking",
                "Stair climbing",
                "Stair descending",
                "Standing",
                "Running",
                "Jumping",
                "Balance activities",
                "Uneven surfaces",
                "Squatting",
                "Getting up on toes",
                "Prolonged standing"
            },
            RomMeasurements = new List<string>
            {
                "Dorsiflexion: 0-20°",
                "Plantarflexion: 0-50°",
                "Inversion: 0-35°",
                "Eversion: 0-15°",
                "Subtalar motion",
                "First MTP extension",
                "First MTP flexion"
            },
            MmtMuscles = new List<string>
            {
                "Tibialis anterior",
                "Gastrocnemius",
                "Soleus",
                "Peroneals",
                "Tibialis posterior",
                "Toe flexors",
                "Toe extensors",
                "Intrinsic foot muscles"
            },
            SpecialTests = new List<string>
            {
                "Anterior drawer (ankle)",
                "Talar tilt test",
                "Thompson test (Achilles)",
                "Single leg heel raise",
                "Tinel sign (tarsal tunnel)",
                "Windlass test (plantar fascia)",
                "Squeeze test (syndesmosis)",
                "External rotation test",
                "Kleiger test"
            }
        },
        [BodyPartRegion.Elbow] = new BodyPartData
        {
            Name = "Elbow",
            PainLocations = new List<string>
            {
                "Medial elbow",
                "Lateral elbow",
                "Anterior elbow",
                "Posterior elbow",
                "Olecranon",
                "Radiating to forearm",
                "Radiating to wrist"
            },
            FunctionalLimitations = new List<string>
            {
                "Gripping objects",
                "Lifting",
                "Carrying",
                "Pushing/pulling",
                "Computer use/typing",
                "Writing",
                "Eating activities",
                "Opening jars/doors",
                "Sports activities",
                "Work-related tasks"
            },
            RomMeasurements = new List<string>
            {
                "Flexion: 0-150°",
                "Extension: 0-0°",
                "Supination: 0-80°",
                "Pronation: 0-80°"
            },
            MmtMuscles = new List<string>
            {
                "Biceps brachii",
                "Triceps brachii",
                "Brachialis",
                "Brachioradialis",
                "Wrist extensors",
                "Wrist flexors",
                "Supinator",
                "Pronator teres"
            },
            SpecialTests = new List<string>
            {
                "Cozen test (lateral epicondylitis)",
                "Mill test (lateral epicondylitis)",
                "Golfers elbow test (medial epicondylitis)",
                "Valgus stress test",
                "Varus stress test",
                "Tinel sign (ulnar nerve)"
            }
        },
        [BodyPartRegion.Wrist] = new BodyPartData
        {
            Name = "Wrist/Hand",
            PainLocations = new List<string>
            {
                "Dorsal wrist",
                "Volar wrist",
                "Radial wrist",
                "Ulnar wrist",
                "Carpal tunnel area",
                "Thumb",
                "Fingers",
                "Palm"
            },
            FunctionalLimitations = new List<string>
            {
                "Gripping objects",
                "Pinching",
                "Writing",
                "Typing/computer use",
                "Opening jars/bottles",
                "Turning doorknobs",
                "Carrying bags",
                "Fine motor tasks",
                "Cooking/food prep",
                "Personal care activities",
                "Work-related tasks"
            },
            RomMeasurements = new List<string>
            {
                "Wrist flexion: 0-80°",
                "Wrist extension: 0-70°",
                "Radial deviation: 0-20°",
                "Ulnar deviation: 0-30°",
                "Thumb opposition",
                "Finger flexion/extension",
                "Grip strength"
            },
            MmtMuscles = new List<string>
            {
                "Wrist flexors",
                "Wrist extensors",
                "Finger flexors",
                "Finger extensors",
                "Thumb muscles",
                "Intrinsic hand muscles",
                "Opponens pollicis",
                "Lumbricals"
            },
            SpecialTests = new List<string>
            {
                "Phalen test (carpal tunnel)",
                "Reverse Phalen test",
                "Tinel sign (carpal tunnel)",
                "Finkelstein test (De Quervain)",
                "Grip strength test",
                "Pinch strength test",
                "Watson test (scaphoid)",
                "Piano key test"
            }
        }
    };

    /// <summary>
    /// Detects the body part region from a chief complaint text using keyword matching.
    /// </summary>
    /// <param name="chiefComplaint">The patient's chief complaint text.</param>
    /// <returns>The detected body part region, or null if no match found.</returns>
    public BodyPartRegion? DetectBodyPart(string chiefComplaint)
    {
        if (string.IsNullOrWhiteSpace(chiefComplaint))
        {
            return null;
        }

        var text = chiefComplaint.ToLowerInvariant();

        // Check for body parts in order of specificity
        if (text.Contains("shoulder"))
        {
            return BodyPartRegion.Shoulder;
        }

        if (text.Contains("knee"))
        {
            return BodyPartRegion.Knee;
        }

        if (text.Contains("hip"))
        {
            return BodyPartRegion.Hip;
        }

        if (text.Contains("ankle") || text.Contains("foot"))
        {
            return BodyPartRegion.Ankle;
        }

        if (text.Contains("elbow"))
        {
            return BodyPartRegion.Elbow;
        }

        if (text.Contains("wrist") || text.Contains("hand"))
        {
            return BodyPartRegion.Wrist;
        }

        if (text.Contains("neck") || text.Contains("cervical"))
        {
            return BodyPartRegion.Neck;
        }

        if (text.Contains("back") || text.Contains("lumbar") || text.Contains("spine"))
        {
            return BodyPartRegion.Back;
        }

        return null;
    }

    /// <summary>
    /// Gets the clinical data for a specific body part region.
    /// </summary>
    /// <param name="region">The body part region.</param>
    /// <returns>The clinical data, or null if region not found.</returns>
    public BodyPartData? GetBodyPartData(BodyPartRegion region)
    {
        return BodyPartDatabase.TryGetValue(region, out var data) ? data : null;
    }

    /// <summary>
    /// Gets all available body part regions.
    /// </summary>
    /// <returns>List of all supported body part regions.</returns>
    public IEnumerable<BodyPartRegion> GetAllBodyPartRegions()
    {
        return BodyPartDatabase.Keys;
    }

    /// <summary>
    /// Gets mock previous note data for a patient.
    /// In production, this would query the database for the most recent note.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <param name="noteType">The note type (e.g., "evaluation", "daily").</param>
    /// <returns>Mock previous note data.</returns>
    public PreviousNoteData GetPreviousNoteData(string patientId, string noteType)
    {
        // Mock data - in production, query database for previous note
        return new PreviousNoteData
        {
            ChiefComplaint = "Right shoulder pain with limited overhead reaching",
            PainLevel = new List<int> { 6 },
            PainLocation = "Lateral shoulder",
            PainDescription = "Sharp pain with overhead activities, dull ache at rest",
            Limitations = "Overhead reaching, Lifting objects, Sleeping on affected side",
            Rom = "Flexion: 160° (limited), Abduction: 140° (limited), External rotation: 70° (limited)",
            Mmt = "Deltoid: 4/5, Rotator cuff: 3+/5, Biceps: 5/5",
            Diagnosis = "Right shoulder impingement syndrome",
            Goals = new List<string>
            {
                "Increase shoulder flexion to 180° within 4 weeks",
                "Reduce pain to 2/10 with overhead activities",
                "Return to work duties without limitation"
            }
        };
    }
}
