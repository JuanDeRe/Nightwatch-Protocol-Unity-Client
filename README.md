# 🕹️ Nightwatch Protocol: Unity Client

The Unity 6 (C#) client of Nightwatch Protocol, a real-time asymmetric multiplayer horror game. 
It renders the game and talks to the backend; it never decides the official game state.

## What was achieved
The full player journey runs against the real backend:
1. **Register** and **log in**
2. **Join a waiting-room match**, or **create a room and invite friends** with a code
3. **Enter a match** as the guard or an animatronic
4. See everything **synchronized in real time**: player movement, doors, item collection, security cameras and the other players
5. A **shared match clock** (12 AM → 6 AM) driven by the server's start time, so every player sees the same time

## 🎥 Gameplay
[![Nightwatch Protocol Demo](docs/demo-thumbnail.png)](https://youtu.be/gWeziQBm-as)

## Structure (`Assets/Scripts`)
| Folder | Purpose |
|---|---|
| `Networking` | REST clients for Auth and Matchmaking, WebSocket `RealtimeClient` |
| `Contracts` | Message/event DTOs shared with the backend |
| `Session` | Session and JWT handling |
| `UI` | Login, lobby and game controllers |
| `Gameplay` | Player, door and security-camera views and registries, camera follow |

## Tech stack
Unity 6 (6000.4) · C# · REST/JSON · WebSockets · TextMeshPro

## Run it
1. Open with Unity 6000.4.x
2. Set the backend URLs in `Assets/Scripts/Configuration/ApiConfiguration.cs` (defaults point to `localhost`)
3. Start the backend services (see the [main repo](https://github.com/Nightwatch-Protocol/nightwatch-protocol)), open the login scene and press Play

## Limitations
- Battery system: UI placeholder only
- Special abilities, flash/shock and camera sabotage are not implemented
- Reaction to match end (win/lose screen) <!-- VERIFY: not found in main -->

## Assets
<!-- VERIFY: sprites resemble Five Nights at Freddy's characters. If they are third-party, add credits / a non-commercial fan-project disclaimer, or replace them. -->

## Part of Nightwatch Protocol
[Main repo](https://github.com/Nightwatch-Protocol/nightwatch-protocol) · **Unity Client** · [Auth](https://github.com/JuanDeRe/NP-AuthService) · [Matchmaking](https://github.com/JuanDeRe/NP-MatchmakingService) · [Gateway](https://github.com/JuanDeRe/NP-RealtimeGateway) · [Game Service](https://github.com/JuanDeRe/NP-GameService) · [Terraform](https://github.com/JuanDeRe/NP-Terraform)
