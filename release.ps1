Write-Output "release: Release started"

Push-Location $PSScriptRoot
try {

    $dbp = [Xml] (Get-Content .\Directory.Version.props)
    $versionPrefix = $dbp.Project.PropertyGroup.VersionPrefix

    Write-Output "release: Package version prefix is $versionPrefix"

    $branch = @{ $true = $env:CI_TARGET_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$NULL -ne $env:CI_TARGET_BRANCH];
    $revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:CI_BUILD_NUMBER, 10); $false = "local" }[$NULL -ne $env:CI_BUILD_NUMBER];
    $suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)) -replace '([^a-zA-Z0-9\-]*)', '')-$revision"}[$branch -eq "master" -and $revision -ne "local"]
    $commitHash = $(git rev-parse --short HEAD)
    $buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

    Write-Output "release: Package version suffix is $suffix"
    Write-Output "release: Build version suffix is $buildSuffix"

    if ($env:NUGET_API_KEY) {

        Write-Output "release: Publishing NuGet packages"

        foreach ($nupkg in Get-ChildItem artifacts/*.nupkg) {
            & dotnet nuget push -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json "$nupkg"
            if($LASTEXITCODE -ne 0) { throw "Publishing failed" }
        }

        if (!($suffix)) {
            Write-Output "release: Creating release for version $versionPrefix"

            Invoke-Expression "gh release create v$versionPrefix --title v$versionPrefix --generate-notes $(get-item ./artifacts/*.nupkg) $(get-item ./artifacts/*.snupkg)"
        }
    }
} finally {
    Pop-Location
}