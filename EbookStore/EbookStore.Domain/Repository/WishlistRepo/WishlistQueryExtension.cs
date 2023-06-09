﻿using EbookStore.Contract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Windows.Documents;

namespace EbookStore.Domain.Repository.WishlistRepo;
public static class WishlistQueryExtension
{
    public static IQueryable<Book> QueryCurrentWishItem(this IQueryable<Book> books, IQueryable<WishItem> wishItems, Guid id)
    {
        var result = books.Join(wishItems
                            .Where(w => w.UserId == id && w.IsActive == true),
                                b => b.BookId,
                                w => w.BookId,
                                (b, w) => new Book
                                {
                                    BookId = b.BookId,
                                    Title = b.Title,
                                    IsActive= b.IsActive,
                                    NumberOfPage= b.NumberOfPage,
                                    Price= b.Price,
                                    Description= b.Description,
                                    CoverImage = b.CoverImage,
                                    PdfLink= b.PdfLink,
                                    //EpubLink= b.EpubLink,
                                    ReleaseDate= b.ReleaseDate,
                                    Sale  = b.Sale
                                });
        return result;
    }

    public static IQueryable<Book> QueryId(this IQueryable<Book> query, int id)
    {
        return query.Where(x => x.BookId == id);
    }

    public static IQueryable<Book> QueryTitle(this IQueryable<Book> query, string title)
    {
        return query.Where(b => b.Title.Contains(title));
    }

    public static IQueryable<Book> QueryGenres(this IQueryable<Book> query, List<int> genres)
    {
        if (genres.Count > 0)
        {
            query = query.Where(b => b.BookGenres.Any(bg => genres.Contains(bg.GenreId)));
        }
        return query;
    }

    public static IQueryable<Book> QueryReleaseDate(
        this IQueryable<Book> query, DateTime start, DateTime end)
    {
        return query.Where(b => DateTime.Compare(b.ReleaseDate, start) > 0
            && DateTime.Compare(b.ReleaseDate, end) < 0);
    }

    public static IQueryable<Book> QueryActive(this IQueryable<Book> query)
    {
        return query.Where(x => x.IsActive == true);
    }
}
