using AutoMapper;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_Web.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;

    private readonly IMapper _mapper;

    public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper)
    {
        _villaNumberService = villaNumberService;
        _mapper = mapper;
    }
}