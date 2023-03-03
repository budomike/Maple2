﻿using System;
using Maple2.Model;
using Maple2.Model.Game;
using Maple2.Server.Game.Manager.Field;
using Maple2.Server.Game.Packets;

namespace Maple2.Server.Game.Model;

public class FieldItem : FieldEntity<Item> {
    public IActor? Owner { get; init; }

    private readonly int despawnTick;

    public FieldItem(FieldManager field, int objectId, Item value) : base(field, objectId, value) {
        despawnTick = Environment.TickCount + (int) TimeSpan.FromMinutes(2).TotalSeconds;
    }

    public void Pickup(FieldPlayer looter) {
        if (Value.IsMeso()) {
            Field.Broadcast(ItemPickupPacket.PickupMeso(ObjectId, looter, Value.Amount));
        } else if (Value.IsStamina()) {
            Field.Broadcast(ItemPickupPacket.PickupStamina(ObjectId, looter, Value.Amount));
        } else {
            Field.Broadcast(ItemPickupPacket.PickupItem(ObjectId, looter));
        }
    }

    public override void Sync() {
        if (Environment.TickCount > despawnTick) {
            Field.RemoveItem(ObjectId);
        }
    }
}
