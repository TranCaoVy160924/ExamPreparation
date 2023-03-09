﻿using AutoMapper;
using EbookStore.Contract.Model;
using EbookStore.Contract.ViewModel.Book.BookResponse;
using EbookStore.Contract.ViewModel.CartItem.Request;
using EbookStore.Contract.ViewModel.Pagination;
using EbookStore.Data.EF;
using EbookStore.Domain.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookStore.Domain.Repository.CartlistRepo;
public class CartlistRepository : ICartlistRepository
{
    private readonly EbookStoreDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CartlistRepository(
       EbookStoreDbContext dbContext,
       IMapper mapper,
       UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userManager = userManager;
    }

    #region addToCartAsync
    public async Task AddBookToCartlistAsync(int bookId, Guid userId)
    {
        var existingWishItem = await _dbContext.CartItems.SingleOrDefaultAsync(ci => ci.UserId == userId && ci.BookId == bookId);
        if (existingWishItem != null)
        {
            throw new ApplicationException($"This book {bookId} is already in cart.");
        }
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new ApplicationException($"Unable to find user with username: {userId}");
        }

        var book = await _dbContext.Books.SingleOrDefaultAsync(b => b.BookId == bookId);

        if (book == null)
        {
            throw new ApplicationException($"Unable to find book with id: {bookId}");
        }

        var cartItem = new CartItem
        {
            UserId = user.Id,
            User = user,
            BookId = book.BookId,
            Book = book,
            IsActive = true
        };

        _dbContext.CartItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();
    }
    #endregion

    #region GetAsync
    public async Task<PagedList<BookResponse>> GetAsync(CartItemQueryRequest request, Task<Guid> id)
    {
        IQueryable<Book> query = _dbContext.Books
            .Include(b => b.Sale)
            .QueryActive()
            .QueryTitle(request.Title)
            .QueryGenres(request.Genres)
            .QueryReleaseDate(request.StartReleaseDate, request.EndReleaseDate)
            .QueryCurrentCartItem(_dbContext.CartItems, await id)
            .AsQueryable();

        var paginatedResult = await query.PaginateResultAsync(request);
        return paginatedResult.MapResultToResponse<Book, BookResponse>(_mapper);
    }
    #endregion
}