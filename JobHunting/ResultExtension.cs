using JobHunting.Application.Common;
using Microsoft.AspNetCore.Mvc;


namespace JobHunting
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(
            this Result<T> result,
            ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.Ok(result.Value);

            return result.Error!.Type switch
            {
                ErrorType.NotFound
                    => controller.NotFound(result.Error),

                ErrorType.Invalid
                    => controller.BadRequest(result.Error),

                ErrorType.Conflict
                    => controller.Conflict(result.Error),

                ErrorType.Unauthorized
                    => controller.Unauthorized(result.Error),

                ErrorType.Forbidden
                    => controller.Forbid(),

                _ => controller.StatusCode(500, result.Error)
            };
        }

        public static IActionResult ToActionResult(
            this Result result,
            ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.NoContent();

            return result.Error!.Type switch
            {
                ErrorType.NotFound => controller.NotFound(result.Error),
                ErrorType.Invalid => controller.BadRequest(result.Error),
                ErrorType.Conflict => controller.Conflict(result.Error),
                ErrorType.Unauthorized => controller.Unauthorized(result.Error),
                ErrorType.Forbidden => controller.Forbid(),
                _ => controller.StatusCode(500, result.Error)
            };
        }
    }
}
