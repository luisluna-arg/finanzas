using FinanceBack.Models;
using FinanceBack.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FinanceBack.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BsonController<Service, Domain> : ControllerBase where Domain : BsonDomain where Service : IMongoDBAsyncService<Domain>
    {
        protected readonly Service _service;

        protected BsonController(Service service) =>
            _service = service;

        [HttpGet]
        public async Task<List<Domain>> All() => await _service.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Domain>> Get(string id)
        {
            var book = await _service.GetAsync(id);

            if (book is null) return NotFound();

            return book;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Domain newBook)
        {
            await _service.CreateAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Domain updatedBook)
        {
            var book = await _service.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            updatedBook.Id = book.Id;

            await _service.UpdateAsync(id, updatedBook);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _service.GetAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            await _service.RemoveAsync(id);

            return NoContent();
        }
    }
}
