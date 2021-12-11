// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Game.Overlays.Notifications;
using osu.Game.Updater;
using sbtw.Game.Online;

namespace sbtw.Game.Updater
{
    public class GitHubUpdateManager : UpdateManager
    {
        private string version;
        private GameHost host;

        [BackgroundDependencyLoader]
        private void load(SBTWGame game, GameHost host)
        {
            version = game.Version;
            this.host = host;
        }

        protected override async Task<bool> PerformUpdateCheck()
        {
            try
            {
                var releases = new SBTWJsonWebRequest<GitHubRelease>("https://api.github.com/repos/lenitrous/sbtw/releases/latest");

                await releases.PerformAsync().ConfigureAwait(false);

                if (releases.ResponseObject.TagName != version)
                {
                    Notifications.Post(new SimpleNotification
                    {
                        Text = $"A newer release of sbtw! is available. Click here to head over to GitHub.",
                        Icon = FontAwesome.Solid.Upload,
                        Activated = () =>
                        {
                            host.OpenUrlExternally(releases.ResponseObject.HtmlUrl);
                            return true;
                        }
                    });
                }
            }
            catch
            {
                return true;
            }

            return false;
        }
    }
}
