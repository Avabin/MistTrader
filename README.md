# MistTrader

## GitHub Actions Workflow

The GitHub Actions workflow for this project is defined in the `.github/workflows/dotnet.yml` file. This workflow is configured to run on push, pull request, and manual dispatch events. It includes steps to checkout code, set up .NET, restore dependencies, build the project, run tests, and publish test results.

### Workflow Steps

1. **Checkout code**: This step uses the `actions/checkout@v2` action to checkout the code from the repository.
2. **Setup .NET**: This step uses the `actions/setup-dotnet@v1` action to set up a .NET environment with the specified version.
3. **Restore dependencies**: This step runs the `dotnet restore` command to restore the project's dependencies.
4. **Build**: This step runs the `dotnet build --no-restore` command to build the project.
5. **Run tests**: This step runs the `dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --logger "trx;LogFileName=TestResults/test_results.trx"` command to run the tests and collect code coverage data.
6. **Publish test results**: This step uses the `actions/upload-artifact@v3` action to publish the test results. The test results are generated in the `TestResults` directory and uploaded as an artifact.

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
      uses: actions/upload-artifact@v3
      with:
        name: test-results
        path: TestResults
```

### Viewing Test Results

The test results are displayed directly in the GitHub Actions interface. You can view the test results by navigating to the "Actions" tab in your GitHub repository and selecting the workflow run you are interested in. The test results are available as an artifact that you can download and view locally.
