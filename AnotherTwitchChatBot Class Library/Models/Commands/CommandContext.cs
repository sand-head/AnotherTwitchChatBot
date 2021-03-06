﻿using ATCB.Library.Helpers;
using ATCB.Library.Models.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.Client;

namespace ATCB.Library.Models.Commands
{
    public class CommandContext
    {
        private TwitchClient BotClient, UserClient;
        private TwitchAPI TwitchApi;
        private ChatCommand Context;
        private bool FromConsole;

        public ChatMessageContext ChatMessage;
        public TwitchStreamContext TwitchStream;
        public ApplicationSettings Settings;
        public List<string> Commands { get; private set; }

        public CommandContext(TwitchClient botClient, TwitchClient userClient, TwitchAPI twitchApi, ChatCommand context, CommandFactory factory, ApplicationSettings settings, bool fromConsole = false)
        {
            BotClient = botClient;
            UserClient = userClient;
            TwitchApi = twitchApi;
            Context = context;
            Settings = settings;
            FromConsole = fromConsole;

            Commands = factory.ToList();
            
            // Provide information to the ChatMessageContext
            ChatMessage = new ChatMessageContext();
            if (context != null)
            {
                ChatMessage.Bits = context.ChatMessage.Bits;
                ChatMessage.IsChatBot = fromConsole;
                ChatMessage.DisplayName = context.ChatMessage.DisplayName;
                ChatMessage.IsBroadcaster = context.ChatMessage.IsBroadcaster;
                ChatMessage.IsModerator = context.ChatMessage.IsModerator;
                ChatMessage.IsModeratorOrBroadcaster = context.ChatMessage.IsBroadcaster || context.ChatMessage.IsModerator;
                ChatMessage.IsSubscriber = context.ChatMessage.IsSubscriber;
                ChatMessage.Message = context.ChatMessage.Message;
            }

            // Provide information to the TwitchStreamContext
            TwitchStream = new TwitchStreamContext();
            var channel = TwitchApi.Channels.v3.GetChannelByNameAsync(UserClient.TwitchUsername).ConfigureAwait(false).GetAwaiter().GetResult();
            TwitchStream.Game = channel.Game;
            TwitchStream.Title = channel.Status;
            TwitchStream.Username = UserClient.TwitchUsername;
        }

        public List<string> ArgumentsAsList => Context.ArgumentsAsList;
        public string ArgumentsAsString => Context.ArgumentsAsString;
        public string CommandText => Context.CommandText;

        /// <summary>
        /// Sends a message to chat through the chat bot's account.
        /// </summary>
        /// <param name="message">The chat message to be sent.</param>
        public void SendMessage(string message)
        {
            if (FromConsole)
                ConsoleHelper.WriteLine(message);
            else
                BotClient.SendMessage(message);
        }

        public class ChatMessageContext
        {
            public int Bits { get; set; }
            public string DisplayName { get; set; }
            public bool IsBroadcaster { get; set; }
            public bool IsChatBot { get; set; }
            public bool IsModerator { get; set; }
            public bool IsModeratorOrBroadcaster { get; set; }
            public bool IsSubscriber { get; set; }
            public string Message { get; set; }
        }

        public class TwitchStreamContext
        {
            public string Game { get; set; }
            public string Title { get; set; }
            public string Username { get; set; }
        }
    }
}
