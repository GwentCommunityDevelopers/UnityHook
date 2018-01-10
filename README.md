# GwentHook

This is a fork of UnityHook for Hearthstone, modified to hook into Gwent. This README contains details about the Gwent speciifc hook. See the original [README](https://github.com/HearthSim/UnityHook/blob/master/README.md) for more details about the UnityHook project.

# UnityHook

UnityHook is a simple platform for hooking managed function calls, targeting
specifically assemblies compiled for [Unity3d](http://unity3d.com/) games.

Installed hooks allow overriding hooked functions' return values, essentially
granting complete control over managed code execution in a targeted game.

## Hooker

**Hooker** is the project that actually injects code into original game assemblies/libraries (= .dll files).
It can be used to 'hook' and 'restore' assemblies. Run `hooker.exe help` for information about the options to pass.
To hook game-assemblies you need to tell it the location of the game folder and the path to a compiled **HookRegistry** binary.

## HookRegistry

**HookRegistry** is the project that contains code to be executed when a hooked method/function has been called
while the game is running. The project compiles to 1 binary file that must be passed to **Hooker**.
Currently implemented hooks are following:

- Gwent - *with dependency on Gwent game libraries*
    - LoadDefinitions - Called when the Gwent client is loading card data on start up. Our hook outputs the latest data_definitions.zip (raw card data) endpoint to a file.
- Hearthstone - *with dependency on HS game libraries*
    - HS hooks have been removed, see the [original repository](https://github.com/HearthSim/UnityHook) for HS hooks.
- General
    - Hooking into the Unity logger. **NOTE:** that as of a patch in September 2017, Gwent no longer outputs to the unity log file.

> **Hooker** will attempt to copy all referenced (by **HookRegistry**) library files to the library folder of the game. Make sure to validate all necessary library files are copied by inspecting the **Hooker** log output.

## Hooks file
The file which declares all methods, located inside the game libraries, to be hooked. `/Hooker/gwent_hooks`  is used in the next section's example.

> **NOTE:** The hooker will always hook all methods entered in the Hooks file, if found. 
Hooking a specific method when the **HooksRegistry** binary has no code to inject will have NO side effect on the game! The game will run a bit slower though..

## Build

Visual Studio 2017 has to be installed to build both projects. Required components are C# - and Unity development tools! Visual Studio 2017 Community edition is free to download and capable to perform the build.

1. Clone the repo;
2. Create a junction link between the solution folder and the game install path. See `/createJunction.bat`;
2. Open UnityHook solution file with Visual Studio;
4. Build project **Hooker**;
5. Build project **HookRegistry**;
6. If you are getting build errors, under the HookRegistry project, right click on `References -> Add Reference -> Browse -> Browse (bottom right) -> In your Gwent install directory select UnityEngine.dll, Assembly-CSharp.dll and Assembly-CSharp-firstpass.dll`. Those 3 should give all the dependencies for this project. Try building again.
6. All binary files can be found inside the `bin` folder of each project.

## Usage Example
> The example expects the gwent_hooks file to be used as of the latest commit, also the latest **HookRegistry** binary.

What you need

- The **PATH** to **Hooker** compiled binaries. Refered to as {HOOKERPATH};
- The compiled binary **FILE** from **HookRegistry**. Referred to as {REGISTRY};
- The **PATH** to the game installation folder. Referred to as {GAMEDIR};
- The path to a hooks **FILE**, example_hooks as mentioned above. Referred to as {HOOKS}.
    
Steps

1. Call Hooker.exe;
```
{HOOKERPATH}\Hooker.exe hook -d "{GAMEDIR}" -h "{HOOKS}" -l "{REGISTRY}"
```
2. Verify that that Hooker did not encounter a problem;
    - Check the log output;
    - Requested methods are hooked;
    - Game assemblies are duplicated next to the original as {name}.original.dll -> Backup;
    - Patched assemblies are written to the game directory as {name}.out.dll;
    - Patched assemblies replaced the original assemblies as {name}.dll;
    - **HookRegistry** assembly, **and referenced assemblies**, are copied next to the game assemblies;
3. Run the game until you get to the main menu -> Check your Gwent install directory for `gwent_data.txt`.

> To restore, run the command ```{HOOKERPATH}\Hooker.exe restore -d "{GAMEDIR}"```

## Remarks

* This project is intended to run within the context of the Unity Engine. If Unity Engine is *not* initialised when **HookRegistry** is initialised, then no hooks will run. Each method will perform as if unhooked when outside of the Unity Engine context.
