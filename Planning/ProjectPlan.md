# Keeper's Realm - Project Plan & Rules

## My Alias
- When I type 'Status', you will respond with this detailed project plan.

## Coding Style & Consistency
1. C# attributes ([Export], [Signal], etc.) must be on the same line as the member they decorate.
2. Private fields must use _camelCase.
3. Methods, public properties, and signals must use PascalCase.
4. When modifying existing methods, you must preserve the exact original variable and parameter names unless changing that name is the explicit goal of the instruction.
5. Before referencing a method in your instructions, you must first verify it exists in the provided project files. If it does not exist, you must explicitly provide the full code for its creation as the first step. You must not assume methods have been created in prior steps.

---

## Detailed Project Status

### ✅ Phase 1: Main Menu & Campaign Management
* **Status:** Done
* **Details:**
    * ✅ A functional `main_menu.tscn` scene exists.
    * ✅ "New Campaign" and "Load Campaign" buttons are fully implemented.
    * ✅ The `Campaign.cs` Resource class is defined and used as the central data container.
    * ✅ The system for saving and loading Campaign resources is in place and functional.
    * ✅ The "Return to Main Menu" button correctly uses a dirty flag to conditionally show a confirmation dialog.

### ✅ Phase 2: The Core Campaign Tracker
* **Status:** Done
* **Details:**
    * ✅ The tabbed interface (`TabContainer`) is implemented in `main.tscn`.
    * ✅ Scrollable `ItemList`s for Investigators and NPCs are implemented and correctly populated upon creation.
    * ✅ "Add New..." buttons successfully open the `CharacterForm` for entity creation.
    * ✅ Double-clicking an item in a list correctly opens the `CharacterSheetPopup` with detailed information.
    * ✅ The main UI correctly shows the campaign's name (with `*` for unsaved changes), a "Save" button, and the "Return to Main Menu" button.

### 🟡 Phase 3: In-App Entity Management
* **Status:** In Progress / Partially Implemented
* **Details:**
    * ✅ **Entity Creation:** The `CharacterForm` is functional for creating new Investigators and NPCs.
    * ⚫ **Data Display on Load:**
        * ⚫ Create a public method in `Main.cs` to populate the Investigator and NPC lists from the current campaign data.
        * ⚫ Modify `MainMenu.cs` to call this new method after successfully loading a campaign.
    * ⚫ **Entity Editing:**
        * ⚫ Add an "Edit" button to the `CharacterSheetPopup`.
        * ⚫ Create a method in `CharacterForm.cs` to populate its fields with an existing entity's data.
        * ⚫ Update the save logic in `CharacterForm.cs` to handle modifying an existing entity.
    * ⚫ **Entity Deletion:**
        * ⚫ Add "Delete" buttons to the Investigator and NPC tabs.
        * ⚫ Implement a confirmation dialog for deletion.
        * ⚫ Write the logic to remove the selected entity from the campaign data and update the UI.
    * ✅ **Data Persistence:** The Investigator and NPC lists are confirmed to save correctly as part of the `Campaign` resource file.

### ⚫ Phase 4: Networking and Session Management
* **Status:** Not Started
* **Details:**
    * ⚫ No networking components (server/client logic) have been added to the project.

### ⚫ Phase 5: Information Control & Player View
* **Status:** Not Started
* **Details:**
    * ⚫ No GM-side controls for revealing or hiding information exist.
    * ⚫ No player-specific view has been designed or implemented.

### ⚫ Phase 6: Map Creation & Linking
* **Status:** Not Started
* **Details:**
    * ⚫ No TileMap-based map editor scene has been created.
    * ⚫ A system for importing map sprites has not been implemented.
    * ⚫ The `Campaign` resource has no data structures for storing or linking to map files.

### ⚫ Phase 7: Interactive Map Overlays
* **Status:** Not Started
* **Details:**
    * ⚫ Logic for adding dynamic elements like tokens or markers to a map is not implemented.
    * ⚫ The data structure for saving the state of map overlays does not exist.