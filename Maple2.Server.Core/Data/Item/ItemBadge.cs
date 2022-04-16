﻿using Maple2.Model.Enum;
using Maple2.PacketLib.Tools;
using Maple2.Tools;

namespace Maple2.Server.Core.Data; 

public class ItemBadge : IByteSerializable {
    public readonly int Id;
    public readonly BadgeType Type;
    public readonly bool[] Transparency;
    public int PetSkinId;

    public ItemBadge(int id) {
        Id = id;
        Type = (Id / 100000) switch {
            701 => (Id % 10) switch {
                0 => BadgeType.PetSkin,
                1 => BadgeType.Transparency,
                _ => BadgeType.AutoGather
            },
            702 => BadgeType.ChatBubble,
            703 => BadgeType.NameTag,
            704 => BadgeType.Damage,
            705 => BadgeType.Tombstone,
            706 => BadgeType.SwimTube,
            707 => BadgeType.Fishing,
            708 => BadgeType.Buddy,
            709 => BadgeType.Effect,
            _ => BadgeType.None
        };
        
        Transparency = new bool[10];
    }
    
    public void WriteTo(IByteWriter writer) {
        writer.WriteByte(1);
        writer.WriteByte((byte)Type);
        writer.WriteUnicodeString(Id.ToString());

        switch (Type) {
            case BadgeType.Transparency: // Flags for each slot
                foreach (bool toggle in Transparency) {
                    writer.WriteBool(toggle);
                }
                break;
            case BadgeType.PetSkin: // PetId for skin
                writer.WriteInt(PetSkinId);
                break;
        }
    }

    public void ReadFrom(IByteReader reader) {
        throw new System.NotImplementedException();
    }
}
