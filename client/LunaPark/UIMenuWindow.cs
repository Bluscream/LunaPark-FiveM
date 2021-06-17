namespace LunaPark
{
	public class UIMenuWindow
	{
		internal Sprite Background;

		public UIMenu ParentMenu { get; set; }

		public virtual bool Enabled { get; set; }

		internal virtual void Position(float y)
		{
		}

		internal virtual void UpdateParent()
		{
		}

		internal virtual void Draw()
		{
		}

		public void SetParentMenu(UIMenu Menu)
		{
			ParentMenu = Menu;
		}
	}
}
