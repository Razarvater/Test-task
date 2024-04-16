using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WorldWord.Api.Core;
using WorldWord.Api.Services;
using WorldWord.DTO;

namespace WorldWord.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("word")]
    [ApiController]
    public class WordController : Controller
    {
        private readonly WordService _wordService;

        public WordController(WordService wordService)
        {
            _wordService = wordService;
        }

        /// <summary>
        /// Adds new word
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddWordResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> AddWord([FromBody] AddWordDTO dto)
        {
            if (dto.Region == null)
                return BadRequest(ErrorMessages.InvalidRegion);

            AddWordResponseDTO? result = await _wordService.AddNewWordAsync(dto);

            if (result == null)
                return BadRequest(ErrorMessages.EmailAlreadyExists);

            return Ok(result);
        }

        /// <summary>
        /// Gets the most popular word of current UTC day in region
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/most-popular-word-by-region")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordGroupDTO))]
        public async Task<IActionResult> GetMostPopularWordAsync([FromBody] RegionInfo region)
        {
            return Ok(await _wordService.GetMostPopularWordAsync(region.Name));
        }

        /// <summary>
        /// Gets the most popular word of current UTC day in region
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/most-popular-word")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WordGroupDTO))]
        public async Task<IActionResult> GetMostPopularWordAsync()
        {
            return Ok(await _wordService.GetMostPopularWordAsync());
        }
    }
}
