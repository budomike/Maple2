﻿using System.Net;
using Maple2.Model.Error;
using Maple2.PacketLib.Tools;
using Maple2.Server.Core.Constants;

namespace Maple2.Server.Core.Packets;

public static class MigrationPacket {
    public static ByteWriter LoginToGame(IPEndPoint endpoint, ulong token, int mapId) {
        var pWriter = Packet.Of(SendOp.LOGIN_TO_GAME);
        pWriter.Write<MigrationError>(MigrationError.ok);
        pWriter.WriteBytes(endpoint.Address.GetAddressBytes()); // ip
        pWriter.Write<ushort>((ushort)endpoint.Port); // port
        pWriter.Write<ulong>(token);
        pWriter.WriteInt(mapId);

        return pWriter;
    }

    public static ByteWriter LoginToGameError(MigrationError error, string message) {
        var pWriter = Packet.Of(SendOp.LOGIN_TO_GAME);
        pWriter.Write<MigrationError>(error);
        pWriter.WriteUnicodeString(message);

        return pWriter;
    }

    public static ByteWriter GameToLogin(IPEndPoint endpoint, ulong token) {
        var pWriter = Packet.Of(SendOp.GAME_TO_LOGIN);
        pWriter.Write<MigrationError>(MigrationError.ok);
        pWriter.WriteBytes(endpoint.Address.GetAddressBytes()); // ip
        pWriter.Write<ushort>((ushort)endpoint.Port); // port
        pWriter.Write<ulong>(token);

        return pWriter;
    }

    public static ByteWriter GameToLoginError(MigrationError error) {
        var pWriter = Packet.Of(SendOp.GAME_TO_LOGIN);
        pWriter.Write<MigrationError>(error);

        return pWriter;
    }

    public static ByteWriter GameToGame(IPEndPoint endpoint, ulong token) {
        var pWriter = Packet.Of(SendOp.GAME_TO_GAME);
        pWriter.Write<MigrationError>(MigrationError.ok);
        pWriter.Write<ulong>(token);
        pWriter.WriteBytes(endpoint.Address.GetAddressBytes());
        pWriter.Write<ushort>((ushort)endpoint.Port);
        pWriter.WriteInt(); // Map
        pWriter.WriteByte();

        return pWriter;
    }

    public static ByteWriter GameToGameError(MigrationError error) {
        var pWriter = Packet.Of(SendOp.GAME_TO_GAME);
        pWriter.Write<MigrationError>(error);

        return pWriter;
    }

    public static ByteWriter MoveResult(MigrationError error) {
        var pWriter = Packet.Of(SendOp.MOVE_RESULT);
        pWriter.Write<MigrationError>(error);

        return pWriter;
    }
}