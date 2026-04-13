Run the odc-flight-recorder agent to add telemetry and structured logging to an existing OutSystems ODC External Library extension.

The agent will:
1. Read the existing project structure (interface, implementation, .csproj, tests)
2. Identify which actions to instrument and confirm the plan with you
3. Add the `ODCFlightRecorder.SDK` NuGet dependency
4. Add `flightPath` (structured JSON telemetry) and `goldenThreadId` (distributed trace ID) output parameters to each action
5. Wrap action bodies with `FlightRecorder` — capturing key checkpoints via `AddStep()` and finalizing via `FinalizeBatchAsJson()`
6. Update unit tests to assert on telemetry output
7. Build with `dotnet build` and verify with `dotnet test`
8. Provide the ODC entity schema and wiring instructions for storing telemetry in ODC Studio

Pass the path to the project folder, or a description of the actions you want instrumented.

$ARGUMENTS
