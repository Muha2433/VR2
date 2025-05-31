using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VR2.DTOqMoels;
using VR2.Services;

namespace VR2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private SaleService _saleService;
        public HomeController(SaleService saleService) 
        {
        _saleService = saleService;
        } 

        [HttpGet("HomePage")]
        public async Task<IActionResult> MyHomePage()
        {
            List<dtoPropertyReuestSale> reqPropertylsit =await _saleService.RequestPropertySalellist();
            if (reqPropertylsit==null | !reqPropertylsit.Any())
                return BadRequest(new { Message = "no " });

            var homePage = new MyHomeClass();
            homePage.lstRequestSaleProperty=reqPropertylsit;
            return Ok(homePage);
          
        }


        [HttpGet("requestSaleDetail")]
        public async Task<IActionResult> RequestSaleDetail(int id)
        {
            var (success, message, data) = await _saleService.RequestSaleDetail(id);

            if (!success)
                return NotFound(new { success = false, message });

            return Ok(new { success = true, message, result = data });
        }

        //[HttpGet("requestSaleDetail")]
        //public async Task<IActionResult> requestSaleDetail()
        //{
        //    //
        //}

        //[HttpPost("RequestIndividualPurchase")]
        //public async Task<IActionResult> RequestIndividualPurchase([FromBody] int RequestSaleId)
        //{

        //}

        public class MyHomeClass
        {
            public MyHomeClass() 
            {
            
            }
            public ICollection<dtoPropertyReuestSale> lstRequestSaleProperty { get; set; }
        }

    }
}
