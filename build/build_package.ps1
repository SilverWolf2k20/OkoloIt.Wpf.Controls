# Clear Release folder
$sourceDirectory = "..\src";
$csProjfileName = $sourceDirectory + "\OkoloIt.Wpf.Controls.csproj";
$packagePath = $sourceDirectory + "\bin\Release\*.nupkg";

Remove-Item -Path $packagePath;
dotnet clean $csProjfileName -c Release;

# Build Project
dotnet build $csProjfileName -c Release;
dotnet pack  $csProjfileName -c Release;