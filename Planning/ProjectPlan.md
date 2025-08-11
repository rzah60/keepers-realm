# Keeper's Realm - Project Plan & Rules

## My Alias
- When I type 'Status', you will respond with this detailed project plan.
- When I type 'UpdateMd', you will generate the complete text for this file.

## New Session Workflow
1.  **The Plan**: Start the new chat by sending **only** the `ProjectPlan.md` file.
2.  **The Godot Backend**: In a second message, attach **all** up-to-date Godot script (`.cs`) and scene (`.tscn`) files.
3.  **The React Frontend**: In a third message, attach **only the essential source files** from the React project: the `package.json` file and all files from the `src` folder.

## External Knowledge
- If we are working on a specific feature, I may provide you with links to official documentation (e.g., Godot, C#, React) or pages from supplemental RPG sourcebooks (e.g., Malleus Monstrorum) to enhance your knowledge.

## Coding Style & Consistency
1.  C# attributes (`[Export]`, `[Signal]`, etc.) must be on the same line as the member they decorate.
2.  Private fields must use `_camelCase`.
3.  Methods, public properties, and signals must use `PascalCase`.
4.  When modifying existing methods, you must preserve the exact original variable and parameter names.
5.  Before referencing a method in your instructions, you must first verify it exists in the provided project files. If it does not exist, you must explicitly provide the full code for its creation.

---

## Detailed Project Status

### ✅ Phase 1: Main Menu & Campaign Management
* **Status:** Done

### ✅ Phase 2: The Core Campaign Tracker
* **Status:** Done

### ✅ Phase 3: In-App Entity Management
* **Status:** Done

### ✅ Phase 4: Backend API & Session Management
* **Status:** Done

### 🟡 Phase 5: React Player Frontend
* **Status:** In Progress
* **Details:**
    * ✅ Set up the React project with TypeScript.
    * ✅ Created the "Login" component to validate session codes via the API.
    * ✅ Established and manage the WebSocket connection.
    * ✅ Created a "Character Selection" component that displays the list of investigators.
    * ✅ Implemented logic to send the player's character choice to the server.
    * ✅ Created a "Character Sheet" component to display the detailed character data.
    * 🟡 Implemented state management (using React Hooks).
    * ⚫ Style all components for a clean and professional presentation using CSS.

### ⚫ Phase 6: Information Control & Player View (Real-time)
* **Status:** Not Started
* **Details:**
    * ⚫ Implement logic for the GM to "push" updates (e.g., HP changes, status effects) to specific players.
    * ⚫ The React frontend will listen for WebSocket messages and dynamically update the Character Sheet component.
    * ⚫ The server will only send "revealed" data to player clients based on GM controls.

### ⚫ Phase 7: Mobile-Friendly Player View
* **Status:** Not Started
* **Details:**
    * ⚫ Use responsive CSS techniques to ensure the React components are usable on mobile devices.
    * ⚫ Test and refine the layout for common phone and tablet screen sizes.

### ⚫ Phase 8: Map Creation & Linking
* **Status:** Not Started
* **Details:**
    * ⚫ Implement a TileMap-based map editor scene within the Godot application.
    * ⚫ Create a system for importing map sprites and tilesets.
    * ⚫ Associate saved maps with a specific "Location" entry within a Campaign.

### ⚫ Phase 9: Interactive Map Overlays
* **Status:** Not Started
* **Details:**
    * ⚫ Allow GMs to add dynamic elements (tokens, markers) to a map.
    * ⚫ Implement functionality for the GM to control the visibility and position of tokens.
    * ⚫ Save the state of these tokens as part of the Location data.
    * ⚫ (Advanced) Send map and token data to player clients for a shared tactical view.