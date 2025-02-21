# MistTrader

A browser extension and analysis tool for the Mistwood Exchange market. Track your inventory, transactions, and profile data to make better trading decisions.

## Features

- Track your market transactions in real-time
- Monitor your inventory changes
- View your profile statistics
- Export data for analysis
- Calculate profit/loss across trades
- Track market trends for items

## Installation

### Browser Extension

1. Clone this repository:
```bash
git clone https://github.com/Avabin/MistTrader.git
```

2. Navigate to your browser's extensions page:
   - Chrome: `chrome://extensions`
   - Edge: `edge://extensions`

3. Enable "Developer mode" (toggle in the top-right corner)

4. Click "Load unpacked" and select the `extension` folder from the cloned repository

5. The extension should now appear in your browser toolbar

6. Visit [Mistwood Exchange](https://mistwood.pl) and log in to start tracking your trades

### Analysis Tool (CLI)

1. Ensure you have .NET 9.0 SDK installed

2. Build the solution:
```bash
dotnet build
```

3. Run the CLI tool:
```bash
dotnet run --project src/Runners/MistTrader.Cli/MistTrader.Cli.csproj <path-to-export-file>
```

## Usage

### Browser Extension

1. Click the MistTrader icon in your browser toolbar to open the panel
2. The extension automatically tracks:
   - Market transactions
   - Inventory changes
   - Profile updates
3. Use the "Export" button to save your data as a ZIP file
4. Use collapsible sections to view different types of data

### Data Analysis

The exported ZIP file contains:
- `transactions.json`: Your market transaction history
- `inventory.json`: Current inventory state
- `profile.json`: Your profile data and statistics

Use the CLI tool to analyze the data:
```bash
dotnet run --project src/Runners/MistTrader.Cli/MistTrader.Cli.csproj exported-data.zip
```

## Development

### Prerequisites

- .NET 8.0 SDK
- Node.js (for extension development)
- Chrome or Edge browser

### Project Structure

- `extension/`: Browser extension source code
- `src/`: .NET solution
  - `Modules/MistTrader.DataParsers/`: Core data parsing functionality
  - `Runners/MistTraader.Cli/`: Command-line interface
- `tests/`: Unit tests

### Running Tests

```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to your branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to the Mistwood community
- Built using .NET 8.0 and Chrome Extensions API

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/Avabin/MistTrader/issues).
```

This README:
- Explains what the project does
- Provides clear installation instructions for both the extension and CLI tool
- Includes detailed usage instructions
- Has development setup information
- Includes project structure and testing instructions
- Provides contribution guidelines
- Has proper licensing and support information
- Uses proper markdown formatting with code blocks and sections
