using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SchoolERP.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(ErrorOr<T> result)
    {
        return result.Match(
            value => Ok(new { success = true, data = value }),
            errors => Problem(errors));
    }

    private IActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
            return StatusCode(500, new { success = false, error = new { code = "UNKNOWN_ERROR" } });

        var firstError = errors[0];

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return StatusCode(statusCode, new
        {
            success = false,
            error = new
            {
                code = firstError.Code,
                message = firstError.Description
            }
        });
    }
}
