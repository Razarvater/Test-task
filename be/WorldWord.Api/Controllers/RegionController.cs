using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private static readonly ReadOnlyCollection<RegionDTO> _regions = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
                .Select(x => new RegionInfo(x.Name))
                .DistinctBy(x => x.Name)
                .Select(x => x.ToDTO())
                .OrderBy(x => x.EnglishName)
                .ToList().AsReadOnly();

        /// <summary>
        /// Gets the list of regions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/regions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RegionDTO>))]
        public IActionResult GetRegionsList()
        {
            return Ok(_regions);
        }
    }
}
