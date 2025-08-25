namespace CircuitSimBackend
{
    class DataWrapper<T>
    {
        T? data { get; }
        DataWrapper<T>? next { get; }

        public DataWrapper(T data)
        {
            this.data = data;
        }

        /// <summary>Adds a data entry to the end of the singly linked DataWrapper list.</summary>
        public void Add(T data)
        {
            if (this.data == null)
            {
                this.data = data;
            }
            else
            {
                DataWrapper<T> index = this;
                while (index.next != null)
                {
                    index = index.next;
                }
                index.next = new DataWrapper<T>(data);
            }
        }

        /// <summary>Returns a list of data entries within this singly linked DataWrapper list.</summary>
        /// <remarks>Should be run on the first entry in a DataWrapper list to avoid issues.</remarks>
        public override string ToString()
        {
            if (data == null) return "";
            DataWrapper<T>? index = this.next;
            String? nameList = data.ToString();
            while (index != null)
            {
                if (index.data != null)
                {
                    nameList += " " + index.data.ToString();
                }
                index = index.next;
            }
            if (nameList == null) return ""; // (nameList cannot be null in this case)
            return nameList; 
        }

        /// <summary>Counts the nodes from this head to the end.</summary>
        public int Count()
        {
            if (data == null)
            {
                return 0;
            }
            int count = 0;
            DataWrapper<T>? index = this;
            while (index != null)
            {
                count++;
                index = index.next;
            }
            return count;
        }
    }
}