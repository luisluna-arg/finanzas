using FinanceBack.Models;
using FinanceBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundMovementController : ControllerBase
    {
        private readonly FundMovementService _fundsService;

        public FundMovementController(FundMovementService fundsService) =>
            _fundsService = fundsService;

        [HttpGet]
        public async Task<List<FundMovement>> Get() => await _fundsService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<FundMovement>> Get(string id)
        {
            var book = await _fundsService.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public async Task<IActionResult> Post(FundMovement newBook)
        {
            await _fundsService.CreateAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, FundMovement updatedBook)
        {
            var book = await _fundsService.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            updatedBook.Id = book.Id;

            await _fundsService.UpdateAsync(id, updatedBook);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _fundsService.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            await _fundsService.RemoveAsync(id);

            return NoContent();
        }
    }
}

