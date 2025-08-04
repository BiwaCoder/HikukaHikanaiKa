# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**HikukaHikanaika** is a Unity game where players participate in a gacha-based reality show. The core gameplay involves pulling gacha to determine character appearance/beauty stats, then competing in reality show challenges.

## Project Structure

- **Unity Version**: 2021.3.28f1
- **Main Scripts**: Located in `Assets/Scripts/`
  - `BeautyGachaManager.cs`: Core gacha system for beauty/appearance attributes
- **Assets**: 
  - `Assets/Font/`: Custom fonts (Nosutaru-dotMPlusH-10-Regular.ttf)
  - `Assets/Scenes/`: Unity scenes (SampleScene.unity)
- **Project Settings**: Standard Unity configuration in `ProjectSettings/`

## Game Architecture

### Core Systems

**BeautyGachaManager** (`Assets/Scripts/BeautyGachaManager.cs`):
- Manages gacha pulls for beauty/appearance attributes
- Implements weighted probability system with entries like:
  - "オートクチュールのドレス" (10% - highest tier)
  - "制服しか勝たん" (20%)  
  - "量産型ガーリー" (20%)
  - "清潔感あるけど量販感" (30%)
  - "サイズ合ってない" (20% - lowest tier)
- Uses virtual currency system (starts with 300M yen, 30M per pull)
- Connected to Unity UI system (Button, Text components)

## Development Commands

This is a standard Unity project with no custom build scripts. Use Unity Editor for development:

- **Open Project**: Open the project folder in Unity 2021.3.28f1
- **Build**: Use Unity's Build Settings (File → Build Settings)
- **Test**: Use Unity's Test Runner (Window → General → Test Runner)
- **Package Management**: Unity Package Manager handles dependencies

## Dependencies

Key Unity packages (from `Packages/manifest.json`):
- `com.unity.ugui@1.0.0`: UI system for gacha interface
- `com.unity.textmeshpro@3.0.6`: Text rendering
- `com.unity.test-framework@1.1.33`: Testing framework
- `com.unity.visualscripting@1.8.0`: Visual scripting support
- Standard Unity modules for core functionality

## Development Notes

- Game text and UI elements are in Japanese
- Uses Unity's built-in Random system with time-based seeding
- Monetary values use large numbers (millions/billions of yen)
- No external build tools or CI/CD configuration present