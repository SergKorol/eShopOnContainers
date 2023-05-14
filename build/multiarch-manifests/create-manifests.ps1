Param(
    [parameter(Mandatory=$true)][string]$registry
)

if ([String]::IsNullOrEmpty($registry)) {
    Write-Host "Registry must be set to docker registry to use" -ForegroundColor Red
    exit 1 
}

Write-Host "This script creates the local manifests, for pushing the multi-arch manifests" -ForegroundColor Yellow
Write-Host "Tags used are linux-master, win-master, linux-dev, win-dev, linux-latest, win-latest" -ForegroundColor Yellow
Write-Host "Multiarch images tags will be master, dev, latest" -ForegroundColor Yellow


$services = "identity.api", "basket.api", "catalog.api", "coupon.api", "ordering.api", "ordering.backgroundtasks", "payment.api", "webhooks.api", "ocelotapigw", "mobileshoppingagg", "webshoppingagg", "ordering.signalrhub", "webstatus", "webspa", "webmvc", "webhooks.client"

foreach ($svc in $services) {
    Write-Host "Creating manifest for $svc and tags :latest, :master, and :dev" -ForegroundColor Red
    
    docker manifest create $registry/eshop/${svc}:latest $registry/eshop/${svc}:linux-latest $registry/eshop/${svc}:win-latest
    Write-Host "Pushing manifest for $svc and tags :latest, :master, and :dev"
    docker manifest push $registry/eshop/${svc}:latest
    
}
