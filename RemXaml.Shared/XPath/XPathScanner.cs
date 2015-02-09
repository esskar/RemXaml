using sugar;

namespace RemXaml.Shared.XPath
{
    internal sealed class XPathScanner
    {
        private readonly String _source;

        private int _currentIndex;
        private char _currentChar;
        private XPathLexKind _kind;
        private String _name;
        private String _prefix;
        private String _value;
        private bool _canBeFunction;
        private int _lexStart;
        private int _prevLexEnd;
        private XPathLexKind _prevKind;
        private XPathAxis _axis;

		public XPathScanner(String source)
			: this(source, 0) { }

        public XPathScanner(String source, int startFrom)
        {
            if (source == null)
                throw new SugarArgumentNullException("source");
            _source = source;
            _kind = XPathLexKind.Unknown;

            this.SetCurrentIndex(startFrom);
            this.NextLex();
        }

        public String Source
        {
            get { return _source; }
        }

        public XPathLexKind Kind
        {
            get { return _kind; }
        }

        public int LexStart
        {
            get { return _lexStart; }
        }

        public int LexSize
        {
            get { return _currentIndex - _lexStart; }
        }

        public int PrevLexEnd
        {
            get { return _prevLexEnd; }
        }

		public XPathAxis Axis
		{
		    get
		    {
		        if (this.Kind != XPathLexKind.Axis || _axis == XPathAxis.Unknown)
                    throw new SugarInvalidOperationException("Not a axis.");
		        return _axis;
		    }
		}

		public bool CanBeFunction
		{
		    get
		    {
		        if (this.Kind != XPathLexKind.Name)
                    throw new SugarInvalidOperationException("Not a name.");
		        return _canBeFunction;
		    }
		}

        public String Name
        {
            get
            {
                if (this.Kind != XPathLexKind.Name || _name == null)
                    throw new SugarInvalidOperationException("Not a name.");
                return _name;
            }
        }

        public String Prefix
        {
            get
            {
                if (this.Kind != XPathLexKind.Name || _prefix == null)
                    throw new SugarInvalidOperationException("Not a prefix.");
                return _prefix;
            }
        }

        public String RawValue
        {
            get
            {
                if (this.Kind == XPathLexKind.Eof)
                    return XPathLexKindStringBuilder.ToString(this.Kind);
                else
                    return _source.Substring(_lexStart, _currentIndex - _lexStart);
            }
        }


        public String Value
        {
            get
            {
                if (this.Kind != XPathLexKind.String || _value == null)
                    throw new SugarInvalidOperationException("Not a string value.");
                return _value;
            }
        }

        private void SetCurrentIndex(int index)
        {
            if (index < 0 || index >= _source.Length)
                throw new SugarArgumentOutOfRangeException("index");

            _currentIndex = index - 1;
            this.NextChar();
        }

        private void NextChar()
        {
            _currentIndex++;
            if (_currentIndex < _source.Length)
                _currentChar = _source[_currentIndex];
            else if (_currentIndex == _source.Length)
                _currentChar = '\0';
            else
                throw new SugarIOException();
        }

        private void NextLex()
        {
            _prevLexEnd = _currentIndex;
            _prevKind = _kind;

            this.SkipWhiteSpace();

            _lexStart = _currentIndex;
			switch (_currentIndex)
			{
				case '\0':
			        _kind = XPathLexKind.Eof;
			        return;
				case '(':
			        _kind = XPathLexKind.LParens;
			        this.NextChar();
			        break;
				case ')':
			        _kind = XPathLexKind.RParens;
			        this.NextChar();
			        break;
				case '[':
			        _kind = XPathLexKind.LBracket;
			        this.NextChar();
			        break;
				case ']':
			        _kind = XPathLexKind.RBracket;
			        this.NextChar();
			        break;
				case '@':
			        _kind = XPathLexKind.At;
			        this.NextChar();
			        break;
				case ',':
			        _kind = XPathLexKind.Comma;
			        this.NextChar();
			        break;
				case '$':
			        _kind = XPathLexKind.Dollar;
			        this.NextChar();
			        break;
				case '}':
			        _kind = XPathLexKind.RBrace;
			        this.NextChar();
			        break;
				case '.':
			        this.NextChar();
			        if (_currentChar == '.')
			        {
			            _kind = XPathLexKind.DoubleDot;
			            this.NextChar();
			        }
					else if (IsDigit(_currentChar))
					{
					    this.SetCurrentIndex(_lexStart);
					    goto case '0';
					}
					else
					{
					    _kind = XPathLexKind.Dot;
					}
			        break;
				case ':':
			        this.NextChar();
			        if (_currentChar == ':')
			        {
			            _kind = XPathLexKind.DoubleColon;
			            this.NextChar();
			        }
					else
					{
					    _kind = XPathLexKind.Unknown;
					}
			        break;
				case '*':
			        _kind = XPathLexKind.Star;
			        this.NextChar();
			        this.CheckOperator(true);
			        break;
				case '/':
			        this.NextChar();
			        if (_currentChar == '/')
			        {
			            _kind = XPathLexKind.DoubleSlash;
			            this.NextChar();
			        }
					else
					{
					    _kind = XPathLexKind.Slash;
					}
			        break;
				case '|':
			        _kind = XPathLexKind.Union;
			        this.NextChar();
			        break;
				case '+':
			        _kind = XPathLexKind.Plus;
			        this.NextChar();
			        break;
				case '-':
			        _kind = XPathLexKind.Minus;
			        this.NextChar();
			        break;
				case '=':
			        _kind = XPathLexKind.Eq;
			        this.NextChar();
			        break;
				case '!':
			        this.NextChar();
			        if (_currentChar == '=')
			        {
			            _kind = XPathLexKind.Ne;
			            this.NextChar();
			        }
			        else
			        {
			            _kind = XPathLexKind.Unknown;
			        }
			        break;
				case '<':
			        this.NextChar();
			        if (_currentChar == '=')
			        {
			            _kind = XPathLexKind.Le;
			            this.NextChar();
			        }
			        else
			        {
			            _kind = XPathLexKind.Lt;
			        }
			        break;
				case '>':
			        this.NextChar();
			        if (_currentChar == '=')
			        {
			            _kind = XPathLexKind.Ge;
			            this.NextChar();
			        }
			        else
			        {
			            _kind = XPathLexKind.Gt;
			        }
			        break;
				case '"':
            	case '\'':
               		_kind = XPathLexKind.String;
                	this.ScanString();
                	break;
				case '0': case '1': case '2': case '3':
            	case '4': case '5': case '6': case '7':
            	case '8': case '9':
                	_kind = XPathLexKind.Number;
                	this.ScanNumber();
                	break;


			}
        }

        private bool CheckOperator(bool star)
        {
            XPathLexKind op;

            if (star)
            {
                op = XPathLexKind.Multiply;
            }
            else
            {
                if (_prefix.Length > 0 || _name.Length > 3)
                    return false;

                switch (_name)
                {
                	case "or": op = XPathLexKind.Or; break;
					case "and": op = XPathLexKind.And; break;
					case "div": op = XPathLexKind.Divide; break;
					case "mod": op = XPathLexKind.Modulo; break;
					default: return false;
                }
            }

            if (_prevKind.IsOperator())
                return false;

            if (_prevKind.IsOneOf(
                XPathLexKind.Unknown,
                XPathLexKind.Slash,
                XPathLexKind.DoubleSlash,
 				XPathLexKind.At,
				XPathLexKind.DoubleColon,
				XPathLexKind.LParens,
				XPathLexKind.LBracket,
				XPathLexKind.Comma,
				XPathLexKind.Dollar
                )) return false;

            _kind = op;

            return true;
        }

        private void ScanNumber()
        {
            while (IsDigit(_currentChar))
                this.NextChar();

            if (_currentChar == '.')
            {
                this.NextChar();
				while (IsDigit(_currentChar))
                	this.NextChar();
            }

            if ((_currentChar & (~0x20)) == 'E')
            {
                this.NextChar();
                if (_currentChar == '+' || _currentChar == '-')
                    this.NextChar();

				while (IsDigit(_currentChar))
                	this.NextChar();

				throw ScientificNotationException();
            }
        }

        private void ScanString()
        {
            var startIdx = _currentIndex + 1;
            var endIdx = _source.Substring(startIdx).IndexOf(_currentChar);

            if (endIdx < 0)
                throw UnclosedStringException();

            endIdx += startIdx;
            
			_value = _source.Substring(startIdx, endIdx - startIdx);
            this.SetCurrentIndex(endIdx + 1);
        }

        private void SkipWhiteSpace()
        {
            while (IsWhiteSpace(_currentChar))
                this.NextChar();
        }

        private static bool IsDigit(char ch)
        {
            return (uint)(ch - '0') <= 9;
        }

        private static bool IsWhiteSpace(char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
        }

		public XPathParserException UnexpectedTokenException(string token) 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex,
                String.Format("Unexpected token '{0}' in the expression.", token)
            );
        }

        public XPathParserException NodeTestExpectedException(string token) 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex, 
                String.Format("Expected a node test, found '{0}'.", token)
            );
        }

        public XPathParserException PredicateAfterDotException() 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex,
                "Abbreviated step '.' cannot be followed by a predicate. Use the full form 'self::node()[predicate]' instead."
            );
        }

        public XPathParserException PredicateAfterDotDotException() 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex,
                "Abbreviated step '..' cannot be followed by a predicate. Use the full form 'parent::node()[predicate]' instead."
            );
        }
        public XPathParserException ScientificNotationException() 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex,
                "Scientific notation is not allowed."
            );
        }

        public XPathParserException UnclosedStringException() 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex,
                "String literal was not closed."
            );
        }

        public XPathParserException EofExpectedException(string token) 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex, 
                String.Format("Expected end of the expression, found '{0}'.", token)
            );
        }

        public XPathParserException TokenExpectedException(string expectedToken, string actualToken) 
		{
            return new XPathParserException(_source, _lexStart, _currentIndex,
                String.Format("Expected token '{0}', found '{1}'.", expectedToken, actualToken)
            );
        }
    }
}
