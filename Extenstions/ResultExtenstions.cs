namespace AI_genda_API.Extenstions;

public static class ResultExtenstions
{
    public static ObjectResult  ToProblem(this Result result)
    {

        var problem = Results.Problem(statusCode: result.Error.StatusCode);

        var problemdetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
        
        problemdetails!.Extensions = new Dictionary<string, object?>()
        {
            { 
                "error", new string[] { result.Error.Code , result.Error.Descrption }            
            }

        };

        
        return new ObjectResult(problem);
    }

}
