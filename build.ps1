Write-Output "build: Build started"

Push-Location $PSScriptRoot
try {
    if(Test-Path .\artifacts) {
        Write-Output "build: Cleaning ./artifacts"
        Remove-Item ./artifacts -Force -Recurse
    }

    & dotnet restore --no-cache

    $dbp = [Xml] (Get-Content .\Directory.Version.props)
    $versionPrefix = $dbp.Project.PropertyGroup.VersionPrefix

    Write-Output "build: Package version prefix is $versionPrefix"

    $branch = @{ $true = $env:CI_TARGET_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$NULL -ne $env:CI_TARGET_BRANCH];
    $revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:CI_BUILD_NUMBER, 10); $false = "local" }[$NULL -ne $env:CI_BUILD_NUMBER];
    $suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)) -replace '([^a-zA-Z0-9\-]*)', '')-$revision"}[$branch -eq "master" -and $revision -ne "local"]
    $commitHash = $(git rev-parse --short HEAD)
    $buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

    Write-Output "build: Package version suffix is $suffix"
    Write-Output "build: Build version suffix is $buildSuffix"

    & dotnet build -c Release --version-suffix=$buildSuffix /p:ContinuousIntegrationBuild=true
    if($LASTEXITCODE -ne 0) { throw "Build failed" }

    # foreach ($src in Get-ChildItem src/*) {
    #     Push-Location $src



    #     if ($suffix) {
    #         & dotnet pack -c Release --no-build --no-restore  -o ../../artifacts --version-suffix=$suffix
    #     } else {
    #         & dotnet pack -c Release --no-build --no-restore  -o ../../artifacts
    #     }
    #     if($LASTEXITCODE -ne 0) { throw "Packaging failed" }

    #     Pop-Location
    # }

    # Test Project
    Write-Output "build: Testing project"

    & dotnet test -c Release --no-build --no-restore --settings .runsettings
    if($LASTEXITCODE -ne 0) { throw "Testing failed" }



    # Package Project
    Write-Output "build: Packaging project"

    if ($suffix) {
        & dotnet pack -c Release --no-build --no-restore  -o artifacts --version-suffix=$suffix
    } else {
        & dotnet pack -c Release --no-build --no-restore  -o artifacts
    }
    if($LASTEXITCODE -ne 0) { throw "Packaging failed" }
    

    if ($env:NUGET_API_KEY) {

        Write-Output "build: Publishing NuGet packages"

        foreach ($nupkg in Get-ChildItem artifacts/*.nupkg) {
            & dotnet nuget push -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json "$nupkg"
            if($LASTEXITCODE -ne 0) { throw "Publishing failed" }
        }

        if (!($suffix)) {
            Write-Output "build: Creating release for version $versionPrefix"

            Invoke-Expression "gh release create v$versionPrefix --title v$versionPrefix --generate-notes $(get-item ./artifacts/*.nupkg) $(get-item ./artifacts/*.snupkg)"
        }
    }
} finally {
    Pop-Location
}