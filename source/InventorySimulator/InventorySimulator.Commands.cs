﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace InventorySimulator;

public partial class InventorySimulator
{
  [ConsoleCommand("css_ws", "Refreshes player's inventory.")]
  public void OnWSCommand(CCSPlayerController? player, CommandInfo _)
  {
    player?.PrintToChat(Localizer["invsim.announce", GetApiUrl()]);

    if (!Config.Invsim_ws_enabled || player == null) return;
    if (PlayerCooldownManager.TryGetValue(player.SteamID, out var timestamp))
    {
      var cooldown = Config.Invsim_ws_cooldown;
      var diff = Now() - timestamp;
      if (diff < cooldown)
      {
        player.PrintToChat(Localizer["invsim.ws_cooldown", cooldown - diff]);
        return;
      }
    }

    if (FetchingPlayerInventory.Contains(player.SteamID))
    {
      player.PrintToChat(Localizer["invsim.ws_in_progress"]);
      return;
    }

    RefreshPlayerInventory(player, true);
    player.PrintToChat(Localizer["invsim.ws_new"]);
  }
}
