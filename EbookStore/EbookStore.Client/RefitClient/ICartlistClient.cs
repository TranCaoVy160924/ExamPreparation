﻿using EbookStore.Contract.ViewModel.Book.BookResponse;
using EbookStore.Contract.ViewModel.CartItem.Request;
using Refit;

namespace EbookStore.Client.RefitClient;

[Headers("Content-Type: application/json")]
public interface ICartlistClient
{
    [Post("/Cartlist/{bookId}/")]
    Task AddBookToCartlistAsync(int bookId, [Header("Authorization")] string jwtToken);

    [Post("/Cartlist/Search/")]
    Task<ApiResponse<List<BookResponse>>> GetResponseAsync([Body] CartItemQueryRequest queryRequest);
}