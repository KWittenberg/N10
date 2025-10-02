namespace N10.DTOs;

// SERVICE RESPONSE for API
public record ServiceResponse(HttpStatusCode HttpStatusCode, string Message);

public record ServiceResponse<T>(HttpStatusCode HttpStatusCode, string Message, T? Data = default);

public record ServiceResponseList<T>(HttpStatusCode HttpStatusCode, string Message, List<T>? Data = default);

public record ServiceResponsePagination<T>(HttpStatusCode HttpStatusCode, string Message, List<T>? Data, PaginationDto? Pagination);

public record ServiceResponseQgPagination<T>(HttpStatusCode HttpStatusCode, string Message, List<T>? Data, QgPaginationDto? Pagination);



// Result
//public record Result(bool Success, string Message);

//public record Result<T>(bool Success, string Message, T? Data = default);


public record Result(bool Success, string Message)
{
    public static Result Ok(string message = "Success") => new(true, message);
    public static Result Error(string message) => new(false, message);
}

public record Result<T>(bool Success, string Message, T? Data = default)
{
    public static Result<T> Ok(T data, string message = "Success") => new(true, message, data);
    public static Result<T> Error(string message) => new(false, message, default);
}







// Pagination
public record PaginationDto(int TotalRecords, int Page, int PageSize);

public record QgPaginationDto(int TotalRecords, int Page, int PageSize, string? SortBy = null, bool SortDescending = false);

public record OrderCounterDto(int Year, int OrderCount);


// ROLE
//public record RoleDto(Guid Id, string Name);

// Content
//public record ContentDto(Guid Id, string Name, string FilePath, ContentType Type, ContentOrientation? Orientation);



// CATEGORY
public record CategoryDto(Guid Id, string Name, string? Description, string? IconHtml);



public record ProductPromotionDto(Guid PromotionId, Guid ProductId);

// AUTHOR
// public record AuthorOutput(Guid Id, string FirstName, string LastName, string? ImageUrl, DateOnly? DateOfBirth, string? Biography, string? Description);
//public record AuthorDto(Guid Id, string FirstName, string LastName, string? ImageUrl, DateOnly? DateOfBirth, string? Born, string? Died,
//                            string? Awards, string? Biography, string? Description, List<Product>? Products);

//public record AuthorDto(Guid Id, string FirstName, string LastName, string? ImageUrl, DateOnly? DateOfBirth, string? Born, string? Died,
//                        string? Awards, string? Biography, string? Description, List<Product>? Products)
//{
//public string AuthorImageUrl => string.IsNullOrEmpty(ImageUrl) ? "/img/avatar/user.png" : ImageUrl;
//}



// SUBSCRIBER
// public record SubscriberInput(string Email);
public record SubscriberDto(Guid Id, string Email, DateTime SubscribedOn);



// PRODUCT
public record ProductSeed(
                        string Category,
                        string Title,
                        string? ImageUrl,
                        string? Highlight,
                        string? Description,
                        bool IsAvailable,
                        int? InStock,
                        decimal Price,
                        bool Sale,
                        decimal? SalePrice,
                        DateTime? SaleStartDate,
                        DateTime? SaleEndDate,
                        string? Dimensions,
                        int? Weight,
                        int? Year,
                        string? CountryOfOrigin);

//public record ProductDto(Guid Id, string Title, CategoryDto? Category, List<AuthorDto>? Authors, string? DescriptionShort, string? Description, string? ImageUrl,
//                            bool Available, int? Quantity, decimal Price, bool Discount, decimal DiscountPrice, DateTime? SaleStart, DateTime? SaleEnd,
//                            BookSpecificationDto? BookSpecification);



// BOOK SPECIFICATIONS
//public record BookSpecificationSeed(string Title, string? Publisher, string? Isbn, int? YearOfPublication, BookCategory BookCategory, BookBinding BookBinding,
//                                    int? NumberOfPages, int? Width, int? Height, int? Thickness, int? Weight);

//public record BookSpecificationDto(Guid Id, string? Publisher, string? Isbn, int? YearOfPublication, BookCategory BookCategory, BookBinding BookBinding,
//                                        int? NumberOfPages, int? Width, int? Height, int? Thickness, int? Weight);


// PRODUCT AUTHORS
public record ProductAuthorsSeed(string Title, List<string> Authors);

// PRODUCT AUTHORS
public record ProductAuthorDto(Guid ProductId, Guid AuthorId);


// PRODUCT IMAGES
public record ProductImagesSeed(string Title, string Folder, List<string> Images);


//public record ShoppingCartDto(Guid Id, Guid UserId, Guid ProductId, int Quantity, decimal Price);




// WISHLIST
public record WishlistDto(Guid Id, Guid ApplicationUserId, Guid ProductId);


// TOAST
// public record ToastMessage(string Title, string Message);
// public record ToastMessage(string? Title, string? Message, string? HeaderClass);


// public record Response<T>(HttpStatusCode HttpStatusCode, string Message, T? Data);

//public record CategoryOutput(Guid Id, string Name, string? Description);

//public record CategoryInput(string Name, string? Description);

//public record AddressDTO(string Type, string AddressLine1, string? AddressLine2, string PostalCode, string City, string Country, bool IsCurrent);


//public record ScreenChartDto(DateTime Date, decimal? Total);

//public class ScreenChartDto
//{
//    public DateTime Date { get; set; }
//    public int ImageCount { get; set; }
//    public int VideoCount { get; set; }

//    public ScreenChartDto(DateTime date, int imageCount, int videoCount)
//    {
//        Date = date;
//        ImageCount = imageCount;
//        VideoCount = videoCount;
//    }
//}

//public record DashboardDto(List<ScreenDto> Screens, int ScreenCount, int ContentCount, int UserCount);


//public record DashboardDto(List<ScreenDto> Screens, List<ScreenChartDto> ScreenCharts, int UserCount, int ScreenCount, int ContentCount);

//public record DashboardDto(int Manufacturers, int Brands, int Products, int Orders, int Users, int Categories, int Authors, int Coupons, List<OrderChartDto> OrderCharts);