# Overpark'd

## Author(s)
- Jesse Arevalo Baez
- Christopher Uy
- Brendan Chen
- Lawrence Wang

## About
Overpark'd is a betting simulator game that leverages Chapman University's Parking Structure API. Players can track real-time parking structure vacancies and place bets on whether the number of available spots in a chosen structure will increase or decrease during a timed betting window.

## Project Use
### Prerequisite

- Unity Editor Version **2022.3.44f1**

### How to Run the Game

1. Create a build of this game on your preferred desktop platform using Unity Editor version **2022.3.44f1**.
2. Run the built executable.
3. Click the **'play'** button to start the game; a bet window timer will begin.
4. Click **'higher'** or **'lower'** to place bets on your prediction of the parking structure vacancy.
5. Wait until the bet window closes to see the result of your bet (win/loss).
6. *(OPTIONAL)* To speed up the waiting process, press **F3** to skip 30 seconds on the timer. You can also press **F2** to increase your amount of Sandie Coin (in-game currency).

## Design Patterns Used

### Singleton Pattern

**How:**  
Both the `CurrencyManager` and `ParkingApiRequestManager` classes are implemented as singletons. This ensures that only one instance of each manager exists throughout the game. For example, `CurrencyManager.Instance` provides global access to the currency system, while `ParkingApiRequestManager.Instance` serves as the single source of truth for parking data.

**Why:**  
The singleton pattern prevents duplication of critical managers and allows any part of the application to access shared resources easily. This is especially important for systems like currency and networking, where consistency and centralized control are required.

---

### Observer Pattern

**How:**  
`CurrencyManager` implements the `IParkingRequestObserver` interface and subscribes to updates from `ParkingApiRequestManager`. Whenever new parking data is fetched from the API, all registered observers (such as `CurrencyManager`) are notified via methods like `OnDataUpdate` and `OnParkingCapacityChanged`.

**Why:**  
The observer pattern decouples data acquisition from business logic. By using this pattern, the networking system can notify multiple game systems (UI, currency, etc.) of data changes without those systems needing to know how the data is fetched. This enhances modularity, reusability, and maintainability.

---

### Strategy Pattern

**How:**  
The currency display system uses the strategy pattern through the `ICurrencyDisplayStrategy` interface and its implementations (`DollarSignCurrencyDisplayStrategy`, `BasicCurrencyDisplayStrategy`, `NameCurrencyDisplayStrategy`). The `CurrencyManager` can switch between different display strategies at runtime.

Additionally, the betting system uses a form of the strategy pattern by providing multiple prediction strategies (`GuessHigher`, `GuessLower`, and `ParkPrediction`). Each method encapsulates a different betting logic flow, but all share a similar structure.

**Why:**  
The strategy pattern allows for flexible and interchangeable behaviors. For currency display, it enables the game to present currency in different formats without changing the core logic. For betting, it allows different prediction mechanisms to be implemented and extended easily, supporting future game features and variations.

---

### Summary Table

| Pattern     | Where Used                        | Purpose                                                                 |
|-------------|-----------------------------------|-------------------------------------------------------------------------|
| Singleton   | CurrencyManager, ParkingApiRequestManager | Ensure a single instance and global access                              |
| Observer    | CurrencyManager, ParkingApiRequestManager | Decouple data updates from business logic; notify subscribers of changes |
| Strategy    | Currency display, Betting logic   | Allow interchangeable behaviors for display and prediction logic        |