using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers.v1;

// [Route("api/[controller]")]
[Route("api/v{version:apiVersion}/VillaAPI")]
[ApiController]
[ApiVersion("1.0")]
public class VillaAPIController : ControllerBase
{
    protected APIResponse _response;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
    {
        _dbVilla = dbVilla;
        _mapper = mapper;
        this._response = new();
    }
    
    [HttpGet]
    // [Authorize]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetVillas()
    {
        try
        {
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            
            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody]VillaCreateDTO createDto)
    {
        try
        {
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (createDto == null)      
                return BadRequest(createDto);

            Villa villa = _mapper.Map<Villa>(createDto);
        
            await _dbVilla.CreateAsync(villa);
        
            _response.Result = _mapper.Map<List<VillaDTO>>(villa);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVilla", new {id = villa.Id},_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        try
        {
            if (id == 0)
                return BadRequest();

            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
                return NotFound();

            await _dbVilla.RemoveAsync(villa);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDto)
    {
        try
        {
            if (updateDto == null || id != updateDto.Id)
                return BadRequest();
        
            Villa model = _mapper.Map<Villa>(updateDto);

            await _dbVilla.UpdateAsync(model);
        
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
            return BadRequest();
        
        var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

        VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);
        
        if (villa == null)
            return BadRequest();
        
        patchDTO.ApplyTo(villaDto, ModelState);
         
        Villa model = _mapper.Map<Villa>(villaDto);

        await _dbVilla.UpdateAsync(model);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return NoContent();
    }
}