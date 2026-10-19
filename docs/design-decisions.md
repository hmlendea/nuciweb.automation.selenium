# Design Decisions

## Purpose

Documents the key architectural and design decisions made in the NuciWeb.Automation.Selenium project, along with their rationale.

## Decision 1: IWebProcessor Abstraction

**Decision:** Implement `IWebProcessor` from `NuciWeb.Automation` using Selenium WebDriver.

**Rationale:**
- Provides a clean abstraction over Selenium-specific details
- Allows swapping implementations (e.g., Playwright, Puzzle) without changing application code
- Follows the dependency inversion principle
- Enables testing with mock implementations
- Aligns with the NuciWeb framework's design philosophy

**Trade-offs:**
- Additional layer of indirection
- Some functionality duplicated between interface and implementation
- Requires maintaining both interface and implementation

**Alternatives Considered:**
- Direct Selenium usage (rejected - too tightly coupled)
- Extension methods on WebDriver (rejected - not clean abstraction)
- Dynamic dispatch (rejected - performance overhead)

## Decision 2: Static WebDriverInitialiser

**Decision:** Use a static utility class for WebDriver creation.

**Rationale:**
- No instance state is required
- Driver creation is a stateless operation
- Provides simple, direct access without instantiation overhead
- Centralises driver configuration logic
- Avoids unnecessary object allocation

**Trade-offs:**
- Cannot be mocked for testing
- No configuration persistence
- Less flexible than instance-based factory

**Alternatives Considered:**
- Instance factory pattern (rejected - unnecessary complexity)
- Dependency injection container (rejected - overkill for this use case)

## Decision 3: Firefox Preference

**Decision:** Prioritise Firefox when both drivers are available.

**Rationale:**
- Firefox is open-source and widely available
- Lower resource consumption than Chrome
- Better geodating support
- More stable driver compatibility

**Trade-offs:**
- Chrome users may experience fallback
- Firefox may have different rendering behaviour

**Alternatives Considered:**
- Always prefer Chrome (rejected - Firefox preferred by default)
- User choice based on environment (rejected - too complex)

## Decision 4: XPath-Based Element Location

**Decision:** Use XPath expressions for element location.

**Rationale:**
- XPath is flexible and powerful
- Supports complex queries (attributes, text, hierarchy)
- Standard Selenium approach
- Well-documented and widely understood

**Trade-offs:**
- Performance may be slower than CSS selectors
- XPath syntax can be verbose
- Not all elements are easily locatable via XPath

**Alternatives Considered:**
- CSS selectors (rejected - less flexibility)
- ID/name/class selectors (rejected - limited to simple cases)
- JavaScript execution (rejected - bypasses abstraction)

## Decision 5: Synchronous Operations

**Decision:** All operations are synchronous.

**Rationale:**
- Simpler API surface
- Easier to reason about
- No async/await complexity
- Selenium's core operations are synchronous

**Trade-offs:**
- Blocks calling thread
- May cause deadlocks in async contexts
- Limits concurrent usage

**Alternatives Considered:**
- Async/await patterns (rejected - adds complexity without benefit)
- Task-based operations (rejected - unnecessary abstraction)

## Decision 6: Fixed Window Size

**Decision:** Default window size is 1920x1080.

**Rationale:**
- Common desktop resolution
- Ensures consistent layout across environments
- No need for dynamic sizing

**Trade-offs:**
- May not suit mobile or small screens
- Fixed size cannot adapt to different displays

**Alternatives Considered:**
- Configurable window size (rejected - adds complexity)
- Auto-detect screen size (rejected - unreliable)

## Decision 7: Debug Mode Disables Headless

**Decision:** When `isDebugModeEnabled = true`, headless mode is disabled.

**Rationale:**
- Debug mode implies interactive debugging
- Headless mode hides browser windows
- Debugging requires visible browser interaction
- Simplifies troubleshooting

**Trade-offs:**
- Higher resource consumption in debug mode
- Slower execution in debug mode

**Alternatives Considered:**
- Separate debug flag for headless (rejected - conflates concerns)
- Always use headless (rejected - defeats debug purpose)

## Decision 8: IEnumerable<string> Return Types

**Decision:** Methods returning multiple values return `IEnumerable<string>`.

**Rationale:**
- Supports any number of matching elements
- Lazy evaluation (elements fetched on demand)
- Consistent with Selenium's `FindElements` pattern
- Memory efficient for large result sets

**Trade-offs:**
- Caller must enumerate results
- No built-in count (separate method provided)
- No early termination support

**Alternatives Considered:**
- `List<string>` (rejected - eager evaluation, more memory)
- `IReadOnlyCollection<string>` (rejected - same as IEnumerable)
- Array (rejected - fixed size, less flexible)

## Decision 9: User Agent Fetching

**Decision:** Fetch user agent string via NuciWeb.HTTP service.

**Rationale:**
- Provides realistic browser identification
- Avoids hardcoded user agents
- Uses maintained service endpoint
- Supports latest browser versions

**Trade-offs:**
- Network dependency
- Potential service unavailability
- Adds latency to driver creation

**Alternatives Considered:**
- Hardcoded user agent (rejected - outdated)
- System property (rejected - not always available)
- Browser detection (rejected - less accurate)

## Decision 10: No Implicit Waits

**Decision:** No implicit waits configured.

**Rationale:**
- Explicit waits preferred for reliability
- Implicit waits can cause unpredictable timing
- Reduces confusion between implicit and explicit waits
- Forces callers to handle timing explicitly

**Trade-offs:**
- Elements may fail immediately if not found
- Requires caller to implement waits
- More verbose code

**Alternatives Considered:**
- Small implicit wait (rejected - still unpredictable)
- Default timeout (rejected - changes semantics)

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [api-reference/selenium-web-processor.md](./api-reference/selenium-web-processor.md) - Processor API
- [api-reference/web-driver-initialiser.md](./api-reference/web-driver-initialiser.md) - Initialiser API
- [design-decisions.md](./design-decisions.md) - This document
- [invariants.md](./invariants.md) - System invariants
- [error-handling.md](./error-handling.md) - Error handling patterns