public class EmailBodyBuilder(IWebHostEnvironment env)
{
    public string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
    {
        var path = Path.Combine(env.WebRootPath, "Templates", $"{template}.html");

        var body = File.ReadAllText(path);

        foreach (var item in templateModel)
            body = body.Replace(item.Key, item.Value);

        return body;
    }
}