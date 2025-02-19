# Intro
This is a clone of Oregon Trail written in C#. It ain't finished yet.

# Usage
There is no release build of this yet. This is still in the "Run it from within your IDE" phase.

1. Open the solution in VSCode.
2. In the Run and Debug panel, select the "DEBUG console" configuration
    OR, pick Blazor to run it in a web browser with a gui kinda
3. Run it

# Capabilities
* Customizable locations, inventory and more by editing the yaml files
* Unlimited party size
* Every loop simulates one day of travel
* Each day a random occurrence is selected and affects a party member
* Items can be purchased when stopped at a location
* Health decreases daily depending on pace and rations
* Game over when all party members run out of health

# Roadmap

## Original Features
* Rest command to increase health
* Change names of party members and party leader
* Weather system
* River crossing system
* GUI
* Hunting system
* NPC dialog
* Win conditions and Game Over conditions
* Profession selection screen
* Decide when to start journey
    * Implement individual professions
* Weight of wagon makes wagon travel slower
* Different areas of game make different hazards more likely

## New Features
* New feature: Multiple wagons in wagon group when there are too many party members to fit in a single wagon
* New feature: Blazor (web browser) implementation
* New feature: Avalonia (desktop and mobile app) implementation
* New feature: Twitch and Youtube integration (not to be implemented until game is more complete)
    * `!join` command to join the wagon party
    * `!join (wagon number)` join a specific wagon
    * spend channel points towards wagon upgrades
        (what upgrades?)
    * spend channel points to sabotage wagon
    * channel points/command to stop and rest
    * vote on decisions