using System.Diagnostics;
using Calculator.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Project_CSCB532.Tests.Pages;

public class ErrorModelTests
{
    [Fact]
    public void OnGet_UsesActivityIdWhenPresent()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ErrorModel>>();
        var model = new ErrorModel(logger);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "trace-ignored" };
        model.PageContext = new PageContext { HttpContext = httpContext };
        var activity = new Activity("test-activity");
        activity.Start();

        // Act
        model.OnGet();
        activity.Stop();

        // Assert
        Assert.Equal(activity.Id, model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void OnGet_UsesTraceIdentifierWhenNoActivity()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ErrorModel>>();
        var model = new ErrorModel(logger);
        var httpContext = new DefaultHttpContext { TraceIdentifier = "trace-identifier" };
        model.PageContext = new PageContext { HttpContext = httpContext };

        // Act
        model.OnGet();

        // Assert
        Assert.Equal("trace-identifier", model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void OnGet_WithEmptyTraceIdentifier_SetsEmptyRequestId()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ErrorModel>>();
        var model = new ErrorModel(logger);
        var httpContext = new DefaultHttpContext { TraceIdentifier = string.Empty };
        model.PageContext = new PageContext { HttpContext = httpContext };

        // Act
        model.OnGet();

        // Assert
        Assert.Equal(string.Empty, model.RequestId);
        Assert.False(model.ShowRequestId);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("req-123", true)]
    public void ShowRequestId_ReturnsExpectedForValues(string? requestId, bool expected)
    {
        // Arrange
        var logger = Mock.Of<ILogger<ErrorModel>>();
        var model = new ErrorModel(logger) { RequestId = requestId };

        // Act
        var result = model.ShowRequestId;

        // Assert
        Assert.Equal(expected, result);
    }
}
