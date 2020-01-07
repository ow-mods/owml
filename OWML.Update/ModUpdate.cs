using System;
using HtmlAgilityPack;
using OWML.Common;

namespace OWML.Update
{
    public class ModUpdate
    {
        private const string BaseUrl = "https://github.com";
        public string ReleasesUrl = BaseUrl + "/amazingalek/owml/releases";

        private readonly IModConsole _writer;

        public ModUpdate(IModConsole writer)
        {
            _writer = writer;
        }

        public string GetLatestVersion()
        {
            var web = new HtmlWeb();

            string versionNumber;

            try
            {
                var releasesDoc = web.Load(ReleasesUrl);
                var releaseLink = releasesDoc.DocumentNode.QuerySelector("div.release-header a");
                versionNumber = releaseLink.InnerText;
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while getting latest version: " + ex);
                return null;
            }

            if (versionNumber.Contains("+"))
            {
                var indexOfPlus = versionNumber.IndexOf("+");
                versionNumber = versionNumber.Substring(0, indexOfPlus);
            }

            return versionNumber;
        }

    }
}
