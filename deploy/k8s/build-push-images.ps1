Param(
    [parameter(Mandatory=$true)][string]$registry,
    [parameter(Mandatory=$true)][string]$dockerUser,
    [parameter(Mandatory=$true)][string]$dockerPassword,
    [parameter(Mandatory=$true)][string]$imageTag,
    [parameter(Mandatory=$false)][bool]$buildImages=$false,
    [parameter(Mandatory=$false)][bool]$pushImages=$true,
    [parameter(Mandatory=$false)][string]$dockerOrg="eshop"
)


# Initialization

$useDockerHub = [string]::IsNullOrEmpty($registry)

# Check required commands (only if not in CI environment)

$requiredCommands = ("docker", "docker-compose")
foreach ($command in $requiredCommands) {
    if ((Get-Command $command -ErrorAction SilentlyContinue) -eq $null) {
        Write-Host "$command must be on path" -ForegroundColor Red
        exit
    }
}

# Get tag to use from current branch if no tag is passed
if ([string]::IsNullOrEmpty($imageTag)) {
    $imageTag = $(git rev-parse --abbrev-ref HEAD)
}
Write-Host "Docker image Tag: $imageTag" -ForegroundColor Yellow

# Build  docker images if needed
if ($buildImages) {
    Write-Host "Building Docker images tagged with '$imageTag'" -ForegroundColor Yellow
    $env:TAG=$imageTag
    docker-compose -f ../../src/docker-compose.yml build    
}

# Login to Docker registry

if (-not [string]::IsNullOrEmpty($dockerUser)) {
    $registryFDQN =  if (-not $useDockerHub) {$registry} else {"index.docker.io/v1/"}

    Write-Host "Logging in to $registryFDQN as user $dockerUser" -ForegroundColor Yellow
    if ($useDockerHub) {
        docker login -u $dockerUser -p $dockerPassword
    }
    else {
        docker login -u $dockerUser -p $dockerPassword $registryFDQN
    }

}

# Push images to Docker registry
if ($pushImages) {
    Write-Host "Pushing images to $registry/$dockerOrg..." -ForegroundColor Yellow
    $services = (
        "basket.api", 
        "catalog.api",
        "coupon.api", 
        "identity.api",
        "mobileshoppingagg", 
        "ordering.api", 
        "ordering.backgroundtasks",
        "ordering.signalrhub",
        "payment.api",
        "webhooks.api",
        "webhooks.client", 
        "webmvc",
        "webshoppingagg", 
        "webspa", 
        "webstatus")

    foreach ($service in $services) {
        Write-Host "Tag: $imageTag" -ForegroundColor Green
        Write-Host "Adding tag to $service" -ForegroundColor Yellow
        docker tag eshop/${service}:linux-latest $registry/eshop/${service}:linux-latest
        Write-Host "Start push the $service" -ForegroundColor Yellow
        docker push $registry/eshop/${service}:linux-latest
        # Write-Host "Start pull the $service" -ForegroundColor Yellow
        # docker pull $registry/eshop/${service}:$imageTag      
    }
}





