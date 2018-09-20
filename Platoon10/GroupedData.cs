using System.Collections.Generic;

namespace Platoon10
{
    public class GroupedDataCollection<T> : List<T>
    {
        public object Key { get; set; }

        public new IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)base.GetEnumerator();
        }
    }
}