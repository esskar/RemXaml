using sugar;
using sugar.collections;
using sugar.xml;

namespace RemXaml.Shared
{
	public class XamlParser
	{
	    public object Load(XmlDocument doc, String rootName, String objectName, object eventSink)
	    {
	        if (doc == null)
	            throw new SugarArgumentNullException("doc");
			if (rootName == null || rootName.Length == 0)
				throw new SugarArgumentNullException("rootName");
			if (objectName == null || objectName.Length == 0)
				throw new SugarArgumentNullException("objectName");

			var state = new State(eventSink);

	        var retval = (object) null;

	        return retval;
	    }


	    private class State
	    {
	        public readonly Dictionary<string, object> Namespaces;
	        public readonly Dictionary<string, object> Objects;
	        public readonly object EventSink;

	        public State(object eventSink)
	        {
	            if (eventSink == null)
	                throw new SugarArgumentNullException("eventSink");
	            this.EventSink = eventSink;

	            this.Namespaces = new Dictionary<string, object>();
	            this.Objects = new Dictionary<string, object>();
	        }
	    }
	}
}
