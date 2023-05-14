using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Coupon.FunctionalTests
{
    public class CouponScenarios
        : CouponScenarioBase
    {
        [Theory]
        [InlineData("DISC-5")]
        [InlineData("DISC-10")]
        [InlineData("DISC-15")]
        [InlineData("DISC-20")]
        [InlineData("DISC-25")]
        [InlineData("DISC-30")]
        public async Task GetCouponByCode_ReturnOk(string code)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.GetCouponByCode(code));
                
                response.EnsureSuccessStatusCode();
            }
        }
    }
    
    
}


