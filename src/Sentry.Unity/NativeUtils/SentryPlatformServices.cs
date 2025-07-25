using System;

namespace Sentry.Unity.NativeUtils;

/// <summary>
/// These are SDK's services that are only available at runtime and cannot be baked into the SDK. The
/// <c>SentryInitialization.cs</c> is provided as <c>.cs</c> and gets compiled with the game. It sets <c>IUnityInfo</c>
/// and the <c>PlatformConfiguration</c> callback during the game's startup so that they are available during initializtion.
/// </summary>
/// <remarks>Consider this <c>internal</c>.</remarks>
public static class SentryPlatformServices
{
    private static ISentryUnityInfo? _unityInfo;

    /// <summary>
    /// The UnityInfo holds methods that rely on conditionally compilation, i.e. IL2CPP backend.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when <c>UnityInfo</c> was not set. This is expected to be automatically set during the game's startup.
    /// Thrown when <c>UnityInfo</c> is set twice.
    /// </exception>
    public static ISentryUnityInfo UnityInfo
    {
        get => _unityInfo ?? throw new InvalidOperationException(
            "The PlatformServices.UnityInfo has not been set. By default, the SDK will configure this via the " +
            "'RuntimeInitializeOnLoadMethod' in 'SentryInitialization.cs'. This needs to be set up before initializing the SDK.");
        set => _unityInfo = value;
    }

    /// <summary>
    /// The PlatformConfiguration callback is responsible for configuring the native SDK and setting up scope sync.
    /// </summary>
    public static Action<SentryUnityOptions>? PlatformConfiguration { get; set; }
}
