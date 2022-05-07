﻿using Maple2.Database.Context;
using Maple2.File.Flat;
using Maple2.File.Flat.maplestory2library;
using Maple2.File.IO;
using Maple2.File.Parser.Flat;
using Maple2.File.Parser.MapXBlock;
using Maple2.Model.Metadata;
using static M2dXmlGenerator.FeatureLocaleFilter;

namespace Maple2.File.Ingest.Mapper;

public class MapEntityMapper : TypeMapper<MapEntity> {
    private readonly HashSet<string> xBlocks;
    private readonly XBlockParser parser;

    public MapEntityMapper(MetadataContext db, M2dReader exportedReader) {
        xBlocks = db.MapMetadata.Select(metadata => metadata.XBlock).ToHashSet();
        var index = new FlatTypeIndex(exportedReader);
        parser = new XBlockParser(exportedReader, index);
    }

    private IEnumerable<MapEntity> ParseMap(string xblock, IEnumerable<IMapEntity> entities) {
        foreach (IMapEntity entity in entities) {
            switch (entity) {
                case IPortal portal:
                    if (!FeatureEnabled(portal.feature) || !HasLocale(portal.locale)) continue;
                    yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                        Block = new Portal(portal.PortalID, portal.TargetFieldSN, portal.TargetPortalID, (byte) portal.PortalType, portal.ActionType, portal.Position, portal.Rotation, portal.PortalDimension, portal.frontOffset, portal.IsVisible, portal.MinimapIconVisible, portal.PortalEnable)
                    };
                    continue;
                case ISpawnPoint spawn:
                    switch (spawn) {
                        case ISpawnPointPC pcSpawn:
                            yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                                Block = new SpawnPointPC(pcSpawn.SpawnPointID, pcSpawn.Position, pcSpawn.Rotation, pcSpawn.IsVisible, pcSpawn.Enable)
                            };
                            continue;
                    }
                    continue;
                case IMS2PhysXProp physXProp:
                    switch (physXProp) {
                        case IMS2Liftable liftable:
                            yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                                Block = new Liftable((int) liftable.ItemID, liftable.ItemStackCount, liftable.LiftableLifeTime, liftable.LiftableRegenCheckTime, liftable.LiftableFinishTime, liftable.MaskQuestID, liftable.MaskQuestState, liftable.EffectQuestID, liftable.EffectQuestState, liftable.Position, liftable.Rotation)
                            };
                            continue;
                        case IMS2TaxiStation taxiStation:
                            yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                                Block = new TaxiStation(taxiStation.Position, taxiStation.Rotation)
                            };
                            continue;
                    }
                    continue;
                case IMS2Breakable breakable:
                    switch (breakable) {
                        case IMS2BreakableActor actor:
                            yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                                Block = new Breakable(actor.IsVisible, (int) actor.TriggerBreakableID, actor.hideTimer, actor.resetTimer)
                            };
                            continue;
                        case IMS2BreakableNIF nif:
                            yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                                Block = new Breakable(nif.IsVisible, (int) nif.TriggerBreakableID, nif.hideTimer, nif.resetTimer)
                            };
                            continue;
                    }
                    continue;
                case IMS2InteractObject interactObject:
                    switch (interactObject) {
                        case IMS2InteractActor actor:
                            yield return new MapEntity(xblock, new Guid(entity.EntityId), entity.EntityName) {
                                Block = new InteractActor(actor.interactID, actor.MinimapInVisible, actor.Position, actor.Rotation)
                            };
                            continue;
                    }
                    continue;
                // case IMS2TriggerObject triggerObject:
                //     switch (triggerObject) {
                //         case IMS2TriggerActor actor:
                //             continue;
                //         case IMS2TriggerAgent agent:
                //             continue;
                //         case IMS2TriggerBlock block:
                //             continue;
                //         case IMS2TriggerBox box:
                //             continue;
                //         case IMS2TriggerCamera camera:
                //             continue;
                //         case IMS2TriggerCube cube:
                //             continue;
                //         case IMS2TriggerEffect effect:
                //             continue;
                //         case IMS2TriggerLadder ladder:
                //             continue;
                //         case IMS2TriggerMesh mesh:
                //             continue;
                //         case IMS2TriggerPortal _:
                //             throw new InvalidOperationException("IMS2TriggerPortal should be parsed as IPortal.");
                //         case IMS2TriggerRope rope:
                //             continue;
                //         case IMS2TriggerSkill skill:
                //             continue;
                //         case IMS2TriggerSound sound:
                //             continue;
                //     }
                //     continue;
            }
        }
    }

    protected override IEnumerable<MapEntity> Map() {
        IEnumerable<MapEntity> results = Enumerable.Empty<MapEntity>();
        parser.Parse((xblock, entities) => {
            xblock = xblock.ToLower();
            if (!xBlocks.Contains(xblock)) {
                return;
            }

            results = results.Concat(ParseMap(xblock, entities));
        });

        return results;
    }
}