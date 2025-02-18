# MistTrader

## Overview

MistTrader is a project that provides tools for parsing and analyzing transaction data from the Mistwood Exchange. The repository contains multiple modules and projects, each with its own functionality.

### Main Modules

- **MistTrader.DataParsers**: This module contains various parsers for processing transaction data. It includes high-performance parsers, reactive parsers, and optimized exchange parsers.
- **MistTrader.Cli**: This module provides a command-line interface for running the parsers and analyzing transaction data.

## Building the Project

To build the project, you need to have the .NET SDK installed. You can download it from the [official .NET website](https://dotnet.microsoft.com/download).

1. Clone the repository:
   ```sh
   git clone https://github.com/Avabin/MistTrader.git
   cd MistTrader
   ```

2. Restore dependencies:
   ```sh
   dotnet restore
   ```

3. Build the project:
   ```sh
   dotnet build
   ```

## Running the Project

To run the project, use the following command:

```sh
dotnet run --project src/Runners/MistTrader.Cli -- <path-to-json-file>
```

Replace `<path-to-json-file>` with the path to the JSON file containing the transaction data.

## Testing the Project

To run the tests, use the following command:

```sh
dotnet test
```

This will execute all the tests in the repository and display the results.

## Dependencies

The project has the following dependencies:

- .NET SDK 9.0 or later
- System.Reactive (version 6.0.1)
- ConsoleAppFramework (version 5.4.0)
- Microsoft.Extensions.Logging.Console (version 9.0.2)
- FluentAssertions (for testing)
- NUnit (for testing)


### Viewing Test Results

The test results are displayed directly in the GitHub Actions interface. You can view the test results by navigating to the "Actions" tab in your GitHub repository and selecting the workflow run you are interested in. The test results are available as an artifact that you can download and view locally.
