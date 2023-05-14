Param(
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][string]$gitUser="SergKorol",
    [parameter(Mandatory=$false)][string]$repoName="eShopOnContainers",
    [parameter(Mandatory=$false)][string]$gitBranch="linux-latest",
    [parameter(Mandatory=$false)][string]$patToken
)

$gitContext = "https://github.com/$gitUser/$repoName"

$services = @( 
    @{ Name="eshopbasket"; Image="eshop20220130011824.azurecr.io/eshop/basket.api"; File="src/Services/Basket/Basket.API/Dockerfile" },
    @{ Name="eshopcatalog"; Image="eshop20220130011824.azurecr.io/eshop/catalog.api"; File="src/Services/Catalog/Catalog.API/Dockerfile" },
    @{ Name="eshopcoupon"; Image="eshop20220130011824.azurecr.io/eshop/coupon.api"; File="src/Services/Coupon/Coupon.API/Dockerfile" },
    @{ Name="eshopidentity"; Image="eshop20220130011824.azurecr.io/eshop/identity.api"; File="src/Services/Identity/Identity.API/Dockerfile" },
    @{ Name="eshopordering"; Image="eshop20220130011824.azurecr.io/eshop/ordering.api"; File="src/Services/Ordering/Ordering.API/Dockerfile" },
	@{ Name="eshoporderingbg"; Image="eshop20220130011824.azurecr.io/eshop/ordering.backgroundtasks"; File="src/Services/Ordering/Ordering.BackgroundTasks/Dockerfile" },
    @{ Name="eshopwebspa"; Image="eshop20220130011824.azurecr.io/eshop/webspa"; File="src/Web/WebSPA/Dockerfile" },
    @{ Name="eshopwebmvc"; Image="eshop20220130011824.azurecr.io/eshop/webmvc"; File="src/Web/WebMVC/Dockerfile" },
    @{ Name="eshopwebstatus"; Image="eshop20220130011824.azurecr.io/eshop/webstatus"; File="src/Web/WebStatus/Dockerfile" },
    @{ Name="eshoppayment"; Image="eshop20220130011824.azurecr.io/eshop/payment.api"; File="src/Services/Payment/Payment.API/Dockerfile" },
    @{ Name="eshopocelotapigw"; Image="eshop20220130011824.azurecr.io/eshop/ocelotapigw"; File="src/ApiGateways/ApiGw-Base/Dockerfile" },
    @{ Name="eshopmobileshoppingagg"; Image="eshop20220130011824.azurecr.io/eshop/mobileshoppingagg"; File="src/ApiGateways/Mobile.Bff.Shopping/aggregator/Dockerfile" },
    @{ Name="eshopwebshoppingagg"; Image="eshop20220130011824.azurecr.io/eshop/webshoppingagg"; File="src/ApiGateways/Web.Bff.Shopping/aggregator/Dockerfile" },
    @{ Name="eshoporderingsignalrhub"; Image="eshop20220130011824.azurecr.io/eshop/ordering.signalrhub"; File="src/Services/Ordering/Ordering.SignalrHub/Dockerfile" }
)

$services |% {
    $bname = $_.Name
    $bimg = $_.Image
    $bfile = $_.File
    Write-Host "Setting ACR build $bname (${bimg}:$gitBranch) context $gitContext"    
    az acr task create --registry eshop20220130011824 --name $bname --image ${bimg}:$gitBranch --context $gitContext --branch $gitBranch --git-access-token "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" --file $bfile
}


# az acr task create --registry "eshop20220130011824.azurecr" \
# --name "eshopbasket" \
# --image "eshop20220130011824.azurecr.io/eshop/basket.api" \
# --context "https://github.com/SergKorol/eShopOnContainers" \
# --branch "linux-latest" \
# --git-access-token "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" \
# -file "src/Services/Basket/Basket.API/Dockerfile" 





