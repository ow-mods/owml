using System;
using HtmlAgilityPack;
using OWML.Common;

namespace OWML.Update
{
    public class ModUpdate
    {
        public string ReleasesUrl = "https://github.com/amazingalek/owml/releases";

        private readonly IModConsole _writer;

        public ModUpdate(IModConsole writer)
        {
            _writer = writer;
        }

        public string GetLatestVersion()
        {
            var web = new HtmlWeb();
            HtmlNode releaseLink;

            try
            {
                var releasesDoc = web.Load(ReleasesUrl);
                releaseLink = releasesDoc.DocumentNode.QuerySelector("div.release-header a");
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while getting latest version: " + ex);
                return null;
            }

            var versionNumber = releaseLink?.InnerText;

            if (string.IsNullOrEmpty(versionNumber))
            {
                _writer.WriteLine("Error: version number is null or empty");
                return null;
            }

            if (versionNumber.Contains("+"))
            {
                var indexOfPlus = versionNumber.IndexOf("+");
                return versionNumber.Substring(0, indexOfPlus);
            }

            return versionNumber;
        }

    }
}
