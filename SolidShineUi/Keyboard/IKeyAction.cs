using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidShineUi.KeyboardShortcuts
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
        /// Add an IKeyAction to the list. If an IKeyAction with the same ID is already on this list, this new IKeyAction is not added.
        /// </summary>
        /// <param name="action">The IKeyAction to add.</param>
        /// <returns>True if the item was added; false if the item was not added (an action is already on this list with the same ID).</returns>
        public new bool Add(IKeyAction action)
        {
            if (!ContainsId(action.ID))
            {
                base.Add(action);
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Add a collection of IKeyAction items to this list. If an IKeyAction with the same ID is already on this list, the new IKeyAction is not added and is skipped.
        /// </summary>
        /// <param name="actions">The collection of IKeyAction items to add.</param>
        /// <returns>The collection of items that were successfully added to the list. Any items that are not on this list were not added, as an action is already on this list with the same ID.</returns>
        public new IEnumerable<IKeyAction> AddRange(IEnumerable<IKeyAction> actions)
        {
            IEnumerable<IKeyAction> insertables = actions.Where(a => !ContainsId(a.ID));
            base.AddRange(insertables);

            return insertables;
        }

        /// <summary>
        /// Inserts an IKeyAction to the list at the specified index. If an IKeyAction with the same ID is already on this list, this new IKeyAction is not inserted.
        /// </summary>
        /// <param name="index">The zero-based index to insert the IKeyAction at.</param>
        /// <param name="action">The IKeyAction to add.</param>
        /// <returns>True if the item was added; false if the item was not added (an action is already on this list with the same ID).</returns>
        public new bool Insert(int index, IKeyAction action)
        {
            if (!ContainsId(action.ID))
            {
                base.Insert(index, action);
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Inserts a collection of IKeyAction items to the list at the specified index. If an IKeyAction with the same ID is already on this list, the new IKeyAction is not inserted and is skipped.
        /// </summary>
        /// <param name="index">The zero-based index to insert the collection at.</param>
        /// <param name="actions">The collection of IKeyAction items to insert.</param>
        /// <returns>The collection of items that were successfully added to the list. Any items that are not on this list were not added, as an action is already on this list with the same ID.</returns>
        public new IEnumerable<IKeyAction> InsertRange(int index, IEnumerable<IKeyAction> actions)
        {
            IEnumerable<IKeyAction> insertables = actions.Where(a => !ContainsId(a.ID));
            base.InsertRange(index, insertables);

            return insertables;
        }

        /// <summary>
        /// Get if this list contains an IKeyAction with this ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsId(string id)
        {
            return this.Any(a => a.ID == id);
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

        /// <summary>
        /// Get an IKeyAction with a particular ID.
        /// </summary>
        /// <param name="id">The ID to get.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no item with this ID.</exception>
        public IKeyAction this[string id]
        {
            get => GetFromId(id);
        }

    }
}
