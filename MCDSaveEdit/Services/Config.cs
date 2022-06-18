﻿using MCDSaveEdit.Data;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class Config
    {
        public static Config instance = new GameAnalyticsRemoteConfig();

        public Config()
        {
            _ = downloadAsync();
        }

        public bool isConfigsReady { get; protected set; }

        public bool showInventoryIndexOrEquipmentSlot {
            get {
                return Constants.IS_DEBUG;
            }
        }

        public bool showIDsInSelectionWindow {
            get {
                return Constants.IS_DEBUG;
            }
        }

        public string? stableReleaseVersionString { get; protected set; }
        public string? betaReleaseVersionString { get; protected set; }

        protected virtual async Task downloadAsync()
        {
            //Using GitHub
            try
            {
                var request = WebRequest.Create(Constants.LATEST_RELEASE_GITHUB_URL);
                using var response = await request.GetResponseAsync();
                stableReleaseVersionString = response.ResponseUri.Segments.Last();
                betaReleaseVersionString = string.Empty;
                isConfigsReady = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public virtual string newVersionDownloadURL()
        {
            return Constants.LATEST_RELEASE_GITHUB_URL;
        }

        public string versionLabel()
        {
            if (!string.IsNullOrWhiteSpace(stableReleaseVersionString) && Constants.CURRENT_VERSION.Equals(new Version(stableReleaseVersionString!)))
            {
                return R.STABLE_VERSION_LABEL;
            }
            if (!string.IsNullOrWhiteSpace(betaReleaseVersionString) && Constants.CURRENT_VERSION.Equals(new Version(betaReleaseVersionString!)))
            {
                return R.BETA_VERSION_LABEL;
            }
            if(!string.IsNullOrWhiteSpace(betaReleaseVersionString) && Constants.CURRENT_VERSION > (new Version(betaReleaseVersionString!)))
            {
                return R.UNRELEASED_VERSION_LABEL;
            }
            return R.OLD_VERSION_LABEL;
        }

        public bool isNewBetaVersionAvailable()
        {
            if (string.IsNullOrWhiteSpace(betaReleaseVersionString))
            {
                return false;
            }
            var betaVersion = new Version(betaReleaseVersionString!);
            return Constants.CURRENT_VERSION < betaVersion;
        }

        public bool isNewStableVersionAvailable()
        {
            if(string.IsNullOrWhiteSpace(stableReleaseVersionString))
            {
                return false;
            }
            var stableVersion = new Version(stableReleaseVersionString!);
            return Constants.CURRENT_VERSION < stableVersion;
        }

    }
}
