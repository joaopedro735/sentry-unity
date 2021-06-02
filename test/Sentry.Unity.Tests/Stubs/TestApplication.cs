﻿using System;
using Sentry.Unity.Integrations;
using UnityEngine;
using UnityEngine.TestTools.Constraints;

namespace Sentry.Unity.Tests.Stubs
{
    internal sealed class TestApplication : IApplication
    {
        public TestApplication(
            bool isEditor = true,
            string productName = "",
            string version = "",
            string persistentDataPath = "")
        {
            IsEditor = isEditor;
            ProductName = productName;
            Version = version;
            PersistentDataPath = persistentDataPath;
        }

        public event Application.LogCallback? LogMessageReceived;
        public event Action? Quitting;
        public string ActiveSceneName => "TestSceneName";
        public bool IsEditor { get; }
        public string ProductName { get; }
        public string Version { get; }
        public string PersistentDataPath { get; }

        private void OnQuitting() => Quitting?.Invoke();

        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
            => LogMessageReceived?.Invoke(condition, stacktrace, type);
    }
}
