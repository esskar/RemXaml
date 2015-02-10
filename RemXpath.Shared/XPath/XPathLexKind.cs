using sugar;

namespace RemXpath
{
	internal enum XPathLexKind
	{
		Unknown,
		Or,
		And,
		Eq,
		Ne,
		Lt,
		Le,
		Gt,
		Ge,
		Plus,
		Minus,
		Multiply,
		Divide,
		Modulo,
		UnaryMinus,
		Union,

		DoubleDot,
		DoubleColon,
		DoubleSlash,
		Number,
		Axis,

		Name,
		String,
		Eof,

		LParens,
		RParens,
		LBracket,
		RBracket,
		Dot,
		At,
		Comma,

		Star,
		Slash,
		Dollar,
		RBrace
	}

    internal static class XPathLexKindExtensions
    {
        public static bool IsOneOf(this XPathLexKind kind, params XPathLexKind[] list)
        {
            foreach (var lexKind in list)
            {
                if (kind == lexKind)
                    return true;
            }
            return false;
        }

        public static bool IsOperator(this XPathLexKind kind)
        {
            return kind.IsOneOf(
				XPathLexKind.Or,
				XPathLexKind.And,
				XPathLexKind.Eq,
				XPathLexKind.Ne,
				XPathLexKind.Lt,
				XPathLexKind.Le,
				XPathLexKind.Gt,
				XPathLexKind.Ge,
				XPathLexKind.Plus,
				XPathLexKind.Minus,
				XPathLexKind.Multiply,
				XPathLexKind.Divide,
				XPathLexKind.Modulo,
				XPathLexKind.UnaryMinus,
                XPathLexKind.Union);
        }
    }

	internal static class XPathLexKindStringBuilder
	{
		public static String ToString(XPathLexKind kind)
		{
		    switch (kind)
		    {
				case XPathLexKind.Eof: return "<eof>";
				case XPathLexKind.Name: return "<name>";
				case XPathLexKind.String: return "<string literal>";
				case XPathLexKind.LParens: return "(";
				case XPathLexKind.RParens: return ")";
				case XPathLexKind.LBracket: return "[";
				case XPathLexKind.RBracket: return "]";
				case XPathLexKind.Dot: return ".";
				case XPathLexKind.At: return "@";
				case XPathLexKind.Comma: return ",";
				case XPathLexKind.Star: return "*";
				case XPathLexKind.Slash: return "/";
				case XPathLexKind.Dollar: return "$";
				case XPathLexKind.RBrace: return "}";

				default:
		            throw new SugarInvalidOperationException();
		    }
		}
	}
}
