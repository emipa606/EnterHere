# Copilot Instructions: Enter Here Mod

## Mod Overview and Purpose
**Enter Here** is a mod for RimWorld that enhances how visitors and caravans enter and exit the map. Inspired by the idea from annabell lee, this mod allows players to place designated entering and exiting spots on the map edges, facilitating a more controlled and organized flow of pawns. It is particularly effective as a companion to the "Ask Before Enter" mod, ensuring a seamless experience for managing external groups within the colony confines.

## Key Features and Systems
- **Enter and Exit Spots:** Introduces map edge spots that can be configured for entry, exit, or both.
- **Compatibility with Existing Game Mechanics:**
  - Supports both player and NPC caravans, including traders and visitors, particularly those enhanced by the Hospitality mod.
  - Works with the Vehicle Framework to manage vehicle caravans.
  - Optionally applicable to raids, providing both strategic advantages and potential risks.
- **Translation Support:** Incorporates Portuguese translations, thanks to community contributor DvdFalaschi.
- **Visual Enhancements:** Updated spot icons by Soul.

## Coding Patterns and Conventions
- Utilizes C# static and instance classes to encapsulate game logic.
- Follows a clear naming convention where class and method names are descriptive of their function (e.g., `CaravanEnterMapUtility_GetEnterCell`).
- Emphasis on modular design with specific dedicated classes for various game-related events, adhering to RimWorld's mod structure.
- Consistent use of access modifiers to maintain encapsulation and integrity of code components.

## XML Integration
- XML files handle Defs for enter and exit spot items, integrating seamlessly into RimWorld's database.
- XML-based options support customization for enabling/disabling the mod's features for different pawn types.
- Ensure XML files align with RimWorld's schema for consistent loading and gameplay integration.

## Harmony Patching
- Uses Harmony library to patch existing RimWorld methods:
  - Implements patches such as `EnterHere_HospitalityPatch` for seamless integration with Hospitality mod.
  - Common use of prefix and postfix patches to modify game behavior without altering original game files.
- Consider leveraging `HarmonyPatch` annotations for clarity and maintainability of code, ensuring patches are correctly defined and targeted.

## Suggestions for Copilot
- **Code Completion:** Assist with generating method headers and implementing typical patterns for Harmony patches.
- **Error Handling:** Provide suggestions for robust error handling strategies when integrating with game events.
- **Performance Optimization:** Offer optimized code snippets for frequently used operations in pathfinding and map cell management.
- **XML Handling:** Generate XML file templates for new Defs consistent with the mod's functionality.
- **Multilingual Support:** Assist in adding or updating translations within XML or C# code files.
- **Testing and Debugging:** Propose methodologies for testing patches and new features, helping maintain mod stability and performance.

By following these guidelines and utilizing Copilot effectively, developers can ensure that the Enter Here mod remains a polished and valuable addition to the RimWorld community.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
