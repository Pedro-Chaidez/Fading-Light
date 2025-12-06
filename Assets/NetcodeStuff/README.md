# Netcode Multiplayer Implementation

This folder contains all the networking scripts for the Fading Light multiplayer system.

## Quick Start

1. **Read the Setup Guide**: See `NETCODE_SETUP.md` for detailed setup instructions
2. **Configure Player Prefab**: Add network components to your player prefab
3. **Configure Ghosts**: Ensure ghosts have NetworkObject components
4. **Set Up Scenes**: Add NetworkGameManager and NetworkWinScreenManager to game scenes

## Scripts

### Core Networking Scripts

- **NetworkPlayerMotor.cs**: Networked player movement synchronization
- **NetworkPlayerHealth.cs**: Networked player health system
- **NetworkPlayerSetup.cs**: Handles player initialization (owner vs remote)

### Game Management

- **NetworkGameManager.cs**: Manages game state and scene transitions
- **NetworkWinScreenManager.cs**: Synchronizes win condition across clients
- **NetworkGhostManager.cs**: Handles ghost destruction synchronization

### Lobby System

- **LobbyManager.cs**: Main lobby system with create/join functionality
- **LanDiscovery.cs**: LAN server discovery

### Items/Interactables

- **NetworkBanishItem.cs**: Networked version of BanishItem for ghost elimination

## Features

✅ Lobby system with LAN discovery  
✅ Synchronized player movement  
✅ Synchronized player health  
✅ Synchronized win condition  
✅ Networked ghost elimination  
✅ Player name system  
✅ Scene transition management  

## Requirements

- Unity Netcode for GameObjects 2.7.0 (already in package manifest)
- Unity Transport Package (UTP) - included with Netcode

## Testing

1. **Build and Run**: Build the game and run two instances
2. **Host Game**: In first instance, click "Host Game"
3. **Join Game**: In second instance, click "Join Game" and select the server
4. **Play**: Both players should see each other and interact with the same game world

## Notes

- The system is designed to work alongside single-player scripts
- Single-player mode will use the original scripts (DisplayWinScreen, PlayerMotor, etc.)
- Multiplayer mode automatically uses the networked versions
- All networked scripts check for NetworkManager before executing network code
