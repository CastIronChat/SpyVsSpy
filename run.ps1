param(
    [Parameter(Mandatory=$true)]
    $Command
)

<#
To support both git-bash and Powershell
this bash script will invoke the Powershell script ./run.ps1
Prefer pwsh but fallback to legacy Windows Powershell

To run a subcommand, for example "format":
    ./run format

Subcommands are declared in the switch() statement below.
#>

Write-Host "Running command $Command..."
switch($Command) {
    "format" {
        dotnet tool run dotnet-format .\Assembly-CSharp.csproj
        dotnet tool run dotnet-format .\Assembly-CSharp-Editor.csproj
    }
    default {
        Write-Error "Unexpected command: $Command"
    }
}
