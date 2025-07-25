using Sentry.Extensibility;
using Sentry.Unity.Integrations;
using UnityEngine;
using UnityEngine.Analytics;

namespace Sentry.Unity.iOS;

/// <summary>
/// Access to the Sentry native support on iOS/macOS.
/// </summary>
public static class SentryNativeCocoa
{
    /// <summary>
    /// Configures the native support.
    /// </summary>
    /// <param name="options">The Sentry Unity options to use.</param>
    public static void Configure(SentryUnityOptions options) =>
        Configure(options, ApplicationAdapter.Instance.Platform);

    // For testing
    internal static void Configure(SentryUnityOptions options, RuntimePlatform platform)
    {
        options.DiagnosticLogger?.LogInfo("Attempting to configure native support via the Cocoa SDK");

        if (!options.UnityInfo.IsNativeSupportEnabled(options, platform))
        {
            options.DiagnosticLogger?.LogDebug("Native support is disabled for: '{0}'", platform);
            return;
        }

        if (platform == RuntimePlatform.IPhonePlayer)
        {
            if (SentryCocoaBridgeProxy.IsEnabled())
            {
                options.DiagnosticLogger?.LogDebug("The native SDK is already initialized");
            }
            else if (!SentryCocoaBridgeProxy.Init(options))
            {
                options.DiagnosticLogger?.LogWarning("Failed to initialize the native SDK");
                return;
            }

            options.ScopeObserver = new NativeScopeObserver("iOS", options);
        }
        else
        {
            if (!SentryCocoaBridgeProxy.Init(options))
            {
                options.DiagnosticLogger?.LogWarning("Failed to initialize the native SDK");
                return;
            }
            options.ScopeObserver = new NativeScopeObserver("macOS", options);
        }

        SentryCocoaBridgeProxy.SetSdkName(); // Since we're not building the SDK we have to overwrite the name here

        options.NativeContextWriter = new NativeContextWriter();
        options.EnableScopeSync = true;
        options.CrashedLastRun = () =>
        {
            options.DiagnosticLogger?.LogDebug("Checking for 'CrashedLastRun'");

            var crashedLastRun = SentryCocoaBridgeProxy.CrashedLastRun() == 1;
            options.DiagnosticLogger?.LogDebug("Native SDK reported: 'crashedLastRun': '{0}'", crashedLastRun);

            return crashedLastRun;
        };

        options.NativeSupportCloseCallback += () => Close(options);
        if (options.UnityInfo.IL2CPP)
        {
            options.DefaultUserId = SentryCocoaBridgeProxy.GetInstallationId();
            if (string.IsNullOrEmpty(options.DefaultUserId))
            {
                // In case we can't get an installation ID we create one and sync that down to the native layer
                options.DiagnosticLogger?.LogDebug("Failed to fetch 'Installation ID' from the native SDK. Creating new 'Default User ID'.");

                // We fall back to Unity's Analytics Session Info: https://docs.unity3d.com/ScriptReference/Analytics.AnalyticsSessionInfo-userId.html
                // It's a randomly generated GUID that gets created immediately after installation helping
                // to identify the same instance of the game
                options.DefaultUserId = AnalyticsSessionInfo.userId;
                if (options.DefaultUserId is not null)
                {
                    options.ScopeObserver.SetUser(new SentryUser { Id = options.DefaultUserId });
                }
                else
                {
                    options.DiagnosticLogger?.LogDebug("Failed to create new 'Default User ID'.");
                }
            }
        }

        options.DiagnosticLogger?.LogInfo("Successfully configured the native SDK");
    }

    /// <summary>
    /// Closes the native Cocoa support.
    /// </summary>
    public static void Close(SentryUnityOptions options)
    {
        options.DiagnosticLogger?.LogInfo("Attempting to close the Cocoa SDK");

        if (!options.UnityInfo.IsNativeSupportEnabled(options, ApplicationAdapter.Instance.Platform))
        {
            options.DiagnosticLogger?.LogDebug("Cocoa Native Support is not enable. Skipping closing the Cocoa SDK");
            return;
        }

        options.DiagnosticLogger?.LogDebug("Closing the Cocoa SDK");
        SentryCocoaBridgeProxy.Close();
    }
}
