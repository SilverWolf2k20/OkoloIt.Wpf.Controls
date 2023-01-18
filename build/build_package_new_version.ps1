# Update version
$sourceDirectory = "..\src\OkoloIt.Utilities.Result";
$csProjfileName = $sourceDirectory + "\OkoloIt.Utilities.Result.csproj";

[xml]$projectContents = Get-Content -Path $csProjfileName;
$oldVersion = [Version]$projectContents.Project.PropertyGroup.Version;
$newVersion = [Version]::new($oldVersion.Major ,$oldVersion.Minor ,$oldVersion.Build + 1);

# Save new version
$projectContents.Project.PropertyGroup.Version = $newVersion.ToString();
$projectContents.Save($csProjfileName);

"Version number has been udated from " + $oldVersion + " to " + $newVersion;

# Clear Release folder
$packagePath = $sourceDirectory + "\bin\Release\*.nupkg";

Remove-Item -Path $packagePath;
dotnet clean $csProjfileName -c Release;

# Build Project
dotnet build $csProjfileName -c Release;
dotnet pack  $csProjfileName -c Release;