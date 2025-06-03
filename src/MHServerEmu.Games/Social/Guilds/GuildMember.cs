﻿using Gazillion;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;

namespace MHServerEmu.Games.Social.Guilds
{
    public class GuildMember
    {
        public const ulong InvalidGuildId = 0;

        // This static method is for serializing Player and Avatar entity guild information,
        // rather than anything to do with GuildMember instances directly. Client-accurate.
        public static bool SerializeReplicationRuntimeInfo(Archive archive, ref ulong guildId, ref string guildName, ref GuildMembership guildMembership)
        {
            bool success = true;

            bool hasGuildInfo = guildId != InvalidGuildId;
            success &= Serializer.Transfer(archive, ref hasGuildInfo);
            if (hasGuildInfo == false)
                return success;

            // Transfer the actual guild info if there is any
            success &= Serializer.Transfer(archive, ref guildId);
            success &= Serializer.Transfer(archive, ref guildName);

            int guildMembershipValue = (int)guildMembership;
            success &= Serializer.Transfer(archive, ref guildMembershipValue);
            guildMembership = (GuildMembership)guildMembershipValue;

            return success;
        }
    }
}
