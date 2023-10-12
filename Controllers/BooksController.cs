using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bookstoreApp.Api.Data;
using System.Collections;
using AutoMapper.QueryableExtensions;

namespace bookstoreApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper mapper;

        public BooksController(BookStoreDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetBooks()
        {
            // Esegue una query asincrona nel database per ottenere una lista di libri
            // Include anche le informazioni sugli autori dei libri nella query
            var bookDtos = await _context.Books.Include(q => q.Author)
                // Proietta i risultati nella classe BookReadOnlyDto utilizzando AutoMapper e ProjectTo
                .ProjectTo<BookReadOnlyDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            // Verifica se il risultato della query è nullo (potrebbe essere inutile, verifica la logica)
            if (_context.Books == null)
            {
                // Se il risultato è nullo, restituisci una risposta HTTP "Not Found" (404)
                return NotFound();
            }

            // Se la query ha avuto successo, restituisci una risposta HTTP "OK" (200) con la lista di libri convertita
            return Ok(bookDtos);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailsDto>> GetBook(int id)
        {
            // Verifica se la collezione di libri (_context.Books) è nulla
            if (_context.Books == null)
            {
                // Se è nullo, restituisci una risposta HTTP "Not Found" (404)
                return NotFound();
            }

            // Esegue una query asincrona nel database per ottenere i dettagli di un libro specifico
            var bookDto = await _context.Books
                .Include(q => q.Author)
                .ProjectTo<BookDetailsDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(book => book.Id == id);

            // Verifica se il libro richiesto non è stato trovato
            if (bookDto == null)
            {
                // Se il libro non è stato trovato, restituisci una risposta HTTP "Not Found" (404)
                return NotFound();
            }

            // Se il libro è stato trovato, restituisci i dettagli del libro come BookDetailsDto
            return bookDto;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {
            if (id != bookDto.Id)
            {
                // Verifica se l'ID nel parametro della richiesta non corrisponde all'ID nel bookDto
                // Se non corrisponde, restituisci una risposta HTTP "Bad Request" (400)
                return BadRequest();
            }

            // Cerca il libro nel database in base all'ID fornito
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                // Se il libro non è stato trovato, restituisci una risposta HTTP "Not Found" (404)
                return NotFound();
            }

            // Effettua una mappatura tra i dati del bookDto e l'entità book
            mapper.Map(bookDto, book);

            // Imposta lo stato dell'entità book come "Modificato" in modo che Entity Framework segua le modifiche
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                // Salva le modifiche nel database in modo asincrono
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExists(id))
                {
                    // Verifica se il libro non esiste più (potrebbe essere stato eliminato da un'altra operazione)
                    // In tal caso, restituisci una risposta HTTP "Not Found" (404)
                    return NotFound();
                }
                else
                {
                    // Se il libro esiste ancora, ma si è verificata un'eccezione di concorrenza diversa,
                    // rilanciala per ulteriore gestione
                    throw;
                }
            }

            // Se tutto va bene, restituisci una risposta HTTP "No Content" (204)
            return NoContent();

        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookCreateDto>> PostBook(BookCreateDto bookDto)
        {
            if (_context.Books == null)
            {
                // Verifica se la collezione di libri (_context.Books) è nulla
                // Se è nullo, restituisci una risposta con il messaggio di errore specificato
                return Problem("Entity set 'BookStoreDbContext.Books' is null.");
            }

            // Esegui la mappatura dell'oggetto bookDto in un oggetto Book utilizzando AutoMapper
            var book = mapper.Map<Book>(bookDto);

            // Aggiungi il nuovo libro al contesto del database
            _context.Books.Add(book);

            // Salva le modifiche nel database in modo asincrono
            await _context.SaveChangesAsync();

            // Restituisci una risposta HTTP "Created" (201) con l'URL di reindirizzamento
            // all'azione "GetBook" e il nuovo ID del libro
            return CreatedAtAction("GetBook", new { id = book.Id }, book);

        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            // Verifica se la collezione di libri (_context.Books) è nulla
            if (_context.Books == null)
            {
                // Se è nullo, restituisci una risposta HTTP "Not Found" (404)
                return NotFound();
            }

            // Cerca il libro nel database in base all'ID fornito
            var book = await _context.Books.FindAsync(id);

            // Verifica se il libro non è stato trovato
            if (book == null)
            {
                // Se il libro non è stato trovato, restituisci una risposta HTTP "Not Found" (404)
                return NotFound();
            }

            // Rimuovi il libro dal contesto del database
            _context.Books.Remove(book);

            // Salva le modifiche nel database in modo asincrono
            await _context.SaveChangesAsync();

            // Restituisci una risposta HTTP "No Content" (204) per indicare che la risorsa è stata eliminata con successo
            return NoContent();
        }


        private async Task<bool> BookExists(int id)
        {
            // Utilizza il metodo AnyAsync per verificare se esiste almeno un libro con l'ID specificato
            return await _context.Books.AnyAsync(e => e.Id == id);
        }

    }
}
