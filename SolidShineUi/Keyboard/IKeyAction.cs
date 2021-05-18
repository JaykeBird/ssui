using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.Keyboard
{
    public interface IKeyAction
    {
        string ID { get; set; }

        void Execute();

#if NETCOREAPP
        UIElement? SourceElement { get; }
#else
        UIElement SourceElement { get; }
#endif
    }

    public class KeyActionList : List<IKeyAction>
    {

        /// <summary>
        /// Get if this list contains an IKeyAction with this ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsId(string id)
        {
            return this.Where(a => a.ID == id).Any();
        }
        
        /// <summary>
        /// Get an IKeyAction with a particular ID.
        /// </summary>
        /// <param name="id">The ID to get.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no item with this ID.</exception>
        public IKeyAction GetFromId(string id)
        {
            return this.Where(a => a.ID == id).First();
        }


        public IKeyAction this[string id]
        {
            get => GetFromId(id);
        }

    }
}
