param([String]$nugetRepo, [String]$apiKey)

Get-Module | % { if($_.Name -eq "NuGetRemi") { Remove-Module NuGetReMi}}
Import-Module .\NuGetReMi.psm1 -disablenamechecking
Get-Module | % { if($_.Name -eq "GitHelper") { Remove-Module GitHelper}}
Import-Module .\GitHelper.psm1 -disablenamechecking

$nuspecPath = $(Join-Path $(Get-Location) ..\Plugins\QaStats\.build)
$nuspecName = "ReMi.Plugin.QaStats.nuspec"
$nugetConfig = $(Join-Path $(Get-Location) ..\.nuget\NuGet.config)
$outputFolder = $(Join-Path $nuspecPath "lib\\net45")
$matchPattern = ("ReMi.Plugin.QaStats.dll")

$version = GetNextVersion $nuspecName $nuspecPath $nugetRepo

ChangeVersionInAssemblies $version $(Join-Path $nuspecPath ..\)

PublishToFileSystem $(Join-Path $nuspecPath ..\ReMi.Plugin.QaStats\ReMi.Plugin.QaStats.csproj) $outputFolder

CleanupPackageContent $outputFolder $matchPattern

GenerateNuGetPackage $nuspecName $nuspecPath $version

PublishPackage $nugetRepo $nuspecName $nuspecPath $nugetConfig $apiKey

RemoveNuGetPackages $nuspecPath

$project = $nuspecName.Substring(0, $nuspecName.LastIndexOf(".nuspec"))
CommitPackage $project $version
