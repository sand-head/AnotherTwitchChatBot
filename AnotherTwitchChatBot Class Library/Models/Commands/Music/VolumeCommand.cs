﻿using ATCB.Library.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.Client;

namespace ATCB.Library.Models.Commands.Music
{
    public class VolumeCommand : Command
    {
        public VolumeCommand() { }

        public override bool IsSynonym(string commandText) => commandText.Equals("volume");

        public override void Run(ChatCommand context, TwitchClient client)
        {
            if (context.ArgumentsAsList.Count > 0)
            {
                var success = double.TryParse(context.ArgumentsAsList[0], out double newVolume);
                if (success && newVolume <= 100.0)
                {
                    var volumeAsFloat = (float)newVolume / 100;
                    GlobalVariables.GlobalPlaylist.SetVolume(volumeAsFloat);
                    client.SendMessage($"Set the volume to {context.ArgumentsAsList[0]}.");
                }
            }
        }
    }
}
