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

function run() {
    switch($Command) {
        "format" {
            dotnet tool run dotnet-format .\Assembly-CSharp.csproj
            dotnet tool run dotnet-format .\Assembly-CSharp-Editor.csproj
        }
        "build-webgl" {
            # THIS SCRIPT IS NOT WORKING
            # I don't understand -batchmode so I don't know why it fails.
            # I'll come back to this.
            FindUnity
            # try to locate the Unity executable
            Start-Process -Wait -FilePath $Unity -ArgumentList @(
                '-projectPath', $PSScriptRoot, '-quit', '-batchmode', '-executeMethod', 'WebGLBuilder.build'
            )
        }
        "serve-webgl" {
            # Launch the browser and serve webgl build with a little webserver
            Start "http://127.0.0.1:4507"
            & "$PSScriptRoot/scripts/deno.exe" run --allow-net --allow-read https://deno.land/std/http/file_server.ts ./Builds/WebGL
        }
        "publish-webgl" {
            # TODO push to github pages
        }
        default {
            Write-Error "Unexpected command: $Command"
        }
    }
}

$Unity = $null
function FindUnity() {
    $candidates = @(
        "C:\Program Files\Unity\Hub\Editor\2019.2.19f1\Editor\Unity.exe",
        "<PATH TO OTHER UNITY INSTALLATION>"
    )
    $candidate = $candidates | ? {test-path $_} | select-object -first 1
    if($candidate -eq $null) {
        write-host 'Unable to locate Unity executable.  Add your install to the list of paths in "run.ps1" and try again.'
    }
    ([ref]$Unity).Value = $candidate
}

run
