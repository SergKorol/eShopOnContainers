param(
    [parameter(Mandatory=$true)][string]$name,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][bool]$createAcr=$true,
    [parameter(Mandatory=$false)][int]$nodeCount=1,
    [parameter(Mandatory=$false)][string]$nodeVMSize="Standard_D2_v5",
    [parameter(Mandatory=$false)][ValidateSet("VirtualMachineScaleSets","AvailabilitySet",IgnoreCase=$true)]$vmSetType="VirtualMachineScaleSets",
    [parameter(Mandatory=$false)][bool]$enableHttpApplicationAddon=$true,
    [parameter(Mandatory=$false)][bool]$enableAzureMonitoring=$false
)


$id=20220130011824


$resourceGroupName="$name-rg"
$acrName="$name$id"
$aksName="$name-aks"


# Create resource group
Write-Host "Creating Azure Resource Group..." -ForegroundColor Yellow
az group create --name=$resourceGroupName --location=$location

if ($createAcr -eq $true) {
    Write-Host "Creating Azure Container Registry named $acrName" -ForegroundColor Yellow
    az acr create -n $acrName -g $resourceGroupName -l $location  --admin-enabled true --sku Basic
}

#login to acr
az acr login --name $acrName


# Create kubernetes cluster in AKS
Write-Host "Creating AKS $resourceGroupName/$aksName" -ForegroundColor Yellow
az aks create --resource-group=$resourceGroupName --name=$aksName --generate-ssh-keys --node-count=$nodeCount --node-vm-size=$nodeVMSize --vm-set-type $vmSetType

if ($enableHttpApplicationAddon) {
    Write-Host "Enabling Http Application Routing in AKS $serviceName" -ForegroundColor Yellow
    az aks enable-addons --resource-group $resourceGroupName --name $aksName --addons http_application_routing --subnet-name "tshape"
}

if ($enableAzureMonitoring) {
    Write-Host "Enabling Azure Monitoring in AKS $serviceName" -ForegroundColor Yellow
    az aks enable-addons --resource-group $resourceGroupName --name $aksName --addons monitoring --subnet-name "tshape"
}

# Retrieve kubernetes cluster configuration and save it under ~/.kube/config
Write-Host "Getting Kubernetes config..." -ForegroundColor Yellow
az aks get-credentials --resource-group=$resourceGroupName --name=$aksName

if ($createAcr -eq $true) {
    # Show ACR credentials
    Write-Host "ACR $registryName credentials:" -ForegroundColor Yellow
    az acr credential show -n $acrName
}

