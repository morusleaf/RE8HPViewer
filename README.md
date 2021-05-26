# RE8 Enemies HP Viewer

## How to use

1. Run "Resident Evil Village" game.

2. Run `RE8HPView.exe`.

3. Type in `1` or `2`, followed by `enter`.

4. The program should start hooking the game process and pulling enemies HP.

   Note: the first column is the normalized damage from your last attack, the actual damage will be multiplied by DA (dynamic adjustment).

## How to build

1. Pull this repo.
2. Pull the [ProcessMemory](https://github.com/Squirrelies/ProcessMemory) repo.
3. Pull the [SRTPulginProviderRE8](https://github.com/VideoGameRoulette/SRTPluginProviderRE8) repo.
4. Pull the [SRTHost](https://github.com/Squirrelies/SRTHost) repo.
5. Make sure all these 4 projects are under the same folder. Open `RE8HPViwer.sln` in Visual Studio.
6. Build Solution.

