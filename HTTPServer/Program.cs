using System.Net;
using HTTPServer;

var httpListener = new HttpListener();
var manualResetEvent = new ManualResetEvent(false);


// netsh http add urlacl url=http://26.76.43.135:9999/test/ user=Alex

httpListener.Prefixes.Add("http://26.76.43.135:9999/test/");

httpListener.Start();

// 100-199 - Information codes
// 200-299 - Success codes
// 300-399 - Redirects
// 400-499 - Error codes
// 500-599 - Server error codes

_ = Task.Run(async () =>
{
    try
    {
        while (httpListener.IsListening)
        {
            var context = await httpListener.GetContextAsync();

            // Console.WriteLine($"QUERY STRING: {context.Request.QueryString}");
            // Console.WriteLine($"METHOD: {context.Request.HttpMethod}");
            // Console.WriteLine($"{context.Request.RemoteEndPoint}");
            //
            // Console.WriteLine("HEADERS: ");
            //
            // int index = 0;
            //
            // foreach (var header in context.Request.Headers)
            // {
            //     if (index == context.Request.Headers.Count)
            //     {
            //         break;
            //     }
            //
            //     Console.WriteLine($"{header}: {context.Request.Headers.Get(index++)}");
            // }
            //
            // if (context.Request.AcceptTypes is not null)
            // {
            //     Console.WriteLine("ACCEPT-TYPES: ");
            //     foreach (var types in context.Request.AcceptTypes)
            //     {
            //         Console.WriteLine(types);
            //     }
            // }

            try
            {
                var url = context.Request.Url!.ToString();
                var page = url.Split('/').SkipLast(1).Last();

                if (page == "test" &&
                    context.Request.HttpMethod == HttpMethod.Get.Method)
                {
                    using var fileStream = File.Open("test.html", FileMode.Open, FileAccess.Read);

                    await fileStream.CopyToAsync(context.Response.OutputStream);

                    context.Response.StatusCode = HttpStatusCode.OK.ToInt32();
                }
                else
                {
                    context.Response.StatusCode = HttpStatusCode.BadRequest.ToInt32();
                }

                context.Response.Close();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = HttpStatusCode.InternalServerError.ToInt32();
            }
        }
    }
    catch (Exception ex)
    {
#if DEBUG
        Console.WriteLine(ex.Message);
#endif
    }
});


manualResetEvent.WaitOne();