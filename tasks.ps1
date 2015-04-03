properties {
	$configuration = "Release"
	$platform = "Any CPU"
	$folderPath = ".\"
	$cleanPackages = $false
	$oldEnvPath = ""
}

task default -depends CleanUpMsBuildPath

task CleanUpMsBuildPath -depends PublishPackage {
	if($oldEnvPath -ne "")
	{
		Write-Host "Reverting Path variable"
		$Env:Path = $oldEnvPath
	}
}

task Compile -depends UpdatePackages {
	$msbuild = Resolve-Path "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin\MSBuild.exe"
	$options = "/p:configuration=$configuration;platform=$platform;VisualStudioVersion=12.0"
	exec { & $msbuild .\Linq2Rest.sln $options }
	'Executed Compile!'
}

task UpdatePackages -depends Clean {
	$packageConfigs = Get-ChildItem -Path .\ -Include "packages.config" -Recurse
	foreach($config in $packageConfigs){
        Write-Host $config.DirectoryName
		.\.nuget\nuget.exe i $config.FullName -o packages -source https://nuget.org/api/v2/
	}
}

task RunTests -depends Compile {
	'Running Tests'
	.\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe .\Linq2Rest.Tests\bin\v4.0\$configuration\Linq2Rest.Tests.dll
	.\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe .\Linq2Rest.Tests\bin\v4.5\$configuration\Linq2Rest.Tests.dll
	.\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe .\Linq2Rest.Reactive.Tests\bin\v4.0\$configuration\Linq2Rest.Reactive.Tests.dll
	.\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe .\Linq2Rest.Reactive.Tests\bin\v4.5\$configuration\Linq2Rest.Reactive.Tests.dll
}

task PublishPackage -depends RunTests {
	.\.nuget\nuget.exe pack Linq2Rest.nuspec
	.\.nuget\nuget.exe pack Linq2Rest.Reactive.nuspec
}

task CheckMsBuildPath -depends CheckKey {
	$envPath = $Env:Path
	if($envPath.Contains("C:\Windows\Microsoft.NET\Framework\v4.0") -eq $false)
	{
		if(Test-Path "C:\Windows\Microsoft.NET\Framework\v4.0.30319")
		{
			$oldEnvPath = $envPath
			$Env:Path = $envPath + ";C:\Windows\Microsoft.NET\Framework\v4.0.30319"
		}
		else
		{
			throw "Could not determine path to MSBuild. Make sure you have .NET 4.0.30319 installed"
		}
	}
}

task CheckKey {
	if((Test-Path .\Linq2Rest.snk) -eq $false){
		sn -k Linq2Rest.snk
	}
}

task Clean -depends CheckMsBuildPath {
	Get-ChildItem $folderPath -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
	if($cleanPackages -eq $true){
		if(Test-Path "$folderPath\packages"){
			Get-ChildItem "$folderPath\packages" -Recurse | Where { $_.PSIsContainer } | foreach ($_) { Write-Host $_.fullname; remove-item $_.fullname -Force -Recurse }
		}
	}
	
	if(Test-Path "$folderPath\BuildOutput"){
		Get-ChildItem "$folderPath\BuildOutput" -Recurse | foreach ($_) { Write-Host $_.fullname; remove-item $_.fullname -Force -Recurse }
	}
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}