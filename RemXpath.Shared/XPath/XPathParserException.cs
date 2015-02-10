using sugar;

namespace RemXpath
{
	public class XPathParserException : SugarException
	{
	    public readonly String QueryString;

	    public readonly int StartChar;

	    public readonly int EndChar;

	    public XPathParserException(String queryString, int startChar, int endChar, String message)
	        : base(message)
	    {
	        this.QueryString = queryString;
	        this.StartChar = startChar;
	        this.EndChar = endChar;
	    }

	    private enum TrimType
	    {
	        Left,
			Right,
			Middle
	    }

	    private static void AppendTrimmed(StringBuilder sb, String value, int startIndex, int count, TrimType trimType)
	    {
	        const int TrimSize = 32;
	        const string TrimMarker = "...";

	        if (count <= TrimSize)
	        {
	            sb.Append(value, startIndex, count);
	        }
	        else
	        {
	            switch (trimType)
				{
					case TrimType.Left:
				        sb.Append(TrimMarker);
				        sb.Append(value, startIndex + count - TrimSize, TrimSize);
				        break;

					case TrimType.Right:
				        sb.Append(value, startIndex, TrimSize);
				        sb.Append(TrimMarker);
				        break;

					case TrimType.Middle:
				        sb.Append(value, startIndex, TrimSize/2);
				        sb.Append(TrimMarker);
				        sb.Append(value, startIndex + count - TrimSize/2, TrimSize/2);
				        break;
				}
	        }
	    }

	    private String BuildErrorMessage()
	    {
	        if (this.QueryString == null || this.QueryString.Length == 0)
	            return null;

	        var sb = new StringBuilder();
	        AppendTrimmed(sb, this.QueryString, 0, this.StartChar, TrimType.Left);

			var len = this.EndChar - this.StartChar;
	        if (len > 0)
	        {
	            sb.Append(" -->");
	            AppendTrimmed(sb, this.QueryString, this.StartChar, len, TrimType.Middle);
	        }
	        sb.Append("<-- ");
			AppendTrimmed(sb, this.QueryString, this.EndChar, this.QueryString.Length - this.EndChar, TrimType.Right);

	        return sb.toString();
	    }

	    private String BuildDetailedErrorMessage()
	    {
	        String message = this.Message;
	        String error = this.BuildErrorMessage();
	        if (error != null && error.Length > 0)
	        {
	            if (message.Length > 0)
	                message += Environment.NewLine;
	            message += error;
	        }
	        return message;
	    }

		#if COOPER
		public override java.lang.String toString()
		{
			java.lang.String retval = this.Class.Name;
			java.lang.String error =  this.BuildDetailedErrorMessage();

			if (error != null && error.length() > 0)
			{
				retval += ": ";
				retval += error;
			}

			if (this.StackTrace != null)
			{
				retval += Environment.NewLine;
				retval += this.StackTrace;
			}

			return retval;
		}
		#endif
	}
}
