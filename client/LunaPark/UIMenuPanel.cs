using System.Threading.Tasks;

namespace LunaPark
{
	public class UIMenuPanel
	{
		internal dynamic Background;

		public virtual bool Selected { get; set; }

		public virtual bool Enabled { get; set; }

		public UIMenuListItem ParentItem { get; set; }

		internal virtual void Position(float y)
		{
		}

		public virtual void UpdateParent()
		{
			ParentItem.Parent.ListChange(ParentItem, ParentItem.Index);
			ParentItem.ListChangedTrigger(ParentItem.Index);
		}

		internal virtual async Task Draw()
		{
		}

		public void SetParentItem(UIMenuListItem item)
		{
			ParentItem = item;
		}
	}
}
