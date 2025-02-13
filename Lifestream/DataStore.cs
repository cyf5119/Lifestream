﻿using ECommons.Configuration;
using ECommons.Events;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lifestream
{
    internal class DataStore
    {
        internal const string FileName = "StaticData.json";
        internal uint[] Territories;
        internal Dictionary<TinyAetheryte, List<TinyAetheryte>> Aetherytes = new();
        internal string[] Worlds = Array.Empty<string>();
        internal string[] DCWorlds = Array.Empty<string>();
        internal StaticData StaticData;

        internal TinyAetheryte GetMaster(TinyAetheryte aetheryte)
        {
            foreach(var x in Aetherytes.Keys)
            {
                if (x.Group == aetheryte.Group) return x;
            }
            return default;
        }

        internal DataStore()
        {
            var terr = new List<uint>();
            StaticData = EzConfig.LoadConfiguration<StaticData>(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, FileName), false);
            Svc.Data.GetExcelSheet<Aetheryte>().Each(x =>
            {
                if (x.AethernetGroup != 0)
                {
                    if (x.IsAetheryte)
                    {
                        Aetherytes[GetTinyAetheryte(x)] = new();
                        terr.Add(x.Territory.Value.RowId);
                        if(!StaticData.Callback.ContainsKey(x.RowId))
                        {
                            StaticData.Callback[x.RowId] = 0;
                        }
                    }
                }
            });
            Svc.Data.GetExcelSheet<Aetheryte>().Each(x =>
            {
                if (x.AethernetGroup != 0)
                {
                    if (!x.IsAetheryte)
                    {
                        var a = GetTinyAetheryte(x);
                        Aetherytes[GetMaster(a)].Add(a);
                        terr.Add(x.Territory.Value.RowId);
                        if (!StaticData.Callback.ContainsKey(x.RowId))
                        {
                            StaticData.Callback[x.RowId] = 0;
                        }
                    }
                }
            });
            foreach(var x in Aetherytes.Keys.ToArray())
            {
                Aetherytes[x] = Aetherytes[x].OrderBy(x => GetAetheryteSortOrder(x.ID)).ToList();
            }
            Territories = terr.ToArray();
            if (ProperOnLogin.PlayerPresent)
            {
                BuildWorlds();
            }
        }
        
        internal uint GetAetheryteSortOrder(uint id)
        {
            var ret = 10000u;
            if(StaticData.SortOrder.TryGetValue(id, out var x))
            {
                ret += x;
            }
            if (P.Config.Favorites.Contains(id))
            {
                ret -= 10000u;
            }
            return ret;
        }

        internal void BuildWorlds()
        {
            BuildWorlds(Svc.ClientState.LocalPlayer.CurrentWorld.GameData.DataCenter.Value.RowId);
            if(Player.Available)
            {
                if (P.AutoRetainerApi?.Ready == true && P.Config.UseAutoRetainerAccounts)
                {
                    var data = P.AutoRetainerApi.GetOfflineCharacterData(Player.CID);
                    if (data != null)
                    {
                        P.Config.ServiceAccounts[Player.NameWithWorld] = data.ServiceAccount;
                    }
                }
                else if(!P.Config.ServiceAccounts.ContainsKey(Player.NameWithWorld))
                {
                    P.Config.ServiceAccounts[Player.NameWithWorld] = -1;
                }
            }
        }

        internal void BuildWorlds(uint dc)
        {
            Worlds = Svc.Data.GetExcelSheet<World>().Where(x => x.DataCenter.Value.RowId == dc && x.IsPublic).Select(x => x.Name.ToString()).Order().ToArray();
            PluginLog.Debug($"Built worlds: {Worlds.Print()}");
            DCWorlds = Svc.Data.GetExcelSheet<World>().Where(x => x.DataCenter.Value.RowId != dc && x.IsPublic && x.DataCenter.Value.Region == Player.Object.CurrentWorld.GameData.DataCenter.Value.Region).Select(x => x.Name.ToString()).ToArray();
            PluginLog.Debug($"Built DCworlds: {DCWorlds.Print()}");
        }

        internal TinyAetheryte GetTinyAetheryte(Aetheryte aetheryte)
        {
            var AethersX = 0f;
            var AethersY = 0f;
            if (StaticData.CustomPositions.TryGetValue(aetheryte.RowId, out var pos))
            {
                AethersX = pos.X;
                AethersY = pos.Z;
            }
            else
            {
                var map = Svc.Data.GetExcelSheet<Map>().FirstOrDefault(m => m.TerritoryType.Row == aetheryte.Territory.Value.RowId);
                var scale = map.SizeFactor;
                var mapMarker = Svc.Data.GetExcelSheet<MapMarker>().FirstOrDefault(m => (m.DataType == (aetheryte.IsAetheryte ? 3 : 4) && m.DataKey == (aetheryte.IsAetheryte ? aetheryte.RowId : aetheryte.AethernetName.Value.RowId)));
                if (mapMarker != null)
                {
                    AethersX = Util.ConvertMapMarkerToRawPosition(mapMarker.X, scale);
                    AethersY = Util.ConvertMapMarkerToRawPosition(mapMarker.Y, scale);
                }
            }
            return new(new(AethersX, AethersY), aetheryte.Territory.Value.RowId, aetheryte.RowId, aetheryte.AethernetGroup);
        }
    }
}
