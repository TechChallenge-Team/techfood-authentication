using Microsoft.AspNetCore.Builder;

namespace TechFood.Infra
{
    public static class RequestPipeline
    {
        public static IApplicationBuilder UseInfra(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
