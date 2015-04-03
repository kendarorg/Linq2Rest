function UpdatePackages
{
	$packageConfigs = Get-ChildItem -Path .\ -Include "packages.config" -Recurse
	foreach($config in $packageConfigs){
		Write-Host $config.FullName
		.\.nuget\nuget.exe i $config.FullName -o .\packages -source https://nuget.org/api/v2/
	}
}

UpdatePackages