using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WorldWord.Api.Converters;
using WorldWord.DTO;

namespace WorldWord.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("region")]
    [ApiController]
    public class RegionController : Controller
    {
        /// <summary>
        /// Gets the list of regions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/regions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RegionDTO>))]
        public IActionResult GetRegionsList()
        {
            List<RegionDTO> regions = new List<RegionDTO>();

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            foreach (var culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.Name);
                if (!regions.Any(x => x.EnglishName == region.EnglishName))
                    regions.Add(region.ToDTO());
            }
            return Ok(regions.OrderBy(x => x.EnglishName));
        }
    }
}
