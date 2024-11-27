1. MVP Architecture:
Model: Handles core game logic (e.g., rules, game state, moves).
View: Manages UI, displays the board
Presenter: Acts as the bridge, processing input, validating moves, and updating the view.
This separation ensures modularity and testability.

2. Command Pattern for GameState:
Encapsulates each action (e.g., move, place) as a command object.

Enables features like replay, game saving, and debugging.

3. Factory and Dependency Injection:
Factory Pattern: Simplifies the creation of game objects (e.g., pieces).
Dependency Injection: Promotes loose coupling, making components (like uinavigation controller) easier to test and extend.

4. Core Game Logic Testing:
Unit Tests: Validate moves, rules, and state transitions.
Edge Case Testing: Test invalid moves and game-breaking scenarios.

