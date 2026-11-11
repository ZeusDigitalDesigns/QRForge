<!-- 
  Section 1: The Header and Description. 
  Keep it punchy. 
-->
# QRForge

QRForge is a custom, cross-platform QR Code Generator built with C# and
.NET 10. Featuring a sleek industrial UI theme, this tool is designed for
developers and users who need to generate and log QR codes efficiently.

<!-- 
  Section 2: Features & Tech Stack. 
-->
## Features
- **Industrial UI:** A custom aesthetic built with Avalonia UI.
- **Database Logging:** SQL Server integration for tracking generations.
- **Cross-Platform:** Tailored to run smoothly on macOS and Windows.

## Tech Stack
- C# / .NET 10
- Avalonia UI
- Entity Framework Core & SQL Server (hosted via OrbStack)
- JetBrains Rider

<!-- 
  Section 3: Getting Started. 
-->
## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- JetBrains Rider (Recommended)
- [OrbStack](https://orbstack.dev/) (For running SQL Server containers)

### Installation
1. Clone the repository:
   `git clone https://github.com/ZeusDigitalDesigns/QRForge.git`
2. Open the solution in JetBrains Rider.
3. Ensure your SQL Server container is running via OrbStack.
4. Build and run the `QRForge.UI` project.