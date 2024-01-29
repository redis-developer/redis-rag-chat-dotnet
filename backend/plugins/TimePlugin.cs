using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace sk_webapi.plugins;

public class TimePlugin
{
    /// <summary>
    /// Get the current date and time in the local time zone"
    /// </summary>
    /// <example>
    /// {{time.now}} => Sunday, January 12, 2025 9:15 PM
    /// </example>
    /// <returns> The current date and time in the local time zone </returns>
    [KernelFunction, Description("Get the current date and time in the local time zone")]
    public string Now(IFormatProvider? formatProvider = null) =>
        // Sunday, January 12, 2025 9:15 PM
        DateTimeOffset.Now.ToString("f", formatProvider);
}