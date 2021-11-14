// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Game.Online.Chat;
using osu.Game.Users;

namespace sbtw.Game
{
    public class SBTWOutputManager : ChannelManager
    {
        private static readonly User user_verbose = new User
        {
            Id = 0,
            Username = "verbose",
            Colour = @"425e8a",
        };

        private static readonly User user_error = new User
        {
            Id = 0,
            Username = "error",
            Colour = @"c21111"
        };

        private long lastChannelId = -1;
        private Channel output;

        [BackgroundDependencyLoader]
        private void load()
        {
            output = addChannel("output", "messages related to project output are logged here.");
        }

        public void Post(string message, LogLevel logLevel = LogLevel.Verbose)
        {
            User user;

            switch (logLevel)
            {
                case LogLevel.Error:
                    user = user_error;
                    break;

                default:
                    user = user_verbose;
                    break;
            }

            output.AddNewMessages(new Message
            {
                Sender = user,
                Content = message,
                Timestamp = DateTimeOffset.Now,
            });
        }

        private Channel addChannel(string name, string desc)
        {
            var channel = new Channel
            {
                Name = name,
                Topic = desc,
                Type = ChannelType.System,
                Id = lastChannelId++
            };

            ((BindableList<Channel>)AvailableChannels).Add(channel);
            JoinChannel(channel);

            return channel;
        }
    }
}
