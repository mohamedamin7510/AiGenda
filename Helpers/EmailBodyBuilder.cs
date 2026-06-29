namespace BucketSurvey.Api.Helpers;

public static class EmailBodyBuilder
{
    public static string  GenerateEmailBody(string Template , Dictionary<string,string> PlaceHolderValues)
    {

        string TemplatePath = $"{Directory.GetCurrentDirectory()}/Templates/{Template}.html";
        
        StreamReader SR = new StreamReader(TemplatePath);

        var Body = SR.ReadToEnd();

        SR.Close();

        foreach (var item in PlaceHolderValues)
           Body = Body.Replace(item.Key , item.Value) ;

        return Body;
    }

}
