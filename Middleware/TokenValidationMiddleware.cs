using CulinaryCart.CulinaryCartDAL.Repositories;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, UserDAL userDal)
    {
        //  Allow public endpoints (login, signup, etc.)
        if (context.Request.Path.StartsWithSegments("/api/Auth") ||
            context.Request.Path.StartsWithSegments("/api/User/Signup"))
        {
            await _next(context);
            return;
        }

        // Extract token from Authorization header
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        
        if (!string.IsNullOrEmpty(token) && userDal.IsTokenRevoked(token))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        

        // Continue down the pipeline
        await _next(context);
    }
}


