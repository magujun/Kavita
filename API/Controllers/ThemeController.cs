﻿using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Theme;
using API.Extensions;
using API.Services;
using API.Services.Tasks;
using Kavita.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ThemeController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IThemeService _themeService;
    private readonly ITaskScheduler _taskScheduler;

    public ThemeController(IUnitOfWork unitOfWork, IThemeService themeService, ITaskScheduler taskScheduler)
    {
        _unitOfWork = unitOfWork;
        _themeService = themeService;
        _taskScheduler = taskScheduler;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SiteThemeDto>>> GetThemes()
    {
        return Ok(await _unitOfWork.SiteThemeRepository.GetThemeDtos());
    }

    [Authorize("RequireAdminRole")]
    [HttpPost("scan")]
    public ActionResult Scan()
    {
        _taskScheduler.ScanSiteThemes();
        return Ok();
    }

    [Authorize("RequireAdminRole")]
    [HttpPost("update-default")]
    public async Task<ActionResult> UpdateDefault(UpdateDefaultThemeDto dto)
    {
        await _themeService.UpdateDefault(dto.ThemeId);
        return Ok();
    }

    /// <summary>
    /// Returns css content to the UI. UI is expected to escape the content
    /// </summary>
    /// <returns></returns>
    [HttpGet("download-content")]
    public async Task<ActionResult<string>> GetThemeContent(int themeId)
    {
        try
        {
            return Ok(await _themeService.GetContent(themeId));
        }
        catch (KavitaException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
