﻿using EbookStore.Application.Helpers;
using EbookStore.Contract.Model;
using EbookStore.Contract.ViewModel.WishItem.Request;
using EbookStore.Domain.Repository.WishlistRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EbookStore.Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WishlistController : ControllerBase
{
    private readonly IWishlistRepository _wishlistRepo;
    private readonly UserManager<User> _userManager;

    public WishlistController(
        IWishlistRepository wishlistRepo,
        UserManager<User> userManager)
    {
        _wishlistRepo = wishlistRepo;
        _userManager = userManager;
    }

    [HttpPost("Search")]
    [Authorize]
    public async Task<IActionResult> GetAsync([FromBody] WishItemQueryRequest queryRequest)
    {
        try
        {
            var pagedResult = await _wishlistRepo.GetAsync(queryRequest, GetUserId());

            Response.Headers.Add("X-Pagination", pagedResult.GetMetadata());

            return Ok(pagedResult.Data);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<Guid> GetUserId()
    {
        var claims = User.Claims.ToList();

        var userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        User user = await _userManager.FindByNameAsync(userName);

        return user.Id;
    }

    [HttpPost("{bookId}")]
    [Authorize]
    public async Task<IActionResult> AddBookToWishlist(int bookId)
    {
        try
        {
            var userId = GetUserId();

            await _wishlistRepo.AddBookToWishlistAsync(bookId, await userId);
            return Ok();
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }
}
