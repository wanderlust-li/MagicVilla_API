using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers;

// [Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public VillaAPIController(ApplicationDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        return Ok(await _db.Villas.ToListAsync());
    }
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        if (id == 0)
            return BadRequest();

        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa == null)
            return BadRequest();
        return Ok(villa);
    }
    
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO villaDto)
    {
        // if (!ModelState.IsValid)
        //     return BadRequest(ModelState);
        if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already Exists!");
            return BadRequest(ModelState);
        }
        if (villaDto == null)
            return BadRequest();
        
        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };
        await _db.Villas.AddAsync(model);
        await _db.SaveChangesAsync();

        return CreatedAtRoute("GetVilla", new {id = model.Id},model);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0)
            return BadRequest();

        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa == null)
            return NotFound();
        
        _db.Villas.Remove(villa);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaDto)
    {
        if (villaDto == null || id != villaDto.Id)
            return BadRequest();
        
        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            Id = villaDto.Id,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();
        return NoContent();
    }
    
    
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
            return BadRequest();
        
        var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        VillaUpdateDTO villaDto = new VillaUpdateDTO()
        {
            Amenity = villa.Amenity,
            Details = villa.Details,
            Id = villa.Id,
            ImageUrl = villa.ImageUrl,
            Name = villa.Name,
            Occupancy = villa.Occupancy,
            Rate = villa.Rate,
            Sqft = villa.Sqft
        };
        
        if (villa == null)
            return BadRequest();
        patchDTO.ApplyTo(villaDto, ModelState);
        
        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            Id = villaDto.Id,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return NoContent();
    }
}