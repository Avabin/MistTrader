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

## GitHub Actions Workflow

The GitHub Actions workflow for this project is defined in the `.github/workflows/dotnet.yml` file. This workflow is configured to run on push, pull request, and manual dispatch events. It includes steps to checkout code, set up .NET, restore dependencies, build the project, run tests, and publish test results.

### Workflow Steps

1. **Checkout code**: This step uses the `actions/checkout@v2` action to checkout the code from the repository.
2. **Setup .NET**: This step uses the `actions/setup-dotnet@v1` action to set up a .NET environment with the specified version.
3. **Restore dependencies**: This step runs the `dotnet restore` command to restore the project's dependencies.
4. **Build**: This step runs the `dotnet build --no-restore` command to build the project.
5. **Run tests**: This step runs the `dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --logger "trx;LogFileName=TestResults/test_results.trx"` command to run the tests and collect code coverage data.
6. **Publish test results**: This step uses the `actions/upload-artifact@v2` action to publish the test results. The test results are generated in the `TestResults` directory and uploaded as an artifact.
7. **Test Reporter**: This step uses the `dorny/test-reporter@v1` action to report the test results in a more readable format.

### Example Workflow File

```yaml
name: .NET

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
  workflow_dispatch:

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v2

    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Run tests
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --logger "trx;LogFileName=TestResults/test_results.trx"

    - name: Publish test results
      uses: actions/upload-artifact@v2
      with:
        name: test-results
        path: TestResults

    - name: Test Reporter
      uses: dorny/test-reporter@v1
      with:
        name: test-results
        path: TestResults/test_results.trx
        reporter: dotnet-trx
```

### Viewing Test Results

The test results are displayed directly in the GitHub Actions interface. You can view the test results by navigating to the "Actions" tab in your GitHub repository and selecting the workflow run you are interested in. The test results are available as an artifact that you can download and view locally.
