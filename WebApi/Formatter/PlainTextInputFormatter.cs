using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace WebApi.Formatter
{
    public class PlainTextInputFormatter : TextInputFormatter
    {
	    public PlainTextInputFormatter()
	    {
			SupportedMediaTypes.Add("text/plain");
			SupportedEncodings.Add(UTF8EncodingWithoutBOM);
			SupportedEncodings.Add(UTF16EncodingLittleEndian);
	    }

	    protected override bool CanReadType(Type type)
	    {
		    return type == typeof(string);
	    }

	    /// <summary>Reads an object from the request body.</summary>
	    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.InputFormatterContext" />.</param>
	    /// <param name="encoding">The <see cref="T:System.Text.Encoding" /> used to read the request body.</param>
	    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that on completion deserializes the request body.</returns>
	    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
	    {
		    string data = null;
		    using (var sr = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
		    {
			    data = await sr.ReadToEndAsync();
		    }

			return InputFormatterResult.Success(data);
	    }
    }
}
