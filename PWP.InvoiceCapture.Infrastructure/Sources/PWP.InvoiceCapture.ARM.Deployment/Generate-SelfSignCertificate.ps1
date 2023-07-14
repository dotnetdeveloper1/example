#Requires -Module Az.KeyVault

# Use this script to create a certificate that you can use to secure a Service Fabric Cluster
# This script requires an existing KeyVault that is EnabledForDeployment.  The vault must be in the same region as the cluster.
# To create a new vault and set the EnabledForDeployment property run:
#
# New-AzResourceGroup -Name KeyVaults -Location WestUS
# New-AzKeyVault -VaultName $KeyVaultName -ResourceGroupName KeyVaults -Location WestUS -EnabledForDeployment
#
# Once the certificate is created and stored in the vault, the script will provide the parameter values needed for template deployment
# 

param(
  [string] [Parameter(Mandatory = $true)] $CertificateName,
  [string] [Parameter(Mandatory = $true)] $CertificatePassword,
  [string] [Parameter(Mandatory = $true)] $CertificateDnsName,
  [string] [Parameter(Mandatory = $true)] $ClientCertificateName,
  [string] [Parameter(Mandatory = $true)] $ClientCertificatePassword,
  [string] [Parameter(Mandatory = $true)] $KeyVaultName,
  [string] [Parameter(Mandatory = $true)] $KeyVaultSecretName,

  [string] [Parameter(Mandatory = $true)] $ApplicationGatewayCertificateName,
  [string] [Parameter(Mandatory = $true)] $ApplicationGatewayCertificatePassword,
  [string] [Parameter(Mandatory = $true)] $ApplicationGatewayCertificateDnsName,

  [string] [Parameter(Mandatory = $true)] $PipelineSourceVaultValueVariableName,
  [string] [Parameter(Mandatory = $true)] $PipelineCertificateUrlValueVariableName,
  [string] [Parameter(Mandatory = $true)] $PipelineApplicationGatewayCertificateUrlValueVariableName,
  [string] [Parameter(Mandatory = $true)] $PipelineCertificateThumbprintVariableName,
  [string] [Parameter(Mandatory = $true)] $PipelineClientCertificateThumbprintVariableName
)

Write-Host "Trying to get existing certificate.."
$Certificate = Get-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $CertificateName -Verbose -ErrorAction Ignore

if (-not $Certificate)
{
  Write-Host "Creating new certificate for the cluster.."

  $SecurePassword = ConvertTo-SecureString -String $CertificatePassword -AsPlainText -Force
  $Certificate = New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName $CertificateDnsName 
  $CertFileFullPath = $(Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Definition) "\$CertificateName.pfx")
  Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecurePassword -Cert $Certificate

  Import-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $CertificateName -FilePath $CertFileFullPath -Password $SecurePassword -Verbose

  $Bytes = [System.IO.File]::ReadAllBytes($CertFileFullPath)
  $Base64 = [System.Convert]::ToBase64String($Bytes)

  $JSONBlob = @{
    data     = $Base64
    dataType = 'pfx'
    password = $CertificatePassword
  } | ConvertTo-Json

  $ContentBytes = [System.Text.Encoding]::UTF8.GetBytes($JSONBlob)
  $Content = [System.Convert]::ToBase64String($ContentBytes)

  $SecretValue = ConvertTo-SecureString -String $Content -AsPlainText -Force
  $KeyVaultSecret = Set-AzKeyVaultSecret -VaultName $KeyVaultName -Name $KeyVaultSecretName -SecretValue $SecretValue -Verbose

  Write-Host "New certificate created."
}

Write-Host "Trying to get existing client certificate.."
$ClientCertificate = Get-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $ClientCertificateName -Verbose -ErrorAction Ignore

if (-not $ClientCertificate)
{
  Write-Host "Creating new client certificate for the cluster.."

  $SecureClientPassword = ConvertTo-SecureString -String $ClientCertificatePassword -AsPlainText -Force
  $ClientCertificate = New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName $CertificateDnsName 
  $CertFileFullPath = $(Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Definition) "\$ClientCertificateName.pfx")
  Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecureClientPassword -Cert $ClientCertificate

  Import-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $ClientCertificateName -FilePath $CertFileFullPath -Password $SecureClientPassword -Verbose

  Write-Host "New client certificate created."
}

Write-Host "Trying to get existing Application Gateway certificate.."
$ApplicationGatewayCertificate = Get-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $ApplicationGatewayCertificateName -Verbose -ErrorAction Ignore

if (-not $ApplicationGatewayCertificate)
{
  Write-Host "Creating new Application Gateway certificate for the cluster.."

  $SecureApplicationGatewayPassword = ConvertTo-SecureString -String $ApplicationGatewayCertificatePassword -AsPlainText -Force
  $ApplicationGatewayCertificate = New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName $ApplicationGatewayCertificateDnsName -Subject "CN=$ApplicationGatewayCertificateDnsName"
  $CertFileFullPath = $(Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Definition) "\$ApplicationGatewayCertificateName.pfx")
  Export-PfxCertificate -FilePath $CertFileFullPath -Password $SecureApplicationGatewayPassword -Cert $ApplicationGatewayCertificate

  Import-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $ApplicationGatewayCertificateName -FilePath $CertFileFullPath -Password $SecureApplicationGatewayPassword -Verbose
  $ApplicationGatewayCertificate = Get-AzKeyVaultCertificate -VaultName $KeyVaultName -Name $ApplicationGatewayCertificateName -Verbose -ErrorAction Ignore

  Write-Host "New Application Gateway certificate created."
}

Write-Host "##vso[task.setvariable variable=$PipelineSourceVaultValueVariableName]$($(Get-AzKeyVault -VaultName $KeyVaultName).ResourceId)"
Write-Host "##vso[task.setvariable variable=$PipelineCertificateUrlValueVariableName]$($KeyVaultSecret.Id)"
Write-Host "##vso[task.setvariable variable=$PipelineApplicationGatewayCertificateUrlValueVariableName]$($ApplicationGatewayCertificate.SecretId)"
Write-Host "##vso[task.setvariable variable=$PipelineCertificateThumbprintVariableName]$($Certificate.Thumbprint)"
Write-Host "##vso[task.setvariable variable=$PipelineClientCertificateThumbprintVariableName]$($ClientCertificate.Thumbprint)"
