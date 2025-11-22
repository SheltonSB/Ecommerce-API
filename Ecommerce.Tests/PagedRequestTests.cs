using Ecommerce.Api.Contracts;

namespace Ecommerce.Tests;

public class PagedRequestTests
{
    [Fact]
    public void Validate_Clamps_Invalid_Values()
    {
        var request = new PagedRequest
        {
            Page = -5,
            PageSize = 500,
            SortDirection = "Descending"
        };

        request.Validate();

        Assert.Equal(1, request.Page);
        Assert.Equal(100, request.PageSize);
        Assert.Equal("desc", request.SortDirection);
    }
}
