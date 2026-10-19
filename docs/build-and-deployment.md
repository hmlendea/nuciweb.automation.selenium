# Build and Deployment

## Purpose

Documents the build pipeline, deployment processes, and environment configuration for the NuciWeb.Automation.Selenium project.

## Build System

### Technology Stack

- **Build Tool:** .NET SDK (dotnet CLI)
- **Target Framework:** .NET 10.0
- **Project Type:** Class Library
- **Solution Format:** .slnx (Solution eXtension format)

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Build in Release configuration
dotnet build -c Release

# Clean and build
dotnet clean && dotnet build

# Build with specific framework
dotnet build -f net10.0
```

### Build Output

| Configuration | Output Path |
|---------------|-------------|
| Debug | `bin/Debug/net10.0/` |
| Release | `bin/Release/net10.0/` |

## Project File

The project is defined in `NuciWeb.Automation.Selenium/NuciWeb.Automation.Selenium.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="NuciWeb" Version="4.0.0" />
    <PackageReference Include="NuciWeb.Automation" Version="1.0.0" />
    <PackageReference Include="NuciWeb.HTTP" Version="1.5.0" />
    <PackageReference Include="Selenium.Support" Version="4.5.1" />
    <PackageReference Include="Selenium.WebDriver" Version="4.5.1" />
  </ItemGroup>
</Project>
```

## Solution File

The solution is defined in `NuciWeb.Automation.Selenium.slnx` using the .NET Solution Extension format.

## Deployment

### NuGet Package

The library is intended to be distributed as a NuGet package:

```bash
# Pack the project
dotnet pack -c Release

# Output: bin/Release/net10.0/NuciWeb.Automation.Selenium.1.0.0.nupkg
```

### Package Versioning

Versioning follows Semantic Versioning (SemVer):

- **Major:** Breaking changes to public API
- **Minor:** New features, backward-compatible
- **Patch:** Bug fixes, backward-compatible

### Publishing to NuGet

```bash
# Push to NuGet
dotnet nuget push bin/Release/net10.0/NuciWeb.Automation.Selenium.1.0.0.nupkg \
  --api-key <API_KEY> \
  --source https://api.nuget.org/v3/index.json
```

## Environments

### Development

- **Framework:** .NET 10.0 SDK
- **IDE:** Visual Studio Code or Visual Studio
- **Browser Drivers:** geckodriver or ChromeDriver installed locally
- **Debug Mode:** Enabled for interactive debugging

### CI/CD

The project uses GitHub Actions for continuous integration:

- **Build:** Compiles the project and runs tests
- **Test:** Executes unit and integration tests
- **Package:** Creates NuGet package
- **Release:** Publishes to NuGet on tagged commits

### Production

- **Framework:** .NET 10.0 runtime
- **Browser Drivers:** Installed on the target system
- **Headless Mode:** Enabled by default
- **Timeout:** Configured per use case

## CI/CD Pipeline

### GitHub Actions Workflow

The workflow is defined in `.github/workflows/dotnet.yml`:

```yaml
# Expected workflow structure
name: .NET Build and Test
on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal
```

### Build Matrix

| OS | .NET Version | Status |
|----|-------------|--------|
| Ubuntu 22.04 | .NET 10.0 | ✅ Supported |
| Windows Server 2022 | .NET 10.0 | ✅ Supported |
| macOS 14 | .NET 10.0 | ✅ Supported |

## Docker Deployment

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /src/bin/Release/net10.0 .
ENTRYPOINT ["dotnet", "NuciWeb.Automation.Selenium.dll"]
```

### Docker Build

```bash
docker build -t nuciweb-automation-selenium .
```

## Related Documentation

- [repository-structure.md](./repository-structure.md) - Source tree layout
- [configuration.md](./configuration.md) - Configuration documentation
- [quick-start.md](./quick-start.md) - Getting started guide
- [testing.md](./testing.md) - Test strategy and execution