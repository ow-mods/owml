using System;
using HtmlAgilityPack;
using OWML.Common;

namespace OWML.Update
{
    public class CheckVersion
    {
        public string GitHubReleasesUrl = "https://github.com/amazingalek/owml/releases";
        public string NexusModsUrl = "https://www.nexusmods.com/outerwilds/mods/";

        private readonly IModConsole _writer;

        public CheckVersion(IModConsole writer)
        {
            _writer = writer;
        }

        public string GetOwmlVersion()
        {
            var versionNumber = GetVersion(GitHubReleasesUrl, "div.release-header a");
            if (versionNumber != null && versionNumber.Contains("+"))
            {
                var indexOfPlus = versionNumber.IndexOf("+");
                return versionNumber.Substring(0, indexOfPlus);
            }
            return versionNumber;
        }

        public string GetNexusVersion(string appId)
        {
            if (string.IsNullOrEmpty(appId))
            {
                _writer.WriteLine("Nexus app ID is null or empty.");
                return null;
            }
            return GetVersion(NexusModsUrl + appId, "li.stat-version div.stat");
        }

        private string GetVersion(string url, string selector)
        {
            var web = new HtmlWeb();
            HtmlNode versionNode;

            try
            {
                var releasesDoc = web.Load(url);
                versionNode = releasesDoc.DocumentNode.QuerySelector(selector);
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Error while getting version in {url}: {ex}");
                return null;
            }

            var versionNumber = versionNode?.InnerText;

            if (string.IsNullOrEmpty(versionNumber))
            {
                _writer.WriteLine("Error: version number is null or empty in " + url);
                return null;
            }

            return versionNumber;
        }
    }
}
