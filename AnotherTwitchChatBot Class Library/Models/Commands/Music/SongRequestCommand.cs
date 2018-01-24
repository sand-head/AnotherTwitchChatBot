﻿using ATCB.Library.Helpers;
using ATCB.Library.Models.Misc;
using ATCB.Library.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.Client;
using YoutubeExplode;
using YoutubeSearch;

namespace ATCB.Library.Models.Commands.Music
{
    public class SongRequestCommand : Command
    {
        private string[] synonyms = { "songrequest", "sr", "songreq" };
        private YoutubeClient client;

        public SongRequestCommand()
        {
            client = new YoutubeClient();
        }

        public override bool IsSynonym(string commandText) => synonyms.Contains(commandText);

        public override void Run(CommandContext context)
        {
            if (context.ArgumentsAsList.Count < 1)
            {
                context.SendMessage("You didn't give me anything to add.");
            }
            else
            {
                var success = YoutubeClient.TryParseVideoId(context.ArgumentsAsString, out string videoId);

                if (!success)
                {
                    // Request by YouTube query
                    var search = new VideoSearch();
                    var video = search.SearchQuery(context.ArgumentsAsString, 1).Where(x => TimeSpanHelper.ConvertDurationToTimeSpan(x.Duration).TotalMinutes < 8.0).FirstOrDefault();
                    if (video != null)
                    {
                        MakeRequest(YoutubeClient.ParseVideoId(video.Url), context);
                    }
                    else
                    {
                        context.SendMessage("Sorry! I couldn't find a song like the one you wanted!");
                    }
                }
                else
                {
                    // Request by YouTube URL
                    MakeRequest(videoId, context);
                }
            }
        }

        private void MakeRequest(string videoId, CommandContext context)
        {
            var videoTitle = Task.Run(() => this.client.GetVideoAsync(videoId)).Result.Title;
            context.SendMessage($"@{context.ChatMessage.DisplayName} Your request, \"{videoTitle}\", is #{GlobalVariables.GlobalPlaylist.RequestedSongCount + 1} in the queue!");
            GlobalVariables.GlobalPlaylist.Enqueue(new RequestedSong(videoId, context.ChatMessage.DisplayName));
        }
    }
}
