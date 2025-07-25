using System;
using NUnit.Framework;
using Sentry.Unity.Tests.Stubs;
using UnityEngine;

namespace Sentry.Unity.iOS.Tests;

public class SentryNativeCocoaTests
{
    [Test]
    public void Configure_DefaultConfiguration_iOS()
    {
        var options = new SentryUnityOptions(new TestUnityInfo { IL2CPP = false });

        // Note: can't test iOS - throws because it tries to call SentryCocoaBridgeProxy.Init()
        // but the bridge isn't loaded now...
        Assert.Throws<EntryPointNotFoundException>(() =>
            SentryNativeCocoa.Configure(options, RuntimePlatform.IPhonePlayer));
    }

    [Test]
    public void Configure_NativeSupportDisabled_iOS()
    {
        var unityInfo = new TestUnityInfo(true, false, false) { IL2CPP = false };
        var options = new SentryUnityOptions(unityInfo) { IosNativeSupportEnabled = false };
        SentryNativeCocoa.Configure(options, RuntimePlatform.IPhonePlayer);
        Assert.Null(options.ScopeObserver);
        Assert.Null(options.CrashedLastRun);
        Assert.False(options.EnableScopeSync);
    }

    [Test]
    public void Configure_DefaultConfiguration_macOS()
    {
        var options = new SentryUnityOptions(new TestUnityInfo { IL2CPP = false });
        // Note: can't test macOS - throws because it tries to call SentryCocoaBridgeProxy.Init()
        // but the bridge isn't loaded now...
        Assert.Throws<EntryPointNotFoundException>(() =>
            SentryNativeCocoa.Configure(options, RuntimePlatform.OSXPlayer));
    }

    [Test]
    public void Configure_NativeSupportDisabled_macOS()
    {
        var unityInfo = new TestUnityInfo(true, false, false) { IL2CPP = false };
        var options = new SentryUnityOptions(unityInfo) { MacosNativeSupportEnabled = false };
        SentryNativeCocoa.Configure(options, RuntimePlatform.OSXPlayer);
        Assert.Null(options.ScopeObserver);
        Assert.Null(options.CrashedLastRun);
        Assert.False(options.EnableScopeSync);
    }
}
