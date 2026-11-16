using Toaster.Configuration;

namespace Toaster;

/// <summary>A request to show one toast. Serialised as JSON between instances.</summary>
public record ToastRequest(
    string Text,
    string? Title,
    int TimeoutSeconds,
    ToastLocationType Location = ToastLocationType.BottomRight,
    ToastMonitorType Monitor = ToastMonitorType.Primary,
    string? BackgroundColour = null);
