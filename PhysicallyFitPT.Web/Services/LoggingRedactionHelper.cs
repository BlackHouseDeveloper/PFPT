namespace PhysicallyFitPT.Web.Services;

/// <summary>
/// Helper class to redact sensitive information from logs for HIPAA compliance
/// </summary>
public static class LoggingRedactionHelper
{
    /// <summary>
    /// Sensitive keys that should be redacted from logs to maintain HIPAA compliance
    /// </summary>
    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "note",
        "answersJson", 
        "diagnosis",
        "medications",
        "chiefComplaint",
        "historyOfPresentIllness",
        "painLocationsCsv",
        "aggravatingFactors",
        "easingFactors",
        "functionalLimitations",
        "patientGoalsNarrative",
        "clinicalImpression",
        "rehabPotential",
        "firstName",
        "lastName",
        "email",
        "mobilePhone",
        "mrn",
        "dateOfBirth"
    };

    /// <summary>
    /// Redacts sensitive information from a log message
    /// </summary>
    /// <param name="logMessage">The original log message</param>
    /// <returns>The redacted log message</returns>
    public static string RedactSensitiveInfo(string logMessage)
    {
        if (string.IsNullOrEmpty(logMessage))
            return logMessage;

        var redactedMessage = logMessage;
        
        foreach (var sensitiveKey in SensitiveKeys)
        {
            // Redact key-value patterns like "firstName: John" or "note=some text"
            // Use a more comprehensive regex that captures everything after the key until a delimiter or end of string
            redactedMessage = System.Text.RegularExpressions.Regex.Replace(
                redactedMessage,
                $@"{sensitiveKey}\s*[:=]\s*([^\s,;]+(?:\s+[^\s,;]+)*)",
                $"{sensitiveKey}: [REDACTED]",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        return redactedMessage;
    }

    /// <summary>
    /// Creates a HIPAA-safe log entry that only includes non-sensitive identifiers
    /// </summary>
    /// <param name="action">The action being performed</param>
    /// <param name="entityId">The entity ID (safe to log)</param>
    /// <param name="userId">The user ID performing the action</param>
    /// <returns>A safe log message</returns>
    public static string CreateSafeLogEntry(string action, Guid? entityId, string? userId = null)
    {
        var logParts = new List<string> { $"Action: {action}" };
        
        if (entityId.HasValue)
            logParts.Add($"EntityId: {entityId}");
        
        if (!string.IsNullOrEmpty(userId))
            logParts.Add($"UserId: {userId}");
            
        logParts.Add($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

        return string.Join(" | ", logParts);
    }
}