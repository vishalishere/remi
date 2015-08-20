namespace ReMi.Contracts.Plugins.Data
{
    public struct NameValuePair
    {
        private string _name;
        private string _value;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public NameValuePair(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return string.Format("[Name={0}, Value={1}]",
                Name, Value);
        }
    }
}
