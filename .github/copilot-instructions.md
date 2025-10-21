# GitHub Copilot Instructions for "Enter Here" Mod Project

This file provides detailed instructions for using GitHub Copilot to assist with the development and enhancement of the "Enter Here" mod for RimWorld. The mod is implemented in C# and integrates with RimWorld's ecosystem through various features and methods. 

## Mod Overview and Purpose

**Mod Name:** Enter Here

**Purpose:**  
The "Enter Here" mod is designed to enhance the navigation of visitors and caravans on the map of RimWorld. It allows for a more orderly entrance by placing entry spots on the map edge, which guides caravans and visitors to enter the map as close to these spots as possible. This system can also be toggled to function as an exit, or both enter and exit, providing strategic control over map navigation.

## Key Features and Systems

- **Placement of Entry Spots:** Entry spots can be placed by the player in the Misc-menu at the map edge to control entrance and exit points for groups.
  
- **Toggling Functionality:** Spots can be toggled to function solely as entry, exit, or both, ensuring seamless integration for different scenarios, including traders, guests, player caravans, and vehicles.
  
- **Compatibility with Other Mods:** Compatible with other mods such as "Ask Before Enter" and "Hospitality." Also supports vehicle mechanics from the "Vehicle Framework." It supports various pawn types, and provides options to enable or disable entry spot usage for specific types, including raids.
  
- **Multilingual Support:** Portuguese translation is included, thanks to community contributions.

## Coding Patterns and Conventions

- Utilize **static classes** for utility methods and system-specific functionality like handling caravan entrances (e.g., `CaravanEnterMapUtility_GetEnterCell`).
  
- Implement **stateful behavior** using derived classes for managing different pawn arrivals and behaviors upon entering and exiting (e.g., `EnterHereMod`, `EnterSpot`).
  
- **Naming Conventions:** Use clear and descriptive method and class names, ensuring they reflect their purpose, such as `TryFindExitSpot`, `GenerateEntities`, and `DoAction`.

## XML Integration

XML is used for defining static data like menu entries and object definitions. Ensure XML compatibility by following RimWorld's schema when defining objects and behaviors in `.xml` files. This includes specifying classes and their properties for entry and exit spots.

## Harmony Patching

Harmony is used to patch RimWorld’s existing methods for deeper integration:

- Patch RimWorld methods to modify behavior for entrance and exit logic.
- Use targeted Harmony patches to inject custom logic into pawn workflows.
- Ensure Harmony patches are centralized for easy maintenance, such as the `EnterHere_HospitalityPatch`.

## Suggestions for Copilot

- **Code Completion:** Use Copilot to expedite method implementation by suggesting boilerplate code for new static utility methods and class constructors.
  
- **Logic Suggestions:** Employ Copilot for suggesting logic inside your patched methods, especially when altering default game behaviors for specific scenarios like custom raid entries.
  
- **Refactoring:** Utilize Copilot to identify optimal refactoring points for code efficiency and readability, especially when dealing with large and complex classes.
  
- **XML Templates:** Use Copilot to generate templates for XML definitions, ensuring consistency and adherence to RimWorld’s schema.

This detailed guide should equip you to leverage GitHub Copilot effectively in enhancing and troubleshooting the "Enter Here" mod, ensuring it integrates seamlessly with RimWorld’s mechanics.
